using HomographyNet;
using SimpleOsciloscope.UI.FrequencyDetection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation.Peers;
using System.Windows.Documents;

namespace SimpleOsciloscope.UI
{

    public class CorrelationBasedFrequencyDetector: IFrequencyDetector
    {

        public int MaxCrosses = 100;

        /// <summary>
        /// Calculates the locations where signal crosses the y=0. to find so, ys are linearly interpolated
        /// </summary>
        /// <param name="ys"></param>
        /// <param name="y0"></param>
        /// <param name="xs"></param>
        /// <param name="dt">each interval in seconds</param>
        /// <returns>the Xs of points where signal crosses zero</returns>
        /// <exception cref="NotImplementedException"></exception>
        public static int Cross(double[] ys, double y0, double[] xs,int n,double dt)
        {
            //times where signal crosses the avg are added to lst
            double d1, d2;

            var cnt = 0;

            for (var i = 0; i < n - 1; i++)
            {
                d1 = ys[i] + y0;
                d2 = ys[i + 1] + y0;

                if (d1 * d2 > 0)
                    continue;

                if (ys[i] == ys[i + 1])
                    continue;

                if (d1 < 0) d1 = -d1;

                if (d2 < 0) d2 = -d2;

                var x0 = i * dt;
                var x1 = x0 + dt;

                var dx = x1 - x0;

                var r1 = d1 / (d1 + d2);

                var xx = x0 + r1 * dx;//location of zero

                xs[cnt++] = xx;
            }

            return cnt;
        }



        public static void Print2DArray<T>(T[,] matrix)
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    Console.Write(matrix[i, j] + "\t");
                }
                Console.WriteLine();
            }
        }

        public bool TryGetFrequency(short[] ys, double samplingRate, out double freq, out double phaseShift)
        {
            var y2s = ArrayPool.Double(ys.Length);

            for (int i = 0; i < ys.Length; i++)
                y2s[i] = ys[i];

            var res = TryGetFrequency(y2s, samplingRate, out freq,out phaseShift);

            ArrayPool.Return(ys);

            return res;
        }

        public bool TryGetFrequency(double[] ys, double samplingRate, out double freq, out double phaseShift)
        {
            if (!TryGetAvgCrossFrequency(ys, samplingRate, out freq, out phaseShift))
                return false;

            var F = freq;

            //should check for F/N and find minimum cross correlation

            var lambda = 1 / F;

            var N = MaxCrosses;

            var n = ys.Length;

            var corr = new SimpleCrossCorrelate();

            var errs = new double[N];

            for (var i = 1; i < N; i++)
            {
                var tau = lambda * i;

                corr.ys = ys;
                corr.Dx = 1 / samplingRate;

                corr.Tau = tau;
                corr.n = n/N;

                var I = corr.CrossCollerate();

                errs[i] = I; ;
            }

            var mult = -1;

            {
                var grpsCount = 10;

                var hist = HistogramData.Generate(errs, errs.Length, grpsCount);

                var regions = HistogramUtil.ClusterSimple(hist);

                var regs = regions.OrderBy(i => i.MinGroup).ToArray();

                var grps = new int[N];

                hist.GetGroups(errs, errs.Length, grps);

                var first = -1;

                for (var i2 = 1; i2 < regs.Length; i2++)
                {
                    var reg1 = regs[i2];

                    for (var i = 1; i < N; i++)
                    {
                        var g = grps[i];

                        if (g >= reg1.MinGroup && g <= reg1.MaxGroup)
                        {
                            first = i;
                            goto done;
                        }
                    }
                }

            done:


                if (first == -1)
                    //throw new NotImplementedException();
                    first = 1;


                mult = first;
            }


            var lambda2 = mult * lambda;

            freq = 1 / lambda2;

            return true;

            throw new NotImplementedException();

        }

        public bool TryGetAvgCrossFrequency( double[] ys, double samplingRate, out double freq, out double phaseShift)
        {
            var n = ys.Length;
            var k = 100;
            phaseShift = 0;

            var avg = HpVectorOperation.Sum(ys, n) / n;

            Trace.WriteLine("AVG: " + avg);
            var xs = ArrayPool.Double(n);

            var dt = 1 / samplingRate;

            var crosses = Cross(ys, -avg, xs, n, dt);

            var bs = ArrayPool.Double(n);
            var bsc = 0;

            {//fill bs
                bs.Clear();



                for (var i = 0; i < crosses - 1; i++)
                {
                    bs[i] = xs[i + 1] - xs[i];
                    bsc++;
                }
            }

            

            {
                //note: need to add a zero, in order to others can be distinguished (sort of scaling)
                bs[0] = 0;
                //possible bug: very large spikes in the signal (like 100x)
            }


            double[] xbs, wbs;

            HistogramData bsHistogram;

            {
                var histogram = bsHistogram = HistogramData.Generate(bs, bsc, k);
                var regions = HistogramUtil.ClusterSimple(histogram);

                xbs = new double[regions.Length];
                wbs = new double[regions.Length];

                var walls = histogram.GetWalls();

                var sws = Enumerable.Range(0, histogram.Groups).Select(i => new KahanSum()).ToArray();
                var swxs = Enumerable.Range(0, histogram.Groups).Select(i => new KahanSum()).ToArray();

                var min = histogram.Min;
                var max = histogram.Max;
                var m = histogram.Groups;
                var groups = ArrayPool.Int32(n);

                histogram.GetGroups(bs, bsc, groups);

                for (var i = 0; i < bsc; i++)
                {
                    var bi = bs[i];
                    var g = groups[i];

                    sws[g].Add(1);
                    swxs[g].Add(bi);
                }

                var tmpCnt = 0;

                foreach (var reg in regions)
                {
                    var st = reg.MinGroup;
                    var br = reg.MaxGroup;

                    var rng = Enumerable.Range(st, br - st).ToArray();

                    var ws = rng.Select(i => sws[i].Value).Sum();
                    var wxs = rng.Select(i => swxs[i].Value).Sum();

                    var xBar = wxs / ws;

                    xbs[tmpCnt] = xBar;
                    wbs[tmpCnt] = ws;

                    tmpCnt++;
                }
            }


            

            double[,] mtx;

            {
                var lng = wbs.Length;

                mtx = new double[lng, lng];

                for (int i = 0; i < lng; i++)
                {
                    for (int j = 0; j < lng; j++)
                    {
                        var ratio = wbs[i] / wbs[j];

                        if (ratio < 1) ratio = 1 / ratio;

                        if (i == j) ratio = 1;

                        mtx[i, j] = ratio;
                    }
                }
            }


            double lambda;

            var N = MaxCrosses;//max cross per wavelength

            {
                var lng = wbs.Length;
                var flags = new bool[lng];

                {
                    var mx = wbs.Max();

                    for (int i = 0; i < lng; i++)
                    {
                        if (wbs[i] > 1.0 / N * mx)
                            flags[i] = true;
                    }
                }

                {
                    lambda = 0.0;

                    var min2 = double.MaxValue;

                    for (int i = 0; i < lng; i++)
                        if (flags[i])
                            if (min2 > wbs[i])
                                min2 = wbs[i];

                    for (int i = 0; i < lng; i++)
                    {
                        if (flags[i])
                        {
                            lambda += xbs[i] * wbs[i] / min2;
                        }
                    }
                }


                {//phase
                    var mx = double.MinValue;

                    for (var i = 0; i < lng; i++)
                    {
                        if (flags[i])
                        {
                            if (xbs[i]>mx)
                            {
                                mx = xbs[i];
                            }
                        }
                    }

                    var grp = bsHistogram.GetGroup(mx);

                    for (int i = 0; i < n; i++)
                    {
                        var grp2 = bsHistogram.GetGroup(bs[i]);

                        if (grp2 == grp)
                        {
                            phaseShift = xs[i];
                        }
                    }
                }

            }

            ArrayPool.Return(xs);
            xs = null;


            freq = 1 / lambda;

            phaseShift = phaseShift % lambda;

            return true;
        }
    }


}

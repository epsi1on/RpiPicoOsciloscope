using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace SimpleOsciloscope.UI.FrequencyDetection
{
    internal class CorrFreqDetector2 : IFrequencyDetector
    {
        public double Cross = 0;

        public static bool TryGetFrequency2(short[] ys, double samplingRate, out double freq, out double phaseShift)
        {
            var corr = new CorrFreqDetector2();

            var min = ys.Min();
            var max = ys.Max();

            var d = max - min;

            var count = 5;

            var fs = new List<double>();

            for (var i = 1; i < count; i++)
            {
                var yi = min + i * d / (double)count;
                double phase = 0;
                double f = 0;

                corr.Cross = yi;
                corr.TryGetFrequency(ys, samplingRate, out f, out phase);

                fs.Add(f);
            }


            throw new NotImplementedException();

        }

        public bool TryGetFrequency(short[] ys, double samplingRate, out double freq, out double phaseShift)
        {
            var cpx = ArrayPool.Complex(ys.Length);
            var cs = ArrayPool.Double(ys.Length);
            var ds = ArrayPool.Double(ys.Length);

            

            var n = ys.Length;

            bool v1, v2;

            var counter = 0;

            for (var i = 0; i < n - 1; i++)
            {
                var dy1 = ys[i] - Cross;
                var dy2 = ys[i + 1] - Cross;

                if (Math.Sign(dy1) != Math.Sign(dy2))
                {
                    //assume both dy1 and dy2 are not zero at same time
                    var xi = i;
                    var xi1 = i + 1;

                    double yi = ys[i];
                    double yi1 = ys[i + 1];

                    //y-yi=m*(x-xi)
                    //(y-yi)/m+xi=x

                    var m = (yi1 - yi) / (1);
                    var x = (Cross - yi) / m + xi;

                    cs[counter++] = x;
                }
            }


            for (var i = 0; i < counter - 1; i++)
            {
                ds[i] = cs[i + 1] - cs[i];
            }

            freq = 0;
            phaseShift = 0;

            FftwUtil.CalcFft(ds, cpx, counter);


            return false;

            throw new NotImplementedException();
        }

        public bool TryGetFrequency(short[] ys, FftContext fftContext, double samplingRate, out double freq, out double phaseShift)
        {
            throw new NotImplementedException();
        }
    }
}

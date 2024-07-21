using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SimpleOsciloscope.UI
{

    public class HistogramData
    {
        public double Min, Max;

        public int[] Values;

        public int Groups { get { return Values.Length; } }


        public int this[int grp]
        {
            get { return Values[grp]; }
            set { Values[grp] = value; }
        }

        public double[] GetWalls()
        {
            var buf = new double[Values.Length + 1];

            var delta = (Max - Min) / Values.Length;

            for (var i = 0; i <= Values.Length; i++)
            {
                buf[i] = Min + i * delta;
            }
            return buf;
        }

        public static HistogramData Generate(double[] data,int n, int groups) 
        {
            //https://stackoverflow.com/a/36267687

            var min = double.MaxValue;// data.Min(); 
            var max = double.MinValue;// data.Max();

            {
                for (var i = 0; i < n; i++)
                {
                    var val = data[i];

                    if (val < min)
                        min = val;

                    if (val > max)
                        max = val;
                }
            }

            var w = max - min;

            var m = groups;

            var histogram = new int[m];

            if (min == max)
            {
                histogram[m / 2] = n;
            }
            else

            for (var i = 0; i < n; i++)
            {
                var val = data[i];

                var grp = (val - min) / (max - min) * (m );

                var g = (int)Math.Floor(grp);

                if (g == m)
                    g--;

                histogram[g]++;
            }

            return new HistogramData() { Min = min, Max = max, Values = histogram };
        }

        //values are whome this histogram generated
        public void GetGroups(double[] values,int n, int[] groups)
        {
            var max = this.Max;
            var min = this.Min;
            var data = this.Values;
            var m = Groups;

            for (var i = 0; i < n; i++)
            {
                var val = values[i];

                var grp = (val - min) / (max - min) * (m);

                var g = (int)Math.Floor(grp);

                if (g == m)
                    g--;

                groups[i] = g;
            }
        }

        public int GetGroup(double values)
        {
            var max = this.Max;
            var min = this.Min;
            var data = this.Values;
            var m = Groups;

            //for (var i = 0; i < n; i++)
            {
                var val = values;

                var grp = (val - min) / (max - min) * (m);

                var g = (int)Math.Floor(grp);

                if (g == m)
                    g--;

                return g;
            }
        }
    }

    [DebuggerDisplay("{MinGroup}-{MaxGroup}")]
    public struct HistogramRegion
    {
        public int MinGroup, MaxGroup;//both are included, floor(min) and ceiling(max)

        public HistogramRegion(int min, int max):this()
        {
            MinGroup = min;
            MaxGroup = max;
        }

        public void Evaluate(HistogramData data, out double x1, out double x2)
        {
            var st = this.MinGroup;
            var en = this.MaxGroup;

            var minVal = data.Min;
            var maxVal = data.Max;

            var delta = maxVal - minVal;

            var start = st * delta;


            throw new NotImplementedException();
        }
       
    }

    public static class HistogramUtil
    {
        //simplest cluster algorithm, spans are spaces with zero
        public static HistogramRegion[] ClusterSimple(HistogramData hist)
        {
            //var tmp = ArrayPool.Double(data.Length);

            //Array.Copy(data, tmp, tmp.Length);

            //Array.Sort(tmp,0,n);

            //var percentile = 0.5;

            //var minPerc = percentile / 100.0;
            //var maxPerc = 1 - percentile / 100.0;

            //var min = tmp.Min();// [(int)(minPerc * n)];
            //var max = tmp.Max();//6 [(int)(maxPerc * n)];

            //var lst = new List<Tuple<int, int, int>>();

            var buf = new List<HistogramRegion>();

            var hst = hist.Values;

            {
                var idx = 0;

                while (true)
                {
                    var st = NextIndexOfNonZero(hst, idx);

                    if (st != -1)
                    {
                        var en = NextIndexOfZero(hst, st);

                        var flag = false;

                        if (en == -1)
                        {
                            en = hst.Length;
                            flag = true;
                        }

                        var cnt = 0;

                        for (int i = st; i < en; i++)
                        {
                            cnt += hst[i];
                        }

                        buf.Add(new HistogramRegion(st, en));
                        //lst.Add(Tuple.Create(st, en, cnt));
                        idx = en;

                        if (flag)
                            break;
                    }
                    else
                    {
                        break;
                    }

                }
            }

            return buf.ToArray();

            /*
            var buf = new List<Tuple<double, long>>();


            foreach(var range in lst)
            {
                var st = range.Item1;
                var en = range.Item2;
                //var cnt = range.Item3;

                var ws = 0l;
                var wxs = 0l;

                for (var i = st; i < en; i++)
                {
                    var cnt = data[i];
                    ws += cnt;
                    wxs += cnt * i;
                }

                var center = (double)wxs / ws;

                buf.Add(Tuple.Create(center, ws));
            }


            return buf;

            */
            throw new Exception();
        }

        private static int NextIndexOfNonZero(int[] data, int idx)
        {
            var n = data.Length;

            for (var i = idx; i < n; i++)
                if (data[i] != 0)
                    return i;

            return -1;
        }

        private static int NextIndexOfZero(int[] data, int idx)
        {
            var n = data.Length;

            for (var i = idx; i < n; i++)
                if (data[i] == 0)
                    return i;

            return -1;
        }
    }
}

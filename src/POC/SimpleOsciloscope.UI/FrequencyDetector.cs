using HomographyNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SimpleOsciloscope.UI
{

    public interface IFrequencyDetector
    {
        bool TryGetFrequency( double[] ys, out double freq);
    }

    public class FrequencyDetector: IFrequencyDetector
    {
        public bool TryGetFrequency( double[] ys,out double freq)
        {
            /*
            var sum = 0l;

            for (var i = values.Length - 1; i >= 0; i--)
                sum += values[i];

            var avg = sum / (double)values.Length;
            */
            //https://stackoverflow.com/a/6288230

            var n = ys.Length;

            double avg;

            {
                var sum = 0d;

                for (var i = 0; i < n; i++)
                {
                    sum += ys[i];
                }

                freq = 0;

                /*
                for (var i = 0; i < n - 1; i++)
                {
                    if (xs[i + 1] <= xs[i])
                        return false;
                }*/

                avg = sum / n;
            }

            var lst = new List<double>();

            {
                double d1, d2;
                
                for (var i = 0; i < n - 1; i++)
                {
                    d1 = ys[i] - avg;
                    d2 = ys[i + 1] - avg;

                    if (d1 * d2 > 0)
                        continue;

                    if (ys[i] == ys[i + 1])
                        continue;

                    if (d1 < 0) d1 = -d1;

                    if (d2 < 0) d2 = -d2;

                    var x0 = i;
                    var x1 = i + 1;

                    var dx = x1 - x0;

                    var r1 = d1 / (d1 + d2);

                    var xx = x0 + r1 * dx;//location of

                    lst.Add(xx);
                }
            }

            var l2 = new List<double>();

            for(var i = 0;i< lst.Count-1;i++)
            {
                l2.Add(lst[i + 1] - lst[i]);
            }

            var shifts = 50;

            var arr = new double[shifts ];

            for (var shift = 1; shift < shifts; shift++)
            {
                var sum = new KahanSum();

                for (int i = 0; i < l2.Count - shift; i++)
                {
                    var diff = l2[i] - l2[i + shift];
                    sum.Add(diff * diff);
                }

                arr[shift] = sum.Value;
            }

            var min = arr.Min();
            var max = arr.Max();

            for (var i = 0; i < arr.Length; i++)
            {
                arr[i] /= max;
            }

            var mines = arr.Skip(1).Min();

            var best = -1;

            for (var i = 0; i < arr.Length; i++)
            {
                if (i == 0) continue;


                if (arr[i]==mines)
                {
                    best = i;
                    break;
                }

            }


            //if(mines > 0.1)
            //    return false;//not good enough
            
            var waveLength = 0.0;

            for (int i = 0; i < best; i++)
            {
                waveLength += l2[i];
            }

            var time = 1 / waveLength;

            freq = time;
            return true;
        }
    }


}

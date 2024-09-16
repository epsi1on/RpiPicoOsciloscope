using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleOsciloscope.UI
{
    public class PwmDsCalculator
    {
        public static void CalculateDutyCycle(short[] samples, out short min, out short max, out double dutyCycle)
        {

            min = max = -1;

            var hist = new long[4096];

            for (var i = 0;i<samples.Length;i++)
            {
                hist[samples[i]]++;
            }

            var percentile = 2;

            //get 98th percentile

            {
                var tot = samples.Length;

                var tmp = 0l;

                for (var i = 0; i < hist.Length; i++)
                {
                    tmp += hist[i];

                    if (tmp / (double)tot > percentile)
                    {
                        min = (short)i;
                        break;
                    }
                }

                tmp = 0;
                for (var i = hist.Length - 1; i >= 0; i--)
                {
                    tmp += hist[i];

                    if (tmp / (double)tot > percentile)
                    {
                        max = (short)i;
                        break;
                    }
                }
            }

            if (min == -1 || max == -1)
                throw new Exception();


            var avg = (max + min) / 2;

            var larger = samples.Count(i => i > avg);
            var smaller = samples.Count(i => i <= avg);

            var ratio = larger / ((double)samples.Length);

            dutyCycle = ratio;
        }
    }
}

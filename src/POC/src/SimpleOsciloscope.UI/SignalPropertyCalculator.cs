using SimpleOsciloscope.UI.FrequencyDetection;
using System;
using System.Linq;

namespace SimpleOsciloscope.UI
{
    public class SignalPropertyCalculator
    {
        public static SignalPropertyList Calculate()
        {
            var repo = UiState.Instance.CurrentRepo;

            var lst = repo.Samples as FixedLengthListRepo<short>;

            var ys = ArrayPool.Short(lst.FixedLength);

            lst.CopyTo(ys);

            var alpha = repo.LastAlpha;
            var beta = repo.LastBeta;
            var sampleRate = UiState.AdcConfig.SampleRate;


            var buf = new SignalPropertyList();


            buf.alpha = alpha;
            buf.beta = beta;

            {
                var min = short.MinValue;
                var max = short.MinValue;
                long sum = 0;

                short y;

                for (var i = 0; i < ys.Length; i++)
                {
                    y = ys[i];

                    if (y > max) max = y;
                    if (y < min) min = y;
                    sum += y;
                }

                buf.Min = min;
                buf.Max = max;
                buf.Avg = ((double)sum) / ys.Length;
            }


            if (buf.Min == buf.Max)
            {
                buf.Frequency = 0;

                return buf;
            }

            long[] histogram;
            long histogramSum;

            {
                histogram = new long[4096];

                for (int i = 0; i < ys.Length; i++)
                    histogram[ys[i]]++;

                histogramSum = histogram.Sum();
            }

            buf.MinPercentile1 = CalculateLowPercentile(histogram, histogramSum, 0.1);
            buf.MaxPercentile1 = CalculateHighPercentile(histogram, histogramSum, 0.1);

            buf.MinPercentile5 = CalculateLowPercentile(histogram, histogramSum, 5);
            buf.MaxPercentile5 = CalculateHighPercentile(histogram, histogramSum, 5);

            buf.Domain = (short)(buf.Max - buf.Min);
            buf.Percentile1Domain = (short)(buf.MaxPercentile1 - buf.MinPercentile1);
            buf.Percentile5Domain = (short)(buf.MaxPercentile5 - buf.MinPercentile5);


            {
                double freq, shiftRadian;
                var dtr = new HybridFrequencyDetector();


                if (!dtr.TryGetFrequency(ys, sampleRate, out freq, out shiftRadian))
                {
                    freq = -1;
                    shiftRadian = -1;
                }

                buf.Frequency= freq;
                buf.PhaseRadian = shiftRadian;
            }

            {
                var avg = (buf.MinPercentile1 + buf.MaxPercentile1) / 2.0;

                long less = 0;
                long greater = 0;

                for (int i = 0; i < histogram.Length; i++)
                {
                    if (i < avg)
                        less += histogram[i];

                    if (i > avg)
                        greater += histogram[i];
                }

                buf.PwmDutyCycle = greater / (double)(less + greater);
            }

            ArrayPool.Return(ys);
            return buf;
        }

        public static short CalculateLowPercentile(long[] hist,long histSum, double percentile)
        {
            var tot = histSum;// hist.Sum();

            var tmp = 0l;

            if (hist.Length > short.MaxValue)
                throw new Exception();


            for (short i = 0; i < hist.Length; i++)
            {
                tmp += hist[i];

                if (tmp / (double)tot > (0.01*percentile))
                {
                    return i;
                }
            }


            return -1;
        }

        public static short CalculateHighPercentile(long[] hist, long histSum, double percentile)
        {
            var tot = histSum;// hist.Sum();

            var tmp = 0l;

            if (hist.Length > short.MaxValue)
                throw new Exception();


            for (short i = (short)(hist.Length - 1); i >= 0; i--)
            {
                tmp += hist[i];

                if (tmp / (double)tot > (0.01 * percentile))
                {
                    return i;
                }
            }


            return -1;
        }
    }
}

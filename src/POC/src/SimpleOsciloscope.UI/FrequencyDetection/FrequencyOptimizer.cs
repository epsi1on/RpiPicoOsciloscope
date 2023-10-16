using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleOsciloscope.UI.FrequencyDetection
{

    //optimizes the frequency
    public class FrequencyOptimizer
    {
        public double Optimize(double[] signal, double sampleRate, double frequency)
        {
            //step 1: find multiply of frequency
            //sometimes frequency is N times of real frequency
            //fist detect N and find smallest wavelength which collerates the signal


            var F = GetHighestFrequency(signal,sampleRate,frequency);

            var w = 5000;
            var h = 300;

            var waveLength = 1 / frequency;

            var deltaX = waveLength * 4;//4 times of signal
            
            var bmp = new RgbBitmap(w, h);


            var n = signal.Length;

            for (var i = 0; i < n; i++)
            {
                var x = i / sampleRate;

            }

            
            //var trx=OneDTransformation.FromInOut(0,w,0,)
            {

            }

            throw new NotImplementedException();
        }


        private double GetHighestFrequency(double[] signal, double sampleRate, double frequency)
        {
            return frequency;
            //todo comment above line

            var n = signal.Length;
            var waveLength = 1 / frequency;

            var xs = ArrayPool.Double(n);
            var ys = ArrayPool.Double(n);

            Array.Copy(signal, ys, n);

            for (var i = 0; i < n; i++)
            {
                var x = i / sampleRate;
                x = x % waveLength;
                xs[i]= x;
            }

            Array.Sort(xs, ys);

            var k = 1000;

            var newX = ArrayPool.Double(k);
            var newY = ArrayPool.Double(k);

            {
                var dx = waveLength / (k - 1);

                for (var i = 0; i < k; i++)
                {
                    var x = i * dx;
                    var y = InterPolate(xs, ys, x);
                    newX[i] = x;
                    newY[i] = y;
                }

                double F2;
                double shift;

                new CorrelationBasedFrequencyDetector().TryGetFrequency(newY, 1, out F2,out shift);

                F2 = frequency / F2;/// frequency;
            }

            //MathNet.Numerics.Statistics.Correlation..
            throw new NotImplementedException();
        }

        static int[] Primes = new int[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37 };

        private static double SelfCrossCollerate(double[] signal, double sampleRate, double frequency)
        {
            throw new NotImplementedException();
        }

        private static double InterPolate(double[] sortedXs, double[] ys, double x)
        {
            var n = sortedXs.Length;

            if (x <= sortedXs[0])
                return ys[0];

            if (x >= sortedXs[n - 1])
                return ys[n - 1];


            for (int i = 0; i < n-1; i++)
            {
                if (x >= sortedXs[i] && x < sortedXs[i + 1])
                {
                    var x0 = sortedXs[i];
                    var x1 = sortedXs[i + 1];

                    var y0 = ys[i];
                    var y1 = ys[i + 1];

                    var dx = x1 - x0;
                    var dy = ys[i + 1] - ys[i];

                    var buf = y1 + (y1 - y0) / (x1 - x0) * (x - x1);

                    return buf;
                }
            }

            throw new NotImplementedException();
        }
    }

    public class SignalData
    {
        public double[] Signal;//signal values
        public double SampleRate;//how many samples per second
    }
}

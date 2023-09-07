using Accord.Math;
using MathNet.Numerics;
using MathNet.Numerics.IntegralTransforms;
using MathNet.Numerics.Statistics;
using MathNet.Numerics.Statistics.Mcmc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace SimpleOsciloscope.UI
{
    public class FftFrequencyDetector : IFrequencyDetector
    {
        /// <summary>
        /// calculates the frequency from input array
        /// </summary>
        /// <param name="xs">times</param>
        /// <param name="ys">volt</param>
        /// <param name="freq">[output] calculated frequency</param>
        /// <returns>true, if freq found, false otherwise</returns>
        public bool TryGetFrequency(double[] ys, double samplingRate, out double fre, out double phaseShift)
        {
            //https://stackoverflow.com/questions/3949324/calculate-autocorrelation-using-fft-in-matlab
            //https://stackoverflow.com/questions/59265603/how-to-find-period-of-signal-autocorrelation-vs-fast-fourier-transform-vs-power
            var n = ys.Length;
            phaseShift = 0;
            //double result = MCMCDiagnostics.ACF(ys, 20, x => x );

            var crs = Correlation.Auto(ys);

            var cpx = ArrayPool.Complex(n);
            var tmp = ArrayPool.Double(n);

            for (var i = 0; i < n; i++)
            {
                cpx[i] = new Complex(ys[i], 0);
            }

            //var sampleRate = 1;

            Fourier.Forward(cpx);

            var freq = Fourier.FrequencyScale(ys.Length, samplingRate);

            
            var omega = cpx;

            for (var i = 0; i < n; i++)
            {
                var c = cpx[i];

                tmp[i] = c.Real * c.Real + c.Imaginary * c.Imaginary;//as stackof answer
            }


            var img1 = ArrayGraphPlotter.Plot(tmp);

            Fourier.InverseReal(tmp, tmp.Length-2);

            var img2 = ArrayGraphPlotter.Plot(tmp);

           
            var ft = tmp;
            var mags = ft.Select(Math.Abs).ToArray();

            var max = double.MinValue;
            var maxIndex = -1;

            for (var i = 0;i<n/2;i++)
            {
                if (Math.Abs(mags[i]) > max)
                {
                    maxIndex= i;
                    max = Math.Abs(mags[i]);
                }
            }

            var mx = mags.Max();

            var signal_freq = 1/freq[(int)max];


            fre = signal_freq;

            return true;
        }
    }
}

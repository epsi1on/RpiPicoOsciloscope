/**/

using FFTW.NET;

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
        public bool TryGetFrequency_old(short[] ys, double samplingRate, out double fre, out double phaseRadian)
        {
            Complex[] tmp1 = null, tmp2 = null;
        //https://stackoverflow.com/questions/3949324/calculate-autocorrelation-using-fft-in-matlab
        //https://stackoverflow.com/questions/59265603/how-to-find-period-of-signal-autocorrelation-vs-fast-fourier-transform-vs-power


        //https://stackoverflow.com/a/7675171
        var n = ys.Length;
            phaseRadian = 0;

            if (tmp1 == null)
                tmp1 = new Complex[ys.Length];

            if (tmp2 == null)
                tmp2 = new Complex[ys.Length];


            var sum = ys.Sum(i => (long)i);
            var avg = sum / (double)ys.Length;

            for (int i = 0; i < ys.Length; i++)
            {
                tmp1[i] = new Complex(ys[i]-avg, 0);
            }

            using (var pinIn = new PinnedArray<Complex>(tmp1))
            using (var pinOut = new PinnedArray<Complex>(tmp2))
            {
                DFT.FFT(pinIn, pinOut);
                //DFT.IFFT(pinOut, pinOut);
            }



            var maxIdx = 0;


            for (int i = 1; i < tmp1.Length / 2; i++)
            {
                if (Math.Abs(tmp2[i].Magnitude) > Math.Abs(tmp2[maxIdx].Magnitude))
                    maxIdx = i;
            }

            var freq = maxIdx * samplingRate / tmp1.Length;
            var phs = phaseRadian = tmp2[maxIdx].Phase;

            for (int i = 1; i < tmp2.Length; i++)
            {
                tmp2[i] = new Complex(tmp2[i].Magnitude, 0);

                //if (Math.Abs(tmp2[i].Magnitude) > Math.Abs(tmp2[maxIdx].Magnitude))
                //    maxIdx = i;
            }

            

            


            using (var pinIn = new PinnedArray<Complex>(tmp1))
            using (var pinOut = new PinnedArray<Complex>(tmp2))
            {
                //DFT.FFT(pinIn, pinOut);
                DFT.IFFT(pinOut, pinIn);
            }


            for (int i = 1; i < tmp2.Length/2; i++)
            {
                //tmp2[i] = new Complex(tmp2[i].Magnitude, 0);

                if (Math.Abs(tmp1[i].Magnitude) > Math.Abs(tmp1[maxIdx].Magnitude))
                    maxIdx = i;
            }

            var vw = 1 / freq;

            //phaseShift = (  phs/( 2*Math.PI)) * vw;


            var tt = tmp2[maxIdx];

            fre = freq;// Math.Abs(tt.Imaginary);

            //Console.WriteLine(tmp1[i] / tmp2[i]);

            //FFTW.NET.DFT.FFT(tmp1, tmp2, PlannerFlags.Default, 1);


            return true;
        }

        public bool TryGetFrequency(short[] ys, FftContext fftContext, double samplingRate, out double fre, out double phaseRadian)
        {
            //https://stackoverflow.com/questions/3949324/calculate-autocorrelation-using-fft-in-matlab
            //https://stackoverflow.com/questions/59265603/how-to-find-period-of-signal-autocorrelation-vs-fast-fourier-transform-vs-power
            //https://stackoverflow.com/a/7675171
            
            var cpx = fftContext.Context;

            var maxIdx = 1;

            {
                var mags = fftContext.Magnitudes;

                for (int i = 1; i < cpx.Length / 2; i++)
                {
                    if (mags[i] > mags[maxIdx])
                        maxIdx = i;
                }
            }

            fre = maxIdx * samplingRate / cpx.Length;

            phaseRadian = cpx[maxIdx].Phase;

            return true;
        }

        private int[] FindKBiggestNumbersM(int[] testArray, int k)
        {
            int[] result = new int[k];
            int indexMin = 0;
            result[indexMin] = testArray[0];
            int min = result[indexMin];

            for (int i = 1; i < testArray.Length; i++)
            {
                if (i < k)
                {
                    result[i] = testArray[i];
                    if (result[i] < min)
                    {
                        min = result[i];
                        indexMin = i;
                    }
                }
                else if (testArray[i] > min)
                {
                    min = testArray[i];
                    result[indexMin] = min;
                    for (int r = 0; r < k; r++)
                    {
                        if (result[r] < min)
                        {
                            min = result[r];
                            indexMin = r;
                        }
                    }
                }
            }
            return result;
        }
    }
}

/**/

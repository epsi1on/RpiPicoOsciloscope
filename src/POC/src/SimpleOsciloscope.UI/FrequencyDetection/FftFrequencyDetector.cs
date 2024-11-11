/**/


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

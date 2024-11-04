using FFTW.NET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SimpleOsciloscope.UI.FrequencyDetection
{
    public static class FftwUtil
    {
        public static void CalcFft(short[] input, Complex[] output)
        {
            var i1 = ArrayPool.Complex(input.Length);

            for (int i = 0; i < input.Length; i++)
            {
                i1[i] = new Complex(input[i], 0);
            }

            using (var pinIn = new PinnedArray<Complex>(i1))
            using (var pinOut = new PinnedArray<Complex>(output))
            {
                DFT.FFT(pinIn, pinOut);
            }

            ArrayPool.Return(i1);
        }


        public static void CalcFft(double[] input, Complex[] output,int length)
        {
            var i1 = ArrayPool.Complex(input.Length);

            for (int i = 0; i < input.Length; i++)
            {
                i1[i] = new Complex(input[i], 0);
            }

            using (var pinIn = new PinnedArray<Complex>(i1))
            using (var pinOut = new PinnedArray<Complex>(output))
            {
                DFT.FFT(pinIn, pinOut);
            }

            ArrayPool.Return(i1);
        }
    }
}

using FFTW.NET;
using SharpFFTW;
using SharpFFTW.Double;
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
        public static void CalcFftSharp(short[] input, Complex[] output)
        {

            var inputt = new ComplexArray(input.Length);
            var outputt = new ComplexArray(output.Length);

            var length = 1000;

            var plan1 = Plan.Create1(length, inputt, outputt, Direction.Forward, Options.Estimate);


        }

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

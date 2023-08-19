using MathNet.Numerics.IntegralTransforms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

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
        public bool TryGetFrequency(double[] ys, out double freq)
        {
            var n = ys.Length;

            var cpx = ArrayPool.Complex(n);
            var tmp = ArrayPool.Double(n);

            for (var i = 0; i < n; i++)
            {
                cpx[i] = new Complex(ys[i], 0);
            }

            Fourier.Forward(cpx);

            for (var i = 0; i < n; i++)
            {
                var c = cpx[i];

                tmp[i] = c.Real * c.Real + c.Imaginary * c.Imaginary;//as stackof answer
            }

            Fourier.InverseReal(tmp, tmp.Length - 2);


            {
                var max = double.MinValue;
                var maxIndex = -1;

                for (var i = 0; i < n; i++)
                {
                    if (max < Math.Abs(tmp[i]))
                    {
                        maxIndex = i;
                        max = Math.Abs(tmp[i]);
                    }
                }

                var f = 1 / max;
            }

            throw new NotImplementedException();
        }
    }
}

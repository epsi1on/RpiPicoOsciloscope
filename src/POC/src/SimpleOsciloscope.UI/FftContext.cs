using SimpleOsciloscope.UI.FrequencyDetection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SimpleOsciloscope.UI
{
    public class FftContext:IDisposable
    {
        public double[] Magnitudes;
        public double[] Phases;
        public Complex[] Context;

        public static FftContext FromSignal(short[] signal)
        {
            var n = signal.Length;

            var ctx = ArrayPool.Complex(n);
            var ph = ArrayPool.Double(n);
            var mag = ArrayPool.Double(n);

            Array.Clear(ctx, 0, n);
            Array.Clear(ph, 0, n);
            Array.Clear(mag, 0, n);

            FftwUtil.CalcFftSharp(signal, ctx);

            var buf = new FftContext();
            buf.Context= ctx;
            buf.Magnitudes = mag;
            buf.Phases = ph;
            buf.Update();

            return buf;
        }

        private void Update()
        {
            var n = Context.Length;

            int sz;

            unsafe
            {
                sz = sizeof(Complex);
            }

            var context = this.Context;

            var mgs = this.Magnitudes;
            var phs = this.Phases;

            //TODO: optimize with SIMD, or unroll or other stuff
            for (var i = 0; i < n; i++)
            {
                mgs[i] = context[i].Magnitude;
                phs[i] = context[i].Phase;
            }
        }

        public void Dispose()
        {
            ArrayPool.Return(Context);
            ArrayPool.Return(Magnitudes);
            ArrayPool.Return(Phases);
        }
    }
}

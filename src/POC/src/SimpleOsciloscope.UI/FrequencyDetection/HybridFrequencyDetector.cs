using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleOsciloscope.UI.FrequencyDetection
{
    //use both fft and inuvative
    internal class HybridFrequencyDetector : IFrequencyDetector
    {

        FftFrequencyDetector fft = new FftFrequencyDetector();

        CorrelationBasedFrequencyDetector cor = new CorrelationBasedFrequencyDetector();


        public bool TryGetFrequency(short[] ys, double samplingRate, out double freq, out double phaseShift)
        {
            double f, d;

            {
                

                if (!fft.TryGetFrequency(ys, samplingRate, out f, out d))
                {
                    freq = f;
                    phaseShift = d;

                    return false;
                }
            }


            {
                cor.preferredFreq = f;


                double f2, p2;

                var res = cor.TryGetFrequency(ys, samplingRate, out f2, out p2);

                if (res)
                {
                    freq = f2;
                    phaseShift = p2;
                    return true;
                }
                else
                {
                    freq = f;
                    phaseShift = d;
                    return true;
                }


            }

            throw new NotImplementedException();
        }
    }
}

using System.Numerics;

namespace SimpleOsciloscope.UI
{
    public interface IFrequencyDetector
    {
        //bool TryGetFrequency(short[] ys, double samplingRate, out double freq, out double phaseShift);

        bool TryGetFrequency(short[] ys, FftContext fftContext, double samplingRate, out double freq, out double phaseShift);
    }


}

namespace SimpleOsciloscope.UI
{
    public interface IFrequencyDetector
    {
        bool TryGetFrequency( double[] ys, double samplingRate, out double freq,out double phaseShift);
    }


}

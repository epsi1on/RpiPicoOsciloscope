namespace SimpleOsciloscope.UI
{
    public interface IFrequencyDetector
    {
        bool TryGetFrequency( short[] ys, double samplingRate, out double freq,out double phaseShift);
    }


}

using System.Windows.Media.Imaging;

namespace SimpleOsciloscope.UI
{
    public interface IScopeRenderer
    {
        RgbBitmap Render(out double frequency);
        WriteableBitmap Render2(out double frequency);

        WriteableBitmap Render2(out double frequency, out double min, out double max);

        WriteableBitmap Render3(SignalPropertyList properties);

        RgbBitmap Render();

        void Clear();

    }
}
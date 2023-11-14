using System.Windows.Media.Imaging;

namespace SimpleOsciloscope.UI
{
    public interface IScopeRenderer
    {
        RgbBitmap Render(out double frequency);
        WriteableBitmap Render2(out double frequency);

        RgbBitmap Render();

    }
}
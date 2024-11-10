using System;
using System.Windows.Media.Imaging;

namespace SimpleOsciloscope.UI
{
    public interface IScopeRenderer
    {
        [Obsolete]
        RgbBitmap Render(out double frequency);

        [Obsolete]
        WriteableBitmap Render2(out double frequency);

        [Obsolete]
        WriteableBitmap Render2(out double frequency, out double min, out double max);

        [Obsolete]
        WriteableBitmap Render3(SignalPropertyList properties);


        void DoRender(BitmapContext context, SignalPropertyList props);


        RgbBitmap Render();

        void Clear(BitmapContext context);


        void Zoom(double delta, int x, int y);


        void ReSetZoom();

        string GetPointerValue(double x, double y);

        //void SetEnabled(bool enabled);
    }
}
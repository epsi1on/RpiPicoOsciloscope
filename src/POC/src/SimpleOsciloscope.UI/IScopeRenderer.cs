namespace SimpleOsciloscope.UI
{
    public interface IScopeRenderer
    {
        RgbBitmap Render(out double frequency);

        RgbBitmap Render();

    }
}
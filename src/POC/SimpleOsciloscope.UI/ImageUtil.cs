using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SimpleOsciloscope.UI
{
    public class ImageUtil
	{
        public static void CopyToBitmap(RgbBitmap bmp, WriteableBitmap target)
        {
            if (target.Format != PixelFormats.Bgr24)
                throw new Exception();


            var h = bmp.Height; var w = bmp.Width;

            var buf = target;//
                             //new WriteableBitmap(w, h, 96, 96, pixelFormat: PixelFormats.Rgb24, null);

            buf.Lock();

            System.Span<byte> sp;

            unsafe
            {
                var pixelCount = buf.PixelHeight * buf.BackBufferStride;
                sp = new System.Span<byte>(buf.BackBuffer.ToPointer(), pixelCount);

                var dt = bmp.Data;

                var ourStride = w * 3;
                var msStride = buf.BackBufferStride;

                //var ourStart = new IntPtr(&dt);
                //var msStart = buf.BackBuffer;

                int ourStart, msStart, length;

                length = ourStride;

                for (var row = 0; row < h; row++)
                {
                    ourStart = row * ourStride;
                    msStart = row * msStride;

                    for (var col = 0; col < length; col++)
                    {
                        sp[msStart + col] = dt[ourStart + col];
                    }
                }
            }

            buf.AddDirtyRect(new System.Windows.Int32Rect(0, 0, w, h));

            buf.Unlock();
        }

    }
}

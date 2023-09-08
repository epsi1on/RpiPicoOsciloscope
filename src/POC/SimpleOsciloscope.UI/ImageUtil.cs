using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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


            using(var ctx = target.GetBitmapContext())
            {
                var dt = bmp.Data;
                var length = dt.Length;
                Marshal.Copy(dt, 0, target.BackBuffer, length);
                buf.AddDirtyRect(new System.Windows.Int32Rect(0, 0, w, h));
                /*
                var pixelCount = buf.PixelHeight * buf.BackBufferStride;
                //sp = new System.Span<byte>(buf.BackBuffer.ToPointer(), pixelCount);

                

                var ourStride = w * 3;
                var msStride = buf.BackBufferStride;

                //var ourStart = new IntPtr(&dt);
                //var msStart = buf.BackBuffer;

                int ourStart, msStart, length;

                length = ourStride;

                unsafe
                {
                    
                    for (var row = 0; row < h; row++)
                    {
                        ourStart = row * ourStride;
                        msStart = row * msStride;

                        for (var col = 0; col < w; col++)
                        {
                            var r = dt[ourStart + col];
                            var g = dt[ourStart + col + 1];
                            var b = dt[ourStart + col + 2];


                            ctx.Pixels[msStart / 3 + col] = r << 16 + g << 8 + b;
                        }
                    }
                }

                */
            }
            /*

            buf.Lock();

            //System.Span<byte> sp;

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
            */
            
        }

        

    }
}

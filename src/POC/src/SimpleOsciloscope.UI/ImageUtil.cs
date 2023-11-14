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
        public static void Copy(BitmapContext src, BitmapContext target)
        {
            if (target.Format != src.Format)
                throw new Exception();

            var length = src.Length * 4;
            BitmapContext.BlockCopy(src, 0, target, 0, length);

            /*
            var h = src.Height;
            var w = src.Width;

            var buf = target;//
                             //new WriteableBitmap(w, h, 96, 96, pixelFormat: PixelFormats.Rgb24, null);

            var ctx = target;// using()

            unsafe{
                //Marshal.Copy()
                Bitm
                var srcPtr = new IntPtr(src.Pixels);
                var dstPtr = new IntPtr(target.Pixels);

                CopyMemory(srcPtr, dstPtr, (uint)length);
            }

            */
        }

        //https://stackoverflow.com/a/15976103
        [DllImport("kernel32.dll", EntryPoint = "CopyMemory", SetLastError = false)]
        public static extern void CopyMemory(IntPtr dest, IntPtr src, uint count);


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
                
            }
            
            
        }

        

    }
}

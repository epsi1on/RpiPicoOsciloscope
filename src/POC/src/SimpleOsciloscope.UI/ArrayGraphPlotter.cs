using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace SimpleOsciloscope.UI
{
    public class ArrayGraphPlotter
    {
        public static WriteableBitmap Plot(double[] ys, int h = 300, int w = 500)
        {
            var margin = 10;

            var bmp = new RgbBitmap(w, h);

            bmp.Clear();

            var minn = ys.Min();
            var maxx = ys.Max();

            var tx = OneDTransformation.FromInOut(0, ys.Length, margin, w - margin);
            var ty = OneDTransformation.FromInOut(minn, maxx, margin, h - margin);

            for (var i = 0; i < ys.Length; i++)
            {
                var c = ys[i];

                var x = i;

                var u = tx.Transform(i);
                var v = ty.Transform(c);

                bmp.SetPixel((int)u, (int)v, 255, 255, 255);


            }

            var b2 = new WriteableBitmap(w, h, 96, 96, System.Windows.Media.PixelFormats.Bgr24, null);

            ImageUtil.CopyToBitmap(bmp, b2);
            return b2;
        }

        public static WriteableBitmap Plot(double[] xs, double[] ys, int h = 300, int w = 500)
        {
            var margin = 10;

            var bmp = new RgbBitmap(w, h);

            bmp.Clear();

            var minY = ys.Min();
            var maxY = ys.Max();

            var xmin = xs.Min();
            var xmax = xs.Max();

            var tx = OneDTransformation.FromInOut(xmin, xmax, margin, w - margin);
            var ty = OneDTransformation.FromInOut(minY, maxY, margin, h - margin);

            for (var i = 0; i < ys.Length; i++)
            {
                var c = ys[i];

                var x = xs[i];

                var u = tx.Transform(x);
                var v = ty.Transform(c);

                bmp.SetPixel((int)u, (int)v, 255, 255, 255);
            }

            var b2 = new WriteableBitmap(w, h, 96, 96, System.Windows.Media.PixelFormats.Bgr24, null);

            ImageUtil.CopyToBitmap(bmp, b2);

            return b2;
        }
    }
}

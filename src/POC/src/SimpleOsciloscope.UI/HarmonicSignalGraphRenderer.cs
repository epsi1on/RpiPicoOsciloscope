using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;

namespace SimpleOsciloscope.UI
{

    public class HarmonicSignalGraphRenderer : IScopeRenderer
    {

        struct IntThickness
        {
            public int Left, Right, Top, Bottom;

            public IntThickness(int left, int top, int right, int bottom)
            {
                Left = left;
                Top = top;
                Right = right;
                Bottom = bottom;
            }
        }
        //public static int Width = 500;
        //public static int Height = 500;

        RgbBitmap BMP;
        WriteableBitmap Bmp2;

        static readonly IntThickness Margin = new IntThickness(40,30,20,10);
        static readonly int MarginLeft = 30;

        static readonly int CyclesToShow = 2;

        public unsafe RgbBitmap Render()
        {
            double f;

            return Render(out f);
        }


        private void DrawGrids(RgbBitmap bmp, int minY, int maxY)
        {
            var trsY = OneDTransformation.FromInOut(minY, maxY, Margin.Top, bmp.Height - Margin.Bottom);

            var count = 4;

            byte r = 128;
            byte b = 128;
            byte g = 0;

            var delta = ((maxY - minY) * 1.0 / count);

            for (int ii = 0; ii <= count; ii++)
            {
                var y = delta * ii;

                var yp = trsY.Transform(y);

                var maxX = bmp.Width - Margin.Right;

                for (int i = Margin.Left; i < maxX; i++)
                {
                    bmp.SetPixel(i, (int)yp, r, g, b);
                }

                var formattedText = new FormattedText("Test String", CultureInfo.GetCultureInfo("en-us"),
                    FlowDirection.LeftToRight, new Typeface(new FontFamily("Sans MS"), FontStyles.Normal,
                    FontWeights.Medium, FontStretches.Normal), 80.0, System.Windows.Media.Brushes.Black);

                var ctx = new BitmapContext();
                
                //WriteableBitmapEx.FillText(bmp, formattedText, 100, 100, Colors.Blue);
            }


            for (int i = Margin.Top; i < bmp.Height - Margin.Bottom; i++)
            {
                bmp.SetPixel(Margin.Left, i, r, g, b);
                bmp.SetPixel(bmp.Width - Margin.Right, i, r, g, b);
            }

        }

        private void DrawGrids(WriteableBitmap bmp, int minY, int maxY)
        {
            var trsY = OneDTransformation.FromInOut(minY, maxY, Margin.Top, bmp.Height - Margin.Bottom);

            var count = 4;

            byte r = 128;
            byte b = 128;
            byte g = 0;

            var col = Color.FromRgb(r, g, b);

            var delta = ((maxY - minY) * 1.0 / count);

            var h = bmp.PixelHeight;
            var w = bmp.PixelWidth;

            for (var ii = 0; ii <= count; ii++)
            {
                var y = delta * ii;

                var yp = trsY.Transform(y);

                var maxX = bmp.Width - Margin.Right;

                bmp.DrawLine(Margin.Left, (int)yp, w - Margin.Right, (int)yp, col);

                for (int i = Margin.Left; i < maxX; i++)
                {
                    //bmp.SetPixel(i, (int)yp, r, g, b);
                }

                var volt = 3.3 * (maxY - y) / maxY;

                var textSize = h/25.0;

                var formattedText = new FormattedText(volt.ToString("0.0") + "v", CultureInfo.GetCultureInfo("en-us"),
                    FlowDirection.LeftToRight, new Typeface(new FontFamily("Sans MS"), FontStyles.Normal,
                    FontWeights.Medium, FontStretches.Normal), textSize, System.Windows.Media.Brushes.Black);

                var ctx = new BitmapContext();

                WriteableBitmapEx.FillText(bmp, formattedText, 5, (int)(yp - textSize / 2), Colors.Blue);
            }

            bmp.DrawLine(Margin.Left, Margin.Top, Margin.Left, h-Margin.Bottom, col);
            bmp.DrawLine(w-Margin.Right, Margin.Top, w-Margin.Right, h-Margin.Bottom, col);

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="sampleRate">sample per second</param>
        public RgbBitmap Render(out double frequency)
        {
            var w = UiState.Instance.RenderBitmapWidth;
            var h = UiState.Instance.RenderBitmapHeight;


            if (BMP == null)
            {
                BMP = new RgbBitmap(w, h);
            }

            if (Bmp2 == null)
            {
                Bmp2 = new WriteableBitmap(w, h, 96, 96, PixelFormats.Pbgra32, null);
            }

            var repo = UiState.Instance.CurrentRepo;

            var arr = (FixedLengthListRepo<short>)repo.Samples;


            {

                if (arr.TotalWrites < arr.FixedLength)
                {
                    frequency = -1;
                    return BMP;
                }

            }

            var l = arr.FixedLength;

            var ys = ArrayPool.Short(l);
            var xs = ArrayPool.Double(l);

            arr.CopyTo(ys);

            var sampleRate = UiState.AdcConfig.SampleRate;

            {
                var deltaT = 1.0 / sampleRate;// repo.AdcSampleRate;

                for (int i = 0; i < l; i++)
                {
                    xs[i] = i * deltaT;
                }
            }


            double freq, shift;

            var dtr = new CorrelationBasedFrequencyDetector();
            dtr.MaxCrosses = 10;

            if (!dtr.TryGetFrequency(ys, sampleRate, out freq, out shift))
                throw new System.Exception();

            //freq = 970;

            var waveLength = 1 / freq;

            var sp = CyclesToShow;// cycles to show

            var twl = sp * waveLength;

            //var xs = ArrayPool.Float(l);
            //var ys = arr;// ArrayPool.Float(l);

            //var min = short.MinValue;
            //var max = short.MaxValue;

            /** /
            {
                //short tmp;

                for (var i = 0; i < l; i++)
                {
                    //xs[i] = xs[i] % twl;
                    //tmp = ys[i];

                    //if (tmp < min) min = tmp;
                    //if (tmp > max) max = tmp;
                }
            }
            /**/

            var min = 0;
            var max = MathUtil.MaxValueForBits(UiState.AdcConfig.ResolutionBits);//.Instance.CurrentRepo.AdcMaxValue;

            {
                var trsX = OneDTransformation.FromInOut(0, twl, Margin.Left, w - Margin.Right);
                var trsY = OneDTransformation.FromInOut(min, max, h - Margin.Bottom, Margin.Top);

                int x, y;

                byte r = 255;
                byte b = 255;
                byte g = 255;


                BMP.Clear();


                DrawGrids(BMP, min, max);

                //var windowSize = l / 10000;
                //var windowStart = l / 2;

                var dt = 1.0 / UiState.AdcConfig.SampleRate;//repo.AdcSampleRate;

                //using (var ctx = BMP.GetBitmapContext())
                {
                    //var ww = ctx.Width;

                    //var lamda = 1.0 / System.Math.Abs(freq);
                    //var lamdaCount = (int)(lamda / dt);//how many sample per lambda
                    //var drawWindowCount = 1000;//how many hoe signals drawn


                    var st = 0;
                    var en = l;// lamdaCount * drawWindowCount;

                    if (en > l)
                        en = l;

                    for (var i = st; i < en; i++)
                    {
                        var xi = xs[i];

                        xi = xi - shift;//% twl;

                        while (xi < 0)
                            xi += twl;

                        xi = xi % twl;

                        var ty = ys[i];

                        x = (int)trsX.Transform(xi);
                        y = (int)trsY.Transform(ty);

                        //var idx = (y * ww) + x;

                        if (x > 0 && y > 0 && x < w && y < h)
                            BMP.SetPixel(x, y, r, g, b);
                    }
                }
            }

            ArrayPool.Return(xs);
            ArrayPool.Return(ys);
            frequency = freq;

            return BMP;
        }

        public unsafe WriteableBitmap Render2(out double frequency)
        {
            var w = UiState.Instance.RenderBitmapWidth;
            var h = UiState.Instance.RenderBitmapHeight;


            if (Bmp2 == null)
            {
                Bmp2 = new WriteableBitmap(w, h, 96, 96, UiState.BitmapPixelFormat, null);
            }

            var repo = UiState.Instance.CurrentRepo;

            var arr = (FixedLengthListRepo<short>)repo.Samples;


            {

                if (arr.TotalWrites < arr.FixedLength)
                {
                    frequency = -1;
                    return Bmp2;
                }

            }

            var l = arr.FixedLength;

            var ys = ArrayPool.Short(l);
            var xs = ArrayPool.Double(l);

            arr.CopyTo(ys);

            var sampleRate = UiState.AdcConfig.SampleRate;

            {
                var deltaT = 1.0 / sampleRate;// repo.AdcSampleRate;

                for (int i = 0; i < l; i++)
                {
                    xs[i] = i * deltaT;
                }
            }


            double freq, shift;

            var dtr = new CorrelationBasedFrequencyDetector();
            dtr.MaxCrosses = 10;

            if (!dtr.TryGetFrequency(ys, sampleRate, out freq, out shift))
                throw new System.Exception();

            //freq = 970;

            var waveLength = 1 / freq;

            var sp = CyclesToShow;// cycles to show

            var twl = sp * waveLength;

            //var xs = ArrayPool.Float(l);
            //var ys = arr;// ArrayPool.Float(l);

            //var min = short.MinValue;
            //var max = short.MaxValue;

            /** /
            {
                //short tmp;

                for (var i = 0; i < l; i++)
                {
                    //xs[i] = xs[i] % twl;
                    //tmp = ys[i];

                    //if (tmp < min) min = tmp;
                    //if (tmp > max) max = tmp;
                }
            }
            /**/

            var min = 0;
            var max = MathUtil.MaxValueForBits(UiState.AdcConfig.ResolutionBits);//.Instance.CurrentRepo.AdcMaxValue;

            {
                var trsX = OneDTransformation.FromInOut(0, twl, Margin.Left, w - Margin.Right);
                var trsY = OneDTransformation.FromInOut(min, max, h - Margin.Bottom, Margin.Top);

                int x, y;

                byte r = 255;
                byte b = 255;
                byte g = 255;


                Bmp2.Clear(Colors.Black);


                DrawGrids(Bmp2, min, max);

                //var windowSize = l / 10000;
                //var windowStart = l / 2;

                var dt = 1.0 / UiState.AdcConfig.SampleRate;//repo.AdcSampleRate;

                //using (var ctx = BMP.GetBitmapContext())
                {
                    //var ww = ctx.Width;

                    //var lamda = 1.0 / System.Math.Abs(freq);
                    //var lamdaCount = (int)(lamda / dt);//how many sample per lambda
                    //var drawWindowCount = 1000;//how many hoe signals drawn


                    var st = 0;
                    var en = l;// lamdaCount * drawWindowCount;

                    if (en > l)
                        en = l;

                    using (var ctx = Bmp2.GetBitmapContext())
                    {
                        for (var i = st; i < en; i++)
                        {
                            var xi = xs[i];

                            xi = xi - shift;//% twl;

                            while (xi < 0)
                                xi += twl;

                            xi = xi % twl;

                            var ty = ys[i];

                            x = (int)trsX.Transform(xi);
                            y = (int)trsY.Transform(ty);

                            //var idx = (y * ww) + x;

                            if (x > 0 && y > 0 && x < w && y < h)
                                WriteableBitmapEx.SetPixel(ctx, x, y, r, g, b);
                        }
                    }
                   
                }
            }

            ArrayPool.Return(xs);
            ArrayPool.Return(ys);
            frequency = freq;

            return Bmp2;
            
        }

        object lc = new object();
    }
}

using SimpleOsciloscope.UI.FrequencyDetection;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SimpleOsciloscope.UI.Render
{
    internal class FftRender : IScopeRenderer
    {

        public FftRender() {

            ReSetZoom();
        }

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


        //RgbBitmap BMP;
        //WriteableBitmap Bmp2;
        static readonly IntThickness Margin = new IntThickness(40, 30, 20, 10);
        static readonly int MarginLeft = 30;


        public void Clear(BitmapContext context)
        {
            //this.Bmp2 = null;
            //this.BMP = null;

            context.Clear();
        }

        public RgbBitmap Render(out double frequency)
        {
            throw new NotImplementedException();
        }

        public RgbBitmap Render()
        {
            throw new NotImplementedException();
        }

        public WriteableBitmap Render2(out double frequency)
        {
            throw new NotImplementedException();
        }

        public WriteableBitmap Render2(out double frequency, out double min, out double max)
        {
            throw new NotImplementedException();
        }

        public double MinFreqShow, MaxFreqShow;

        public WriteableBitmap Render3(SignalPropertyList properties)
        {

            throw new NotImplementedException();

            var Bmp2 = (WriteableBitmap)null;
            double frequency, vmin, vmax;

            var w = UiState.Instance.RenderBitmapWidth;
            var h = UiState.Instance.RenderBitmapHeight;


            //if (Bmp2 == null)
            {
                //Bmp2 = new WriteableBitmap(w, h, 96, 96, UiState.BitmapPixelFormat, null);
            }

            var repo = UiState.Instance.CurrentRepo;

            var arr = (FixedLengthListRepo<short>)repo.Samples;
            //var arrf = (FixedLengthListRepo<float>)repo.SamplesF;

            var n = arr.FixedLength;

            var input = ArrayPool.Short(n);

            arr.CopyTo(input);

            var cpx = ArrayPool.Complex(n);

            //FftwUtil.CalcFft(input, cpx);

            var sampleRate = UiState.AdcConfig.SampleRate;

            var d = 50;
            var ct = 150;

            var minFreq = MinFreqShow;
            var maxFreq = MaxFreqShow;

            //if (minFreq < 0) minFreq = 0;
            //if (maxFreq > (int)sampleRate / 2) maxFreq = (int)sampleRate / 2;

            var maxMag = double.MinValue;

            
            for (var i = 0; i < cpx.Length; i++)
            {
                if (maxMag < cpx[i].Magnitude)
                    maxMag = cpx[i].Magnitude;
            }

            maxMag = Math.Log10(maxMag);

            var trsX = OneDTransformation.FromInOut(minFreq, maxFreq, Margin.Left, w - Margin.Right);
            var trsY = LastYTransform = OneDTransformation.FromInOut(0, maxMag, h - Margin.Bottom, Margin.Top);


            byte r = 255;
            byte b = 255;
            byte g = 255;

            if (Bmp2 == null)
                return null;

            Bmp2.Clear(Colors.Black);

            //DrawGridsHoriz(Bmp2, minFreq, maxFreq);

            int x, y;

            using (var ctx = Bmp2.GetBitmapContext())
            {
                var stFreq = Math.Max(minFreq, 0);
                var enFreq = Math.Min(maxFreq, sampleRate/2);

                var stId = stFreq * cpx.Length / sampleRate;
                var enId = enFreq * cpx.Length / sampleRate;

                for (var i = (int)stId; i < enId; i++)
                {
                    
                    var mag = cpx[i].Magnitude;
                    var freq = i * sampleRate / n;

                    var xi = freq;
                    var yi = Math.Log10(mag);

                    x = (int)trsX.Transform(xi);
                    y = (int)trsY.Transform(yi);

                    if (x > 0 && y > 0 && x < w && y < h)
                        WriteableBitmapEx.SetPixel(ctx, x, y, r, g, b);
                }
            }


            ArrayPool.Return(input);
            ArrayPool.Return(cpx);

            return Bmp2;
            //throw new NotImplementedException();

        }




        private void DrawGridsHoriz(BitmapContext context,double fMin,double fMax)
        {
            
            var trsX = OneDTransformation.FromInOut(fMin, fMax, Margin.Left, context.Width - Margin.Right);

            var count = 10;

            byte r = 128;
            byte b = 128;
            byte g = 0;

            var delta = ((fMax - fMin) * 1.0 / count);

            var dx = fMax - fMin;

            var ctx = context;
            //using (var ctx = bmp.GetBitmapContext())
                for (int ii = 0; ii <= count; ii++)
                {
                    var y = delta * ii+ fMin;

                    var yp = (int)trsX.Transform(y);


                    var maxX = context.Height - Margin.Top;

                    for (int i = Margin.Bottom; i < maxX; i++)
                    {
                        BitmapContextExtensions.SetPixel(ctx, (int)yp, i, r, g, b);
                    }

                    /**/

                    var frq = fMin+ ii * (fMax - fMin) / count;

                    var str = FriendlyStringUtil.ToSI(frq, "0.000") + "Hz";


                    var fontSize = 15;
                    
                    var formattedText = new FormattedText(str, CultureInfo.GetCultureInfo("en-us"),
                        FlowDirection.LeftToRight, new Typeface(new FontFamily("Sans MS"), FontStyles.Normal,
                        FontWeights.Medium, FontStretches.Normal), fontSize, System.Windows.Media.Brushes.Black);

                    context.DrawText(formattedText, yp, 10, Colors.White);
                    context.FillText(formattedText, yp, 10, Colors.White);

                //context.DrawText
                    /**/
                    //WriteableBitmapEx.FillText(bmp, formattedText, 100, 100, Colors.Blue);
                }

            /*

            for (int i = Margin.Top; i < bmp.Height - Margin.Bottom; i++)
            {
                bmp.SetPixel(Margin.Left, i, r, g, b);
                bmp.SetPixel(bmp.Width - Margin.Right, i, r, g, b);
            }

            */

        }


        private void DrawGridsVert(BitmapContext context, double fMin, double fMax)
        {
            var trsY = OneDTransformation.FromInOut(fMin, fMax, Margin.Left, context.Width - Margin.Top);

            var count = 10;

            byte r = 128;
            byte b = 128;
            byte g = 0;

            var delta = ((fMax - fMin) * 1.0 / count);

            var ctx = context;
            //using (bmp.GetBitmapContext())
                for (int ii = 0; ii <= count; ii++)
                {
                    var y = delta * ii;

                    var yp = trsY.Transform(y);



                    var maxX = context.Width - Margin.Right;

                    for (int i = Margin.Left; i < maxX; i++)
                    {
                        BitmapContextExtensions.SetPixel(ctx, i, (int)yp, r, g, b);
                    }

                    /*
                    var formattedText = new FormattedText("Test String", CultureInfo.GetCultureInfo("en-us"),
                        FlowDirection.LeftToRight, new Typeface(new FontFamily("Sans MS"), FontStyles.Normal,
                        FontWeights.Medium, FontStretches.Normal), 80.0, System.Windows.Media.Brushes.Black);

                    var ctx = new BitmapContext();

                    */
                    //WriteableBitmapEx.FillText(bmp, formattedText, 100, 100, Colors.Blue);
                }

            /*

            for (int i = Margin.Top; i < bmp.Height - Margin.Bottom; i++)
            {
                bmp.SetPixel(Margin.Left, i, r, g, b);
                bmp.SetPixel(bmp.Width - Margin.Right, i, r, g, b);
            }

            */

        }

        OneDTransformation LastYTransform;


        public void Zoom(double delta, int x, int y)
        {
            var trsX = OneDTransformation.FromInOut(MinFreqShow, MaxFreqShow, Margin.Left, UiState.Instance.RenderBitmapWidth - Margin.Right);

            var minFreqVisible = trsX.TransformBack(Margin.Left);
            var maxFreqVisible = trsX.TransformBack(UiState.Instance.RenderBitmapWidth - Margin.Right);
            var pointerFreq = trsX.TransformBack(x);

            var d1 = pointerFreq - minFreqVisible;
            var d2 = maxFreqVisible - pointerFreq;

            d1 *= 1 + delta;
            d2 *= 1 + delta;

            MinFreqShow = pointerFreq - d1;
            MaxFreqShow = pointerFreq + d2;

            //if (MinFreqShow < 0)
            //    MinFreqShow = 0;


        }

        public void ReSetZoom()
        {
            var sr = UiState.AdcConfig.SampleRate;

            this.MinFreqShow = 0;
            this.MaxFreqShow = sr / 2;
        }

        

        public string GetPointerValue(double x, double y)
        {
            var trsX = OneDTransformation.FromInOut(MinFreqShow, MaxFreqShow, Margin.Left, UiState.Instance.RenderBitmapWidth - Margin.Right);

            var pointerFreq = trsX.TransformBack(x);

            var mag = double.NaN;

            if (LastYTransform != null)
                mag = LastYTransform.TransformBack(y);

            var l = (UiState.Instance.CurrentRepo.Samples as FixedLengthListRepo<short>).FixedLength;

            var freq = FriendlyStringUtil.ToSI(pointerFreq, "0.00") + "Hz";
            var mg = FriendlyStringUtil.ToSI(mag, "0.00") + "dbV";

            var buf = freq + "\r\n" + mg;
            
            buf = freq;

            return buf;
        }

        public void DoRender(BitmapContext context, SignalPropertyList props)
        {
            double frequency, vmin, vmax;

            var w = UiState.Instance.RenderBitmapWidth;
            var h = UiState.Instance.RenderBitmapHeight;


            //if (Bmp2 == null)
            {
            //    Bmp2 = new WriteableBitmap(w, h, 96, 96, UiState.BitmapPixelFormat, null);
            }

            var repo = UiState.Instance.CurrentRepo;

            var arr = (FixedLengthListRepo<short>)repo.Samples;
            //var arrf = (FixedLengthListRepo<float>)repo.SamplesF;

            var n = arr.FixedLength;

            //var input = ArrayPool.Short(n);

            //arr.CopyTo(input);

            var cpx = props.FftContext;

            var sampleRate = UiState.AdcConfig.SampleRate;

            var d = 50;
            var ct = 150;

            var minFreq = MinFreqShow;
            var maxFreq = MaxFreqShow;

            //if (minFreq < 0) minFreq = 0;
            //if (maxFreq > (int)sampleRate / 2) maxFreq = (int)sampleRate / 2;

            var maxMag = double.MinValue;


            for (var i = 0; i < cpx.Length; i++)
            {
                if (maxMag < cpx[i].Magnitude)
                    maxMag = cpx[i].Magnitude;
            }

            maxMag = Math.Log10(maxMag);

            var trsX = OneDTransformation.FromInOut(minFreq, maxFreq, Margin.Left, w - Margin.Right);
            var trsY = LastYTransform = OneDTransformation.FromInOut(0, maxMag, h - Margin.Bottom, Margin.Top);


            byte r = 255;
            byte b = 255;
            byte g = 255;

            context.Clear(Colors.Black);

            DrawGridsHoriz(context, minFreq, maxFreq);

            int x, y;

            var ctx = context;
            //using (;// Bmp2.GetBitmapContext())
            {
                var stFreq = Math.Max(minFreq, 0);
                var enFreq = Math.Min(maxFreq, sampleRate / 2);

                var stId = stFreq * cpx.Length / sampleRate;
                var enId = enFreq * cpx.Length / sampleRate;

                for (var i = (int)stId; i < enId; i++)
                {

                    var mag = cpx[i].Magnitude;
                    var freq = i * sampleRate / n;

                    var xi = freq;
                    var yi = Math.Log10(mag);

                    x = (int)trsX.Transform(xi);
                    y = (int)trsY.Transform(yi);

                    if (x > 0 && y > 0 && x < w && y < h)
                        WriteableBitmapEx.SetPixel(ctx, x, y, r, g, b);
                }
            }


            //ArrayPool.Return(input);
            //ArrayPool.Return(cpx);

            //return Bmp2;
        }

        /*
        public void SetEnabled(bool enabled)
        {
            Enabled = enabled;
        }

        bool Enabled = false;
        */
    }
}

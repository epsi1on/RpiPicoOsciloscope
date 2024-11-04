using SimpleOsciloscope.UI.FrequencyDetection;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using System.Web.UI.WebControls;

namespace SimpleOsciloscope.UI.Render
{


    //Total Harmonic Distortion
    public class ThdRender:IScopeRenderer
    {

        public ThdRender()
        {

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


        RgbBitmap BMP;
        WriteableBitmap Bmp2;
        static readonly IntThickness Margin = new IntThickness(40, 130, 20, 10);
        static readonly int MarginLeft = 30;


        public void Clear()
        {
            this.Bmp2 = null;
            this.BMP = null;
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
            double frequency, vmin, vmax;

            var w = UiState.Instance.RenderBitmapWidth;
            var h = UiState.Instance.RenderBitmapHeight;


            if (Bmp2 == null)
            {
                Bmp2 = new WriteableBitmap(w, h, 96, 96, UiState.BitmapPixelFormat, null);
            }

            var repo = UiState.Instance.CurrentRepo;

            var arr = (FixedLengthListRepo<short>)repo.Samples;
            var arrf = (FixedLengthListRepo<float>)repo.SamplesF;

            var n = arr.FixedLength;

            var input = ArrayPool.Short(n);

            arr.CopyTo(input);

            var cpx = ArrayPool.Complex(n);

            FftwUtil.CalcFft(input, cpx);

            var sampleRate = UiState.AdcConfig.SampleRate;

            var minFreq = MinFreqShow;
            var maxFreq = MaxFreqShow;

            //if (minFreq < 0) minFreq = 0;
            //if (maxFreq > (int)sampleRate / 2) maxFreq = (int)sampleRate / 2;

            var maxMag = double.MinValue;

            var maxIdx = -1;

            for (var i = 1; i < cpx.Length; i++)//bypass dc offset
            {
                if (maxMag < cpx[i].Magnitude)
                {
                    maxMag = cpx[i].Magnitude;
                    maxIdx = i;
                }
                    
            }

            var maxMagLog = Math.Log10(maxMag);

            


            var trsX = OneDTransformation.FromInOut(minFreq, maxFreq, Margin.Left, w - Margin.Right);
            var trsY = LastYTransform = OneDTransformation.FromInOut(0, maxMagLog, h - Margin.Bottom, Margin.Top);


            byte r = 255;
            byte b = 255;
            byte g = 255;

            if (Bmp2 == null)
                return null;

            Bmp2.Clear(Colors.Black);

            DrawGridsHoriz(Bmp2, minFreq, maxFreq);

            int x, y;


            double THD;
            double THDFreq;

            using (var ctx = Bmp2.GetBitmapContext())
            {
                var st = Math.Max(minFreq, 0);
                var en = Math.Min(maxFreq, cpx.Length);

                for (var i = (int)st; i < en; i++)
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


                {//calculate thd

                    r = 255;
                    b = 0;
                    g = 255;


                    double baseFreq = maxIdx * sampleRate / cpx.Length;

                    baseFreq = properties.Frequency;

                    var bw = 1000;

                    if (bw <= 0)
                        bw = 1;

                    double fq = baseFreq;


                    var lst = new List<Tuple<double, double>>();

                    while (fq < maxFreq)
                    {
                        var start = fq - bw;
                        var end = fq + bw;

                        var startIdx = (int)(start * cpx.Length / sampleRate);
                        var endIdx = (int)(end * cpx.Length / sampleRate);

                        int maxIdx_ = -1;
                        double max_ = double.MinValue;

                        if (startIdx < 0)
                            startIdx = 0;

                        if (endIdx < 0)
                            endIdx = 0;

                        if (startIdx >= cpx.Length)
                            startIdx = cpx.Length-1;

                        if (endIdx >= cpx.Length)
                            endIdx = cpx.Length-1;


                        for (var i = startIdx; i < endIdx; i++)
                        {
                            if (cpx[i].Magnitude > max_)
                                max_ = cpx[maxIdx_ = i].Magnitude;
                        }

                        

                        var mag = max_;
                        var maxFreq_ = maxIdx_ * sampleRate / cpx.Length;

                        lst.Add(new Tuple<double, double>(maxFreq_, max_));

                        var xi = maxFreq_;
                        var yi = Math.Log10(mag);

                        x = (int)trsX.Transform(xi);
                        y = (int)trsY.Transform(yi);

                        var D = 5;


                        var x0 = (int)trsX.Transform(xi - bw / 2);
                        var y0 = y - D;

                        var x1 = (int)trsX.Transform(xi + bw / 2); ;
                        var y1 = y + D;

                        {
                            x0 = x0.Truncate(0, Bmp2.PixelWidth);
                            y0 = y0.Truncate(0, Bmp2.PixelHeight);

                            x1 = x1.Truncate(0, Bmp2.PixelWidth);
                            y1 = y1.Truncate(0, Bmp2.PixelHeight);
                        }
                        

                        WriteableBitmapEx.FillRectangle(ctx, x0, y0, x1, y1, r, g, b);

                        fq += 2 * baseFreq;
                    }

                    {
                        if (lst.Count != 0)
                        {
                            var baseFq = THDFreq = lst[0].Item1;
                            var baseMag = lst[0].Item2;

                            lst.RemoveAt(0);

                            var rms = Math.Sqrt(lst.Sum(i => i.Item2 * i.Item2));

                            THD = rms / baseMag;
                        }
                        else
                            THD = THDFreq = double.NaN;
                        
                    }

                }
            }

            {
                var str = FriendlyStringUtil.ToSI(THDFreq, "0.000") 
                    + "Hz" 
                    + " , "
                    + THD.ToString("p");


                str += " Faulty"; //this section have bugs
                var fontSize = 50;

                var formattedText = new FormattedText(str, CultureInfo.GetCultureInfo("en-us"),
                FlowDirection.LeftToRight, new Typeface(new FontFamily("Sans MS"), FontStyles.Normal,
                FontWeights.Medium, FontStretches.Normal), fontSize, System.Windows.Media.Brushes.Black);

                WriteableBitmapEx.DrawText(Bmp2, formattedText, 0,Bmp2.PixelHeight- fontSize,  Colors.White);
                WriteableBitmapEx.FillText(Bmp2, formattedText, 0,Bmp2.PixelHeight- fontSize,  Colors.White);
            }

            ArrayPool.Return(input);
            ArrayPool.Return(cpx);

            return Bmp2;
            //throw new NotImplementedException();
        }




        private void DrawGridsHoriz(WriteableBitmap bmp, double fMin, double fMax)
        {
            var trsX = OneDTransformation.FromInOut(fMin, fMax, Margin.Left, bmp.Width - Margin.Right);

            var count = 10;

            byte r = 128;
            byte b = 128;
            byte g = 0;

            var delta = ((fMax - fMin) * 1.0 / count);

            var dx = fMax - fMin;


            using (var ctx = bmp.GetBitmapContext())
                for (int ii = 0; ii <= count; ii++)
                {
                    var y = delta * ii + fMin;

                    var yp = (int)trsX.Transform(y);


                    var maxX = bmp.Height - Margin.Top;

                    for (int i = Margin.Bottom; i < maxX; i++)
                    {
                        WriteableBitmapEx.SetPixel(ctx, (int)yp, i, r, g, b);
                    }

                    /**/

                    var frq = fMin + ii * (fMax - fMin) / count;

                    var str = FriendlyStringUtil.ToSI(frq, "0.000") + "Hz";


                    var fontSize = 15;

                    var formattedText = new FormattedText(str, CultureInfo.GetCultureInfo("en-us"),
                        FlowDirection.LeftToRight, new Typeface(new FontFamily("Sans MS"), FontStyles.Normal,
                        FontWeights.Medium, FontStretches.Normal), fontSize, System.Windows.Media.Brushes.Black);

                    WriteableBitmapEx.DrawText(bmp, formattedText, yp, 10, Colors.White, 90);
                    WriteableBitmapEx.FillText(bmp, formattedText, yp, 10, Colors.White, 90);

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


        private void DrawGridsVert(WriteableBitmap bmp, double fMin, double fMax)
        {
            var trsY = OneDTransformation.FromInOut(fMin, fMax, Margin.Left, bmp.Width - Margin.Top);

            var count = 10;

            byte r = 128;
            byte b = 128;
            byte g = 0;

            var delta = ((fMax - fMin) * 1.0 / count);


            using (var ctx = bmp.GetBitmapContext())
                for (int ii = 0; ii <= count; ii++)
                {
                    var y = delta * ii;

                    var yp = trsY.Transform(y);



                    var maxX = bmp.Width - Margin.Right;

                    for (int i = Margin.Left; i < maxX; i++)
                    {
                        WriteableBitmapEx.SetPixel(ctx, i, (int)yp, r, g, b);
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
            return "";
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

        /*
        public void SetEnabled(bool enabled)
        {
            Enabled = enabled;
        }

        bool Enabled = false;
        */
    }
}

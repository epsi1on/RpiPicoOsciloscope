using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace SimpleOsciloscope.UI
{
    internal class HitBasedSignalGraphRender : IScopeRenderer
    {
        RgbBitmap BMP;

        static readonly int Margin = 10;


        public RgbBitmap Render()
        {
            var w = UiState.Instance.RenderBitmapWidth;
            var h = UiState.Instance.RenderBitmapHeight;

            
            var thres = 500;
            var preTrigger = 30;
            var haltTimeUs = 100;//how many micro seconds to be under thress to detect as hit end


            var dt = 1.0 / UiState.AdcConfig.SampleRate;
            var haltTimeS = 100 * dt;

            var haltTimeSamples = (int)(haltTimeS / dt);


            if (BMP == null)
            {
                BMP = new RgbBitmap(w, h);
            }

            if (BMP.Width != w || BMP.Height != h)
            {
                BMP = new RgbBitmap(w, h);
            }



            

            var repo = UiState.Instance.CurrentRepo;

            var arr = (FixedLengthListRepo<short>)repo.Samples;

            if (arr.TotalWrites < arr.FixedLength)
            {
                return BMP;
            }

            var l = arr.FixedLength; ;

            var ys = ArrayPool.Short(l);

            arr.CopyTo(ys);


            int avg;

            {
                var sum = 0l;

                for (int i = 0; i < l; i++)
                {
                    sum += ys[i];
                }

                avg = (int)(sum / arr.FixedLength);
            }

            var st = -1;
            var en = -1;

            for (int i = 0; i < ys.Length; i++)
            {
                if (ys[i] > avg + thres || ys[i] < avg - thres)
                {
                    st =  i - haltTimeSamples;
                    if (st < 0)
                        st = 0;
                    break;
                }
            }

            if (st == -1)
                return BMP;

            BMP.Clear();

            {
                var lstFlag = st;
                


                for (var i = st; i < ys.Length; i++)
                {
                    if (ys[i] > avg + thres || ys[i] < avg - thres)
                        lstFlag = i;


                    if(lstFlag + haltTimeSamples < i)
                    {
                        en = i;
                        break;
                    }
                }

                if (en == -1)
                    en = ys.Length;
            }

            {
                var l2 = en - st;
                st -= l2;
                en += l2;

                if (st < 0)
                    st = 0;

                if (en > ys.Length)
                    en = ys.Length;
            }

            var durrCount = en - st;

            var min = 0;
            var max = UiState.AdcConfig.MaxVoltage;//.Instance.CurrentRepo.AdcMaxValue;

            {
                var trsX = OneDTransformation.FromInOut(st, en, Margin, w - Margin);
                var trsY = OneDTransformation.FromInOut(max, min, Margin, h - Margin);

                

                byte r = 128;
                byte b = 128;
                byte g = 128;


                BMP.Clear();

                //using (var ctx = BMP.GetBitmapContext())
                {
                    int x, y;

                    for (var i = st; i < en; i++)
                    {
                        var tx = i;
                        var ty = ys[i];

                        x = (int)trsX.Transform(tx);
                        y = (int)trsY.Transform(ty);

                        //var idx = (y * ww) + x;

                        if (x > 0 && y > 0 && x < w && y < h)
                            BMP.SetPixel(x, y, r, g, b);
                    }
                }

                {
                    var avgY = (int)trsY.Transform(avg);
                    var thresPlus = (int)trsY.Transform(avg - thres);
                    var thresMinus = (int)trsY.Transform(avg + thres);

                    if (avgY < BMP.Height && avgY > 0)
                        for (var i = Margin; i < w - Margin; i++)
                            BMP.SetPixel(i, avgY, 255, 255, 255);

                    if (thresPlus < BMP.Height && thresPlus > 0)
                        for (var i = Margin; i < w - Margin; i++)
                            BMP.SetPixel(i, thresPlus, 255, 128, 255);

                    if (thresMinus < BMP.Height && thresMinus > 0)
                        for (var i = Margin; i < w - Margin; i++)
                            BMP.SetPixel(i, thresMinus, 255, 128, 255);
                }
            }

            return BMP;
        }


        public RgbBitmap Render(out double frequency)
        {
            frequency = 0;
            return Render();
        }

        public WriteableBitmap Render2(out double frequency)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
        }

        public WriteableBitmap Render2(out double frequency, out double min, out double max)
        {
            throw new NotImplementedException();
        }
    }
}

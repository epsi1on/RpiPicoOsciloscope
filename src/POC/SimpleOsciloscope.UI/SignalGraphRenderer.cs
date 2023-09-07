using System.Security.Cryptography.X509Certificates;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;

namespace SimpleOsciloscope.UI
{

    public class SignalGraphRenderer
	{

        //public static int Width = 500;
        //public static int Height = 500;

        RgbBitmap BMP;

		static readonly int Margin = 10;


		/// <summary>
		/// 
		/// </summary>
		/// <param name="data"></param>
		/// <param name="sampleRate">sample per second</param>
		public unsafe RgbBitmap Render(out double frequency)
		{
            var w = UiState.Instance.RenderBitmapWidth;
            var h = UiState.Instance.RenderBitmapHeight;


            if (BMP == null)
			{
				BMP = new RgbBitmap(w, h);
			}

			if (BMP.Width != w || BMP.Height != h)
			{
                BMP = new RgbBitmap(w, h);
            }


			
			var l = DataRepository.RepoLength;

			var repo = UiState.Instance.CurrentRepo;

            var arr = repo.Samples;

            if (arr.TotalWrites < arr.Count)
            {
                frequency = -1;
                return BMP;
            }
                

            var ys = ArrayPool.Short(l);
            var xs = ArrayPool.Double(l);

            arr.CopyTo(ys);

            {
                var deltaT = 1.0 / repo.SampleRate;

                for (int i = 0; i < l; i++)
                {
                    xs[i] = i * deltaT;
                }
            }


            double freq,shift;

            var dtr = new CorrelationBasedFrequencyDetector();
            dtr.MaxCrosses = 10;

            if (!dtr.TryGetFrequency(ys, repo.SampleRate, out freq,out shift))
                throw new System.Exception();

            if(freq< 0)
            {

            }
            //freq = 970;

            var waveLength = 1 / freq;

			var sp = 2;// cycles to show

			var twl = sp * waveLength;

            //var xs = ArrayPool.Float(l);
			//var ys = arr;// ArrayPool.Float(l);

			var min = double.MaxValue;
			var max = double.MinValue;

            {
				double tmp;

                for (var i = 0; i < l; i++)
                {
					xs[i] = xs[i] % twl;
					tmp = ys[i];

                    if (tmp < min) min = tmp;
                    if (tmp > max) max = tmp;
                }
            }

			{
                var trsX = OneDTransformation.FromInOut(0, twl, Margin, w - Margin);
                var trsY = OneDTransformation.FromInOut(max, min, Margin, h - Margin);

                int x, y;

				byte r = 128;
                byte b = 128;
                byte g = 128;


                BMP.Clear();

                var windowSize = l / 10000;
                var windowStart = l / 2;

                var dt = 1.0 / repo.SampleRate;

                //using (var ctx = BMP.GetBitmapContext())
				{
                    //var ww = ctx.Width;

                    var lamda = 1.0 / System.Math.Abs(freq);
                    var lamdaCount = (int)(lamda/dt);//how many sample per lambda
                    var drawWindowCount = 1000;//how many hoe signals drawn


                    var st = 0;
                    var en = l;// lamdaCount * drawWindowCount;

                    if (en > l)
                        en = l;

                    for (var i = st; i < en; i++)
                    {
                        var tx = xs[i] + shift;
                        var ty = ys[i];

                        x = (int)trsX.Transform(tx);
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

		object lc = new object();
	}
}

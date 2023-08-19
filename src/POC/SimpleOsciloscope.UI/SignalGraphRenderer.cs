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
		public unsafe RgbBitmap Render(int channelId)
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

            ChannelData chn;

            switch(channelId)
            {
                case 0:
                    chn = repo.Channel1;
                    break;
                case 1:
                    chn = repo.Channel2;
                    break;
                default:
                    throw new System.Exception();
            }


            if (chn.Sets < chn.Length)
                return BMP;

            var ys = ArrayPool.Double(l);
            var xs = ArrayPool.Double(l);

            chn.Copy(xs, ys);

            double freq;

            if (!new FrequencyDetector().TryGetFrequency(ys, out freq))
                freq = (float)Temps.Temp;

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

                //using (var ctx = BMP.GetBitmapContext())
				{
                    //var ww = ctx.Width;

                    for (var i = windowStart; i < windowStart+windowSize; i++)
                    {
                        var tx = xs[i];
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

            return BMP;
		}

		object lc = new object();
	}
}

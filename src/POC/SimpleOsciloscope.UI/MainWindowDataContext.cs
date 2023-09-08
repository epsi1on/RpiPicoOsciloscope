using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Media.Imaging;

namespace SimpleOsciloscope.UI
{
    public class MainWindowDataContext : INotifyPropertyChanged
    {

		#region INotifyPropertyChanged members and helpers

		public event PropertyChangedEventHandler PropertyChanged;

		protected static bool AreEqualObjects(object obj1, object obj2)
		{
			var obj1Null = ReferenceEquals(obj1, null);
			var obj2Null = ReferenceEquals(obj2, null);

			if (obj1Null && obj2Null)
				return true;

			if (obj1Null || obj2Null)
				return false;

			if (obj1.GetType() != obj2.GetType())
				return false;

			if (ReferenceEquals(obj1, obj2))
				return true;

			return obj1.Equals(obj2);
		}

		protected void OnPropertyChanged(params string[] propertyNames)
		{
			if (propertyNames == null)
				return;

			if (this.PropertyChanged != null)
				foreach (var propertyName in propertyNames)
					this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion

		#region TotalSmaples Property and field

		[Obfuscation(Exclude = true, ApplyToMembers = false)]
		public long TotalSmaples
		{
			get { return _TotalSmaples; }
			set
			{
				if (AreEqualObjects(_TotalSmaples, value))
					return;

				var _fieldOldValue = _TotalSmaples;

				_TotalSmaples = value;

				MainWindowDataContext.OnTotalSmaplesChanged(this, new PropertyValueChangedEventArgs<long>(_fieldOldValue, value));

				this.OnPropertyChanged("TotalSmaples");
			}
		}

		private long _TotalSmaples;

		public EventHandler<PropertyValueChangedEventArgs<long>> TotalSmaplesChanged;

		public static void OnTotalSmaplesChanged(object sender, PropertyValueChangedEventArgs<long> e)
		{
			var obj = sender as MainWindowDataContext;

			if (obj.TotalSmaplesChanged != null)
				obj.TotalSmaplesChanged(obj, e);
		}

		#endregion

		#region TotalSamplesStr Property and field

		[Obfuscation(Exclude = true, ApplyToMembers = false)]
		public string TotalSamplesStr
		{
			get { return _TotalSamplesStr; }
			set
			{
				if (AreEqualObjects(_TotalSamplesStr, value))
					return;

				var _fieldOldValue = _TotalSamplesStr;

				_TotalSamplesStr = value;

				MainWindowDataContext.OnTotalSamplesStrChanged(this, new PropertyValueChangedEventArgs<string>(_fieldOldValue, value));

				this.OnPropertyChanged("TotalSamplesStr");
			}
		}

		private string _TotalSamplesStr;

		public EventHandler<PropertyValueChangedEventArgs<string>> TotalSamplesStrChanged;

		public static void OnTotalSamplesStrChanged(object sender, PropertyValueChangedEventArgs<string> e)
		{
			var obj = sender as MainWindowDataContext;

			if (obj.TotalSamplesStrChanged != null)
				obj.TotalSamplesStrChanged(obj, e);
		}

		#endregion


		#region BitmapSource Property and field

		[Obfuscation(Exclude = true, ApplyToMembers = false)]
		public WriteableBitmap BitmapSource
		{
			get { return _BitmapSource; }
			set
			{
				if (AreEqualObjects(_BitmapSource, value))
					return;

				var _fieldOldValue = _BitmapSource;

				_BitmapSource = value;

                MainWindowDataContext.OnBitmapSourceChanged(this, new PropertyValueChangedEventArgs<WriteableBitmap>(_fieldOldValue, value));

				this.OnPropertyChanged("BitmapSource");
			}
		}

		private WriteableBitmap _BitmapSource;

		public EventHandler<PropertyValueChangedEventArgs<WriteableBitmap>> BitmapSourceChanged;

		public static void OnBitmapSourceChanged(object sender, PropertyValueChangedEventArgs<WriteableBitmap> e)
		{
			var obj = sender as MainWindowDataContext;

			if (obj.BitmapSourceChanged != null)
				obj.BitmapSourceChanged(obj, e);
		}

		#endregion



		#region FreqBase Property and field

		[Obfuscation(Exclude = true, ApplyToMembers = false)]
		public double FreqBase
		{
			get { return _FreqBase; }
			set
			{
				if (AreEqualObjects(_FreqBase, value))
					return;

				var _fieldOldValue = _FreqBase;

				_FreqBase = value;

				MainWindowDataContext.OnFreqBaseChanged(this, new PropertyValueChangedEventArgs<double>(_fieldOldValue, value));

				this.OnPropertyChanged("FreqBase");
			}
		}

		private double _FreqBase;

		public EventHandler<PropertyValueChangedEventArgs<double>> FreqBaseChanged;

		public static void OnFreqBaseChanged(object sender, PropertyValueChangedEventArgs<double> e)
		{
			var obj = sender as MainWindowDataContext;

			if (obj.FreqBaseChanged != null)
				obj.FreqBaseChanged(obj, e);

            obj.Freq = obj.FreqBase * Math.Pow(10, obj.FreqExponent);
        }

		#endregion

		#region FreqExponent Property and field

		[Obfuscation(Exclude = true, ApplyToMembers = false)]
		public short FreqExponent
		{
			get { return _FreqExponent; }
			set
			{
				if (AreEqualObjects(_FreqExponent, value))
					return;

				var _fieldOldValue = _FreqExponent;

				_FreqExponent = value;

				MainWindowDataContext.OnFreqExponentChanged(this, new PropertyValueChangedEventArgs<short>(_fieldOldValue, value));

				this.OnPropertyChanged("FreqExponent");
			}
		}

		private short _FreqExponent;

		public EventHandler<PropertyValueChangedEventArgs<short>> FreqExponentChanged;

		public static void OnFreqExponentChanged(object sender, PropertyValueChangedEventArgs<short> e)
		{
			var obj = sender as MainWindowDataContext;

			if (obj.FreqExponentChanged != null)
				obj.FreqExponentChanged(obj, e);

            obj.Freq = obj.FreqBase * Math.Pow(10, obj.FreqExponent);
        }

		#endregion

		#region Freq Property and field

		[Obfuscation(Exclude = true, ApplyToMembers = false)]
		public double Freq
		{
			get { return _Freq; }
			set
			{
				if (AreEqualObjects(_Freq, value))
					return;

				var _fieldOldValue = _Freq;

				_Freq = value;

				MainWindowDataContext.OnFreqChanged(this, new PropertyValueChangedEventArgs<double>(_fieldOldValue, value));

				this.OnPropertyChanged("Freq");
			}
		}

		private double _Freq;

		public EventHandler<PropertyValueChangedEventArgs<double>> FreqChanged;

		public static void OnFreqChanged(object sender, PropertyValueChangedEventArgs<double> e)
		{
			var obj = sender as MainWindowDataContext;

			if (obj.FreqChanged != null)
				obj.FreqChanged(obj, e);

			Temps.Temp = e.NewValue;
		}

		#endregion



		internal void Init()
        {
			{
				this.BitmapSource = new WriteableBitmap(UiState.Instance.RenderBitmapWidth, UiState.Instance.RenderBitmapHeight, 96, 96, pixelFormat: UiState.BitmapPixelFormat, null);

				var thr = new Thread(RenderLoopSync); thr.Start();

				this.FreqExponent = 4;
				this.FreqBase = 10;
            }


        }

		SignalGraphRenderer render = new SignalGraphRenderer();


		void RenderLoopSync()
		{

			var wait = (1 / UiState.RenderFramerate)*1000;

            while (true)
			{
				RenderShot();
				this.TotalSmaples = UiState.Instance.CurrentRepo.Samples.Index;// s.Sum(i => i.Sets);
                this.TotalSamplesStr = Utils.numStr(this.TotalSmaples);
				Thread.Sleep((int)wait);
            }
		}

        void RenderShot()
		{
			var sp = System.Diagnostics.Stopwatch.StartNew();

			double freq;

			var bmp = render.Render(out freq);

			Trace.WriteLine(string.Format("Render took {0} ms", sp.ElapsedMilliseconds));
			{
                Application.Current.Dispatcher.Invoke(new Action(() =>
				{
					CopyBitmap(bmp);
				}), System.Windows.Threading.DispatcherPriority.Render);
			}


        }

		void CopyBitmap(RgbBitmap bmp)
		{
			var dst = this.BitmapSource;

            var w = dst.PixelWidth;
            var h = dst.PixelHeight;

			ImageUtil.CopyToBitmap(bmp, dst);
		}



    }
}

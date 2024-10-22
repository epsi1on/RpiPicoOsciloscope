using FftSharp;
using SimpleOsciloscope.UI.HardwareInterface;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SimpleOsciloscope.UI
{
    /// <summary>
    /// Interaction logic for AdcSampler.xaml
    /// </summary>
    public partial class AdcSampler : System.Windows.Window
    {
		AdcSamplerDataContext Context;
        public AdcSampler()
        {
            InitializeComponent();
			DataContext = Context = new AdcSamplerDataContext();
        }


		public static double GetAdcMedian(string portName, AdcChannelInfo inf)
		{
			var wnd = new AdcSampler();

			wnd.Context.SerialPortName = portName;

			wnd.Context.Init();
			wnd.Context.ChannelMask = RpiPicoDaqInterface.GetChannelMask(inf.RpChannel);
            wnd.Context.Chn = inf; 
			
			wnd.Context.StartAdcAsync();
			wnd.Context.StartRenderAsync();
			
			var res = wnd.ShowDialog();

			wnd.Context.TakeSamples = false;
			//wnd.Context.intfs.StopAdc();
			//wnd.Context.intfs.DisConnect();

			Thread.Sleep(100);

			wnd.Context.intfs.DisConnect();

			if (res.HasValue && res.Value)
			{
				return wnd.Context.Center;
			}

			throw new Exception();
		}

        public class AdcSamplerDataContext : INotifyPropertyChanged
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


			public int[] Histogram = new int[4096];

			public bool Finished = false;

			#region HistogramImage Property and field

			[Obfuscation(Exclude = true, ApplyToMembers = false)]
			public WriteableBitmap HistogramImage
			{
				get { return _HistogramImage; }
				set
				{
					if (AreEqualObjects(_HistogramImage, value))
						return;

					var _fieldOldValue = _HistogramImage;

					_HistogramImage = value;

                    AdcSamplerDataContext.OnHistogramImageChanged(this, new PropertyValueChangedEventArgs<WriteableBitmap>(_fieldOldValue, value));

					this.OnPropertyChanged("HistogramImage");
				}
			}

			private WriteableBitmap _HistogramImage;

			public EventHandler<PropertyValueChangedEventArgs<WriteableBitmap>> HistogramImageChanged;

			public static void OnHistogramImageChanged(object sender, PropertyValueChangedEventArgs<WriteableBitmap> e)
			{
				var obj = sender as AdcSamplerDataContext;

				if (obj.HistogramImageChanged != null)
					obj.HistogramImageChanged(obj, e);
			}

			#endregion

			#region IsIdle Property and field

			[Obfuscation(Exclude = true, ApplyToMembers = false)]
			public bool IsIdle
			{
				get { return _IsIdle; }
				set
				{
					if (AreEqualObjects(_IsIdle, value))
						return;

					var _fieldOldValue = _IsIdle;

					_IsIdle = value;

					AdcSamplerDataContext.OnIsIdleChanged(this, new PropertyValueChangedEventArgs<bool>(_fieldOldValue, value));

					this.OnPropertyChanged("IsIdle");
				}
			}

			private bool _IsIdle;

			public EventHandler<PropertyValueChangedEventArgs<bool>> IsIdleChanged;

			public static void OnIsIdleChanged(object sender, PropertyValueChangedEventArgs<bool> e)
			{
				var obj = sender as AdcSamplerDataContext;

				if (obj.IsIdleChanged != null)
					obj.IsIdleChanged(obj, e);
			}

			#endregion

			#region Maximum Property and field

			[Obfuscation(Exclude = true, ApplyToMembers = false)]
			public double Maximum
			{
				get { return _Maximum; }
				set
				{
					if (AreEqualObjects(_Maximum, value))
						return;

					var _fieldOldValue = _Maximum;

					_Maximum = value;

					AdcSamplerDataContext.OnMaximumChanged(this, new PropertyValueChangedEventArgs<double>(_fieldOldValue, value));

					this.OnPropertyChanged("Maximum");
				}
			}

			private double _Maximum;

			public EventHandler<PropertyValueChangedEventArgs<double>> MaximumChanged;

			public static void OnMaximumChanged(object sender, PropertyValueChangedEventArgs<double> e)
			{
				var obj = sender as AdcSamplerDataContext;

				if (obj.MaximumChanged != null)
					obj.MaximumChanged(obj, e);
			}

			#endregion

			#region Minimum Property and field

			[Obfuscation(Exclude = true, ApplyToMembers = false)]
			public double Minimum
			{
				get { return _Minimum; }
				set
				{
					if (AreEqualObjects(_Minimum, value))
						return;

					var _fieldOldValue = _Minimum;

					_Minimum = value;

					AdcSamplerDataContext.OnMinimumChanged(this, new PropertyValueChangedEventArgs<double>(_fieldOldValue, value));

					this.OnPropertyChanged("Minimum");
				}
			}

			private double _Minimum;

			public EventHandler<PropertyValueChangedEventArgs<double>> MinimumChanged;

			public static void OnMinimumChanged(object sender, PropertyValueChangedEventArgs<double> e)
			{
				var obj = sender as AdcSamplerDataContext;

				if (obj.MinimumChanged != null)
					obj.MinimumChanged(obj, e);
			}

			#endregion

			#region Center Property and field

			[Obfuscation(Exclude = true, ApplyToMembers = false)]
			public double Center
			{
				get { return _Center; }
				set
				{
					if (AreEqualObjects(_Center, value))
						return;

					var _fieldOldValue = _Center;

					_Center = value;

					AdcSamplerDataContext.OnCenterChanged(this, new PropertyValueChangedEventArgs<double>(_fieldOldValue, value));

					this.OnPropertyChanged("Center");
				}
			}

			private double _Center;

			public EventHandler<PropertyValueChangedEventArgs<double>> CenterChanged;

			public static void OnCenterChanged(object sender, PropertyValueChangedEventArgs<double> e)
			{
				var obj = sender as AdcSamplerDataContext;

				if (obj.CenterChanged != null)
					obj.CenterChanged(obj, e);
			}

			#endregion

			#region SerialPortName Property and field

			[Obfuscation(Exclude = true, ApplyToMembers = false)]
			public string SerialPortName
			{
				get { return _SerialPortName; }
				set
				{
					if (AreEqualObjects(_SerialPortName, value))
						return;

					var _fieldOldValue = _SerialPortName;

					_SerialPortName = value;

					AdcSamplerDataContext.OnSerialPortNameChanged(this, new PropertyValueChangedEventArgs<string>(_fieldOldValue, value));

					this.OnPropertyChanged("SerialPortName");
				}
			}

			private string _SerialPortName;

			public EventHandler<PropertyValueChangedEventArgs<string>> SerialPortNameChanged;

			public static void OnSerialPortNameChanged(object sender, PropertyValueChangedEventArgs<string> e)
			{
				var obj = sender as AdcSamplerDataContext;

				if (obj.SerialPortNameChanged != null)
					obj.SerialPortNameChanged(obj, e);
			}

			#endregion

			#region AvailablePorts Property and field

			[Obfuscation(Exclude = true, ApplyToMembers = false)]
			public ObservableCollection<string> AvailablePorts
			{
				get { return _AvailablePorts; }
				set
				{
					if (AreEqualObjects(_AvailablePorts, value))
						return;

					var _fieldOldValue = _AvailablePorts;

					_AvailablePorts = value;

					AdcSamplerDataContext.OnAvailablePortsChanged(this, new PropertyValueChangedEventArgs<ObservableCollection<string>>(_fieldOldValue, value));

					this.OnPropertyChanged("AvailablePorts");
				}
			}

			private ObservableCollection<string> _AvailablePorts;

			public EventHandler<PropertyValueChangedEventArgs<ObservableCollection<string>>> AvailablePortsChanged;

			public static void OnAvailablePortsChanged(object sender, PropertyValueChangedEventArgs<ObservableCollection<string>> e)
			{
				var obj = sender as AdcSamplerDataContext;

				if (obj.AvailablePortsChanged != null)
					obj.AvailablePortsChanged(obj, e);
			}

			#endregion

			#region IsGood Property and field

			[Obfuscation(Exclude = true, ApplyToMembers = false)]
			public bool IsGood
			{
				get { return _IsGood; }
				set
				{
					if (AreEqualObjects(_IsGood, value))
						return;

					var _fieldOldValue = _IsGood;

					_IsGood = value;

					AdcSamplerDataContext.OnIsGoodChanged(this, new PropertyValueChangedEventArgs<bool>(_fieldOldValue, value));

					this.OnPropertyChanged("IsGood");
				}
			}

			private bool _IsGood;

			public EventHandler<PropertyValueChangedEventArgs<bool>> IsGoodChanged;

			public static void OnIsGoodChanged(object sender, PropertyValueChangedEventArgs<bool> e)
			{
				var obj = sender as AdcSamplerDataContext;

				if (obj.IsGoodChanged != null)
					obj.IsGoodChanged(obj, e);
			}

			#endregion

			#region GoodnessColor Property and field

			[Obfuscation(Exclude = true, ApplyToMembers = false)]
			public Color GoodnessColor
			{
				get { return _GoodnessColor; }
				set
				{
					if (AreEqualObjects(_GoodnessColor, value))
						return;

					var _fieldOldValue = _GoodnessColor;

					_GoodnessColor = value;

					AdcSamplerDataContext.OnGoodnessColorChanged(this, new PropertyValueChangedEventArgs<Color>(_fieldOldValue, value));

					this.OnPropertyChanged("GoodnessColor");
				}
			}

			private Color _GoodnessColor;

			public EventHandler<PropertyValueChangedEventArgs<Color>> GoodnessColorChanged;

			public static void OnGoodnessColorChanged(object sender, PropertyValueChangedEventArgs<Color> e)
			{
				var obj = sender as AdcSamplerDataContext;

				if (obj.GoodnessColorChanged != null)
					obj.GoodnessColorChanged(obj, e);
			}

			#endregion

			#region ChannelMask Property and field

			[Obfuscation(Exclude = true, ApplyToMembers = false)]
			public int ChannelMask
			{
				get { return _ChannelMask; }
				set
				{
					if (AreEqualObjects(_ChannelMask, value))
						return;

					var _fieldOldValue = _ChannelMask;

					_ChannelMask = value;

					AdcSamplerDataContext.OnChannelMaskChanged(this, new PropertyValueChangedEventArgs<int>(_fieldOldValue, value));

					this.OnPropertyChanged("ChannelMask");
				}
			}

			private int _ChannelMask;

			public EventHandler<PropertyValueChangedEventArgs<int>> ChannelMaskChanged;

			public static void OnChannelMaskChanged(object sender, PropertyValueChangedEventArgs<int> e)
			{
				var obj = sender as AdcSamplerDataContext;

				if (obj.ChannelMaskChanged != null)
					obj.ChannelMaskChanged(obj, e);
			}

			#endregion


			public AdcChannelInfo Chn;


			private DataRepository _Repository;



            int sampleRate = 500_000;

            Thread thrRender;
            Thread thrAdcRead;

            public void Init()
			{
                var hw = intfs = new RpiPicoDaqInterface(SerialPortName, sampleRate);
                _Repository = new DataRepository();
				_Repository.Init(sampleRate);

                hw.TargetRepository = _Repository;

				HistogramImage = BitmapFactory.New(512, 256);

				this.IsGoodChanged += (a, b) => SetGoodnessColor();
            }

			private void SetGoodnessColor()
			{
				this.GoodnessColor = this.IsGood ? Colors.Green : Colors.Red;
			}

			public void Render()
			{
                double adc_6sigma_thres = 0.9;

                double adc_sigma_thres = 5.0;


                if (ConfigurationManager.AppSettings.AllKeys.Contains("adc_6sigma_thres"))
                    adc_6sigma_thres = double.Parse(ConfigurationManager.AppSettings["adc_6sigma_thres"].ToString());

                if (ConfigurationManager.AppSettings.AllKeys.Contains("adc_sigma_thres"))
                    adc_sigma_thres = double.Parse(ConfigurationManager.AppSettings["adc_sigma_thres"].ToString());

                var histLength = 4096;

				var rep = _Repository.Samples as FixedLengthListRepo<short>;

				var arr = new short[rep.FixedLength];

				rep.CopyTo(arr);

				var hist = this.Histogram;

				for (int i = 0; i < hist.Length; i++)//clear
					hist[i] = 0;

				for (int i = 0; i < arr.Length; i++)//clear
					hist[arr[i]]++;

				var max = hist.Max();

				var firstNnz = Extensions.FindFirstIndexOf(hist, ii => ii != 0);
                var lastNnz = Extensions.FindLastIndexOf(hist, ii => ii != 0);

                if (firstNnz == -1)
                    firstNnz = 0;

                if (lastNnz == -1)
                    lastNnz = 4095;

				double avg, stdev2;

				{
                    var momentSum = 0l;
                    var wightSum = 0l;

                    for (var i = 0; i < histLength; i++)
                    {
                        momentSum += hist[i] * i;
						wightSum += hist[i];
                    }

                    avg = momentSum / (double)wightSum;

					var tmp = 0.0;
					double  tmp2;

                    for (var i = 0; i < histLength; i++)
                    {
						var val = i;

						var count = hist[i];

						var diff = (val - avg);

						tmp += diff * diff * count;
                    }

					stdev2 = tmp/hist.Sum();
                }

                this.Minimum = firstNnz;
                this.Maximum = lastNnz;
				this.Center = avg;

                var w = HistogramImage.Width;
				var h = HistogramImage.Height;

				var margin = 10;

                var ysc = OneDTransformation.FromInOut(0, max, h - margin,  margin);
				var xsc = OneDTransformation.FromInOut(firstNnz, lastNnz, margin, w - margin);

                var backGround = Colors.White;
                var foreGround = Colors.Gray;
                var foreGround2 = GoodnessColor;

				/*
				double yAvg, yStd2;//for histogram

                {
                    yAvg = hist.Sum() / (double)hist.Length;

                    yStd2 = hist.Sum(i => (i - yAvg) * (i - yAvg)) / histLength;
                }

				*/

                using (var tmp = HistogramImage.GetBitmapContext())
				{
					tmp.Clear();

					for (var i = 0; i < histLength; i++)
					{
						var x0 = (int)xsc.Transform(i);
						var x1 = (int)xsc.Transform(i + 1);

						if (x1 == x0)
							x1++;

						var y0 = (int)ysc.Transform(0);
						var y1 = (int)ysc.Transform(hist[i]);

						WriteableBitmapExtensions.FillRectangle(_HistogramImage, x0, y0, x1, y1, foreGround);

						{
							var xc = (int)xsc.Transform(avg);
                            var xl = (int)xsc.Transform(avg - Math.Sqrt(stdev2));
                            var xr = (int)xsc.Transform(avg + Math.Sqrt(stdev2));

							var yc = (int)ysc.Transform(max / 4);
							var yl = (int)ysc.Transform(0);

                            WriteableBitmapExtensions.FillTriangle(_HistogramImage, xl, yl, xc, yc, xr, yl, foreGround2);

                            WriteableBitmapExtensions.DrawLine(_HistogramImage, xc, (int)h-margin, xc, margin, foreGround2);
                        }


						{
							var std = Math.Sqrt(stdev2);

                            var xl = Center - 3 * std;
                            var xr = Center + 3 * std;

							if (xl < 0)
								xl = 0;

							if (xr > 4095)
								xr = 4095;

							var sum0 = 0l;

							for (var j = (int)xl; j <= xr; j++)
							{
								sum0 += hist[j];
							}

							var tot = hist.Sum();

							var ratio = sum0 / (double)tot;

							IsGood = ratio > adc_6sigma_thres && stdev2 < adc_sigma_thres* adc_sigma_thres;
                        }
                        /** /
						double gausFitted;

						{
                            var sc1 = 1 / (Math.Sqrt(2 * Math.PI * yStd2 * yStd2));
							var xu = i - yAvg;

							var sc2 = Math.Exp(-xu * xu / (2 * yStd2 * yStd2));

							gausFitted = sc1 * sc2;


							if(gausFitted > 0 && gausFitted < max)
							{
                                var yp2 = (int)ysc.Transform(gausFitted);

								WriteableBitmapExtensions.FillRectangle(_HistogramImage,
									x0, yp2 - 10, x1, yp2, foreGround2);
                            }
                        }
						/**/



                    }
				}
            }

            public void StartAdcAsync()
			{
                thrAdcRead = new Thread(StartReadSync);
                thrAdcRead.Start();
			}

			public bool TakeSamples = true;

            public void StartRenderSync()
			{
				while (TakeSamples)
				{
					Thread.Sleep(100);

					try
					{
                        Application.Current.Dispatcher.Invoke(() => Render());
                    }
					catch { }
					
				}


			}

            public void StartRenderAsync()
            {
                thrRender = new Thread(StartRenderSync);
                thrRender.Start();
            }

            public void StartReadSync()
			{
				var hw = intfs;

				hw.Channel = this.Chn;// RpiPicoDaqInterface.GetChannelMask();

                hw.Connect();
				hw.StopAdc();

				_Repository.Init(sampleRate);

				hw.SetupAdc();

				hw.ReadAdcData();
			}
			public RpiPicoDaqInterface intfs;

            public void Stop()
			{
				TakeSamples = false;

                intfs.StopAdc();

                if (thrAdcRead.IsAlive)
                    thrAdcRead.Join();

                while (thrRender.IsAlive)
				{
                    thrRender.Abort();
					Thread.Sleep(100);
                }
                    

            }

		}

        private void Button_Click(object sender, RoutedEventArgs e)
        {
			this.DialogResult = true;

			this.Context.intfs.StopAdc();

			this.Context.Stop();

            this.Context.intfs.DisConnect();

            this.Close();
        }
    }
}

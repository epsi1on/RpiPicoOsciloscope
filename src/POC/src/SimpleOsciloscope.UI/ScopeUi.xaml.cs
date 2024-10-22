using NAudio.Mixer;
using SimpleOsciloscope.UI.HardwareInterface;
using SimpleOsciloscope.UI.Properties;
using SimpleOsciloscope.UI.Render;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ChannelInfo = SimpleOsciloscope.UI.HardwareInterface.AdcChannelInfo;


namespace SimpleOsciloscope.UI
{
    /// <summary>
    /// Interaction logic for ScopeUi.xaml
    /// </summary>
    public partial class ScopeUi : Window
    {
        public ScopeUi()
        {
            InitializeComponent();
            this.DataContext = this.Context = new ContextClass();
            Context.Init();
            
        }

        ContextClass Context;


        public class ContextClass : INotifyPropertyChanged
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

                    ContextClass.OnTotalSmaplesChanged(this, new PropertyValueChangedEventArgs<long>(_fieldOldValue, value));

                    this.OnPropertyChanged("TotalSmaples");
                }
            }

            private long _TotalSmaples;

            public EventHandler<PropertyValueChangedEventArgs<long>> TotalSmaplesChanged;

            public static void OnTotalSmaplesChanged(object sender, PropertyValueChangedEventArgs<long> e)
            {
                var obj = sender as ContextClass;

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

                    ContextClass.OnTotalSamplesStrChanged(this, new PropertyValueChangedEventArgs<string>(_fieldOldValue, value));

                    this.OnPropertyChanged("TotalSamplesStr");
                }
            }

            private string _TotalSamplesStr;

            public EventHandler<PropertyValueChangedEventArgs<string>> TotalSamplesStrChanged;

            public static void OnTotalSamplesStrChanged(object sender, PropertyValueChangedEventArgs<string> e)
            {
                var obj = sender as ContextClass;

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

                    ContextClass.OnBitmapSourceChanged(this, new PropertyValueChangedEventArgs<WriteableBitmap>(_fieldOldValue, value));

                    this.OnPropertyChanged("BitmapSource");
                }
            }

            private WriteableBitmap _BitmapSource;

            public EventHandler<PropertyValueChangedEventArgs<WriteableBitmap>> BitmapSourceChanged;

            public static void OnBitmapSourceChanged(object sender, PropertyValueChangedEventArgs<WriteableBitmap> e)
            {
                var obj = sender as ContextClass;

                if (obj.BitmapSourceChanged != null)
                    obj.BitmapSourceChanged(obj, e);
            }

            #endregion

            #region Frequency Property and field

            [Obfuscation(Exclude = true, ApplyToMembers = false)]
            public double Frequency
            {
                get { return _Frequency; }
                set
                {
                    if (AreEqualObjects(_Frequency, value))
                        return;

                    var _fieldOldValue = _Frequency;

                    _Frequency = value;

                    ContextClass.OnFrequencyChanged(this, new PropertyValueChangedEventArgs<double>(_fieldOldValue, value));

                    this.OnPropertyChanged("Frequency");
                }
            }

            private double _Frequency;

            public EventHandler<PropertyValueChangedEventArgs<double>> FrequencyChanged;

            public static void OnFrequencyChanged(object sender, PropertyValueChangedEventArgs<double> e)
            {
                var obj = sender as ContextClass;

                if (obj.FrequencyChanged != null)
                    obj.FrequencyChanged(obj, e);
            }

            #endregion

            #region SampleRate Property and field

            [Obfuscation(Exclude = true, ApplyToMembers = false)]
            public long SampleRate
            {
                get { return _SampleRate; }
                set
                {
                    if (AreEqualObjects(_SampleRate, value))
                        return;

                    var _fieldOldValue = _SampleRate;

                    _SampleRate = value;

                    ContextClass.OnSampleRateChanged(this, new PropertyValueChangedEventArgs<long>(_fieldOldValue, value));

                    this.OnPropertyChanged("SampleRate");
                }
            }

            private long _SampleRate;

            public EventHandler<PropertyValueChangedEventArgs<long>> SampleRateChanged;

            public static void OnSampleRateChanged(object sender, PropertyValueChangedEventArgs<long> e)
            {
                var obj = sender as ContextClass;

                if (obj.SampleRateChanged != null)
                    obj.SampleRateChanged(obj, e);
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

                    ContextClass.OnAvailablePortsChanged(this, new PropertyValueChangedEventArgs<ObservableCollection<string>>(_fieldOldValue, value));

                    this.OnPropertyChanged("AvailablePorts");
                }
            }

            private ObservableCollection<string> _AvailablePorts;

            public EventHandler<PropertyValueChangedEventArgs<ObservableCollection<string>>> AvailablePortsChanged;

            public static void OnAvailablePortsChanged(object sender, PropertyValueChangedEventArgs<ObservableCollection<string>> e)
            {
                var obj = sender as ContextClass;

                if (obj.AvailablePortsChanged != null)
                    obj.AvailablePortsChanged(obj, e);
            }

            #endregion

            #region SelectedPort Property and field

            [Obfuscation(Exclude = true, ApplyToMembers = false)]
            public string SelectedPort
            {
                get { return _SelectedPort; }
                set
                {
                    if (AreEqualObjects(_SelectedPort, value))
                        return;

                    var _fieldOldValue = _SelectedPort;

                    _SelectedPort = value;

                    ContextClass.OnSelectedPortChanged(this, new PropertyValueChangedEventArgs<string>(_fieldOldValue, value));

                    this.OnPropertyChanged("SelectedPort");
                }
            }

            private string _SelectedPort;

            public EventHandler<PropertyValueChangedEventArgs<string>> SelectedPortChanged;

            public static void OnSelectedPortChanged(object sender, PropertyValueChangedEventArgs<string> e)
            {
                var obj = sender as ContextClass;

                if (obj.SelectedPortChanged != null)
                    obj.SelectedPortChanged(obj, e);
            }

            #endregion

            #region IsNotConnected Property and field

            [Obfuscation(Exclude = true, ApplyToMembers = false)]
            public bool IsNotConnected
            {
                get { return _IsNotConnected; }
                set
                {
                    if (AreEqualObjects(_IsNotConnected, value))
                        return;

                    var _fieldOldValue = _IsNotConnected;

                    _IsNotConnected = value;

                    ContextClass.OnIsNotConnectedChanged(this, new PropertyValueChangedEventArgs<bool>(_fieldOldValue, value));

                    this.OnPropertyChanged("IsNotConnected");
                }
            }

            private bool _IsNotConnected;

            public EventHandler<PropertyValueChangedEventArgs<bool>> IsNotConnectedChanged;

            public static void OnIsNotConnectedChanged(object sender, PropertyValueChangedEventArgs<bool> e)
            {
                var obj = sender as ContextClass;

                if (obj.IsNotConnectedChanged != null)
                    obj.IsNotConnectedChanged(obj, e);
            }

            #endregion

            #region MinMaxP2p Property and field

            [Obfuscation(Exclude = true, ApplyToMembers = false)]
            public string MinMaxP2p
            {
                get { return _MinMaxP2p; }
                set
                {
                    if (AreEqualObjects(_MinMaxP2p, value))
                        return;

                    var _fieldOldValue = _MinMaxP2p;

                    _MinMaxP2p = value;

                    ContextClass.OnMinMaxP2pChanged(this, new PropertyValueChangedEventArgs<string>(_fieldOldValue, value));

                    this.OnPropertyChanged("MinMaxP2p");
                }
            }

            private string _MinMaxP2p;

            public EventHandler<PropertyValueChangedEventArgs<string>> MinMaxP2pChanged;

            public static void OnMinMaxP2pChanged(object sender, PropertyValueChangedEventArgs<string> e)
            {
                var obj = sender as ContextClass;

                if (obj.MinMaxP2pChanged != null)
                    obj.MinMaxP2pChanged(obj, e);
            }

            #endregion

            #region ListenToAudio Property and field

            [Obfuscation(Exclude = true, ApplyToMembers = false)]
            public bool ListenToAudio
            {
                get { return _ListenToAudio; }
                set
                {
                    if (AreEqualObjects(_ListenToAudio, value))
                        return;

                    var _fieldOldValue = _ListenToAudio;

                    _ListenToAudio = value;

                    ContextClass.OnListenToAudioChanged(this, new PropertyValueChangedEventArgs<bool>(_fieldOldValue, value));

                    this.OnPropertyChanged("ListenToAudio");
                }
            }

            private bool _ListenToAudio;

            public EventHandler<PropertyValueChangedEventArgs<bool>> ListenToAudioChanged;

            public static void OnListenToAudioChanged(object sender, PropertyValueChangedEventArgs<bool> e)
            {
                var obj = sender as ContextClass;

                if (obj.ListenToAudioChanged != null)
                    obj.ListenToAudioChanged(obj, e);
            }

            #endregion

            #region DutyCycle Property and field

            [Obfuscation(Exclude = true, ApplyToMembers = false)]
            public double DutyCycle
            {
                get { return _DutyCycle; }
                set
                {
                    if (AreEqualObjects(_DutyCycle, value))
                        return;

                    var _fieldOldValue = _DutyCycle;

                    _DutyCycle = value;

                    ContextClass.OnDutyCycleChanged(this, new PropertyValueChangedEventArgs<double>(_fieldOldValue, value));

                    this.OnPropertyChanged("DutyCycle");
                }
            }

            private double _DutyCycle;

            public EventHandler<PropertyValueChangedEventArgs<double>> DutyCycleChanged;

            public static void OnDutyCycleChanged(object sender, PropertyValueChangedEventArgs<double> e)
            {
                var obj = sender as ContextClass;

                if (obj.DutyCycleChanged != null)
                    obj.DutyCycleChanged(obj, e);
            }

            #endregion

            #region AvailableChannels Property and field

            [Obfuscation(Exclude = true, ApplyToMembers = false)]
            public ObservableCollection<ChannelInfo> AvailableChannels
            {
                get { return _AvailableChannels; }
                set
                {
                    if (AreEqualObjects(_AvailableChannels, value))
                        return;

                    var _fieldOldValue = _AvailableChannels;

                    _AvailableChannels = value;

                    ContextClass.OnAvailableChannelsChanged(this, new PropertyValueChangedEventArgs<ObservableCollection<ChannelInfo>>(_fieldOldValue, value));

                    this.OnPropertyChanged("AvailableChannels");
                }
            }

            private ObservableCollection<ChannelInfo> _AvailableChannels;

            public EventHandler<PropertyValueChangedEventArgs<ObservableCollection<ChannelInfo>>> AvailableChannelsChanged;

            public static void OnAvailableChannelsChanged(object sender, PropertyValueChangedEventArgs<ObservableCollection<ChannelInfo>> e)
            {
                var obj = sender as ContextClass;

                if (obj.AvailableChannelsChanged != null)
                    obj.AvailableChannelsChanged(obj, e);
            }

            #endregion

            #region SelectedChannel Property and field

            [Obfuscation(Exclude = true, ApplyToMembers = false)]
            public ChannelInfo SelectedChannel
            {
                get { return _SelectedChannel; }
                set
                {
                    if (AreEqualObjects(_SelectedChannel, value))
                        return;

                    var _fieldOldValue = _SelectedChannel;

                    _SelectedChannel = value;

                    ContextClass.OnSelectedChannelChanged(this, new PropertyValueChangedEventArgs<ChannelInfo>(_fieldOldValue, value));

                    this.OnPropertyChanged("SelectedChannel");
                }
            }

            private ChannelInfo _SelectedChannel;

            public EventHandler<PropertyValueChangedEventArgs<ChannelInfo>> SelectedChannelChanged;

            public static void OnSelectedChannelChanged(object sender, PropertyValueChangedEventArgs<ChannelInfo> e)
            {
                var obj = sender as ContextClass;

                if (obj.SelectedChannelChanged != null)
                    obj.SelectedChannelChanged(obj, e);
            }

            #endregion

            #region SignalInfo Property and field

            [Obfuscation(Exclude = true, ApplyToMembers = false)]
            public SignalPropertyList SignalInfo
            {
                get {
                    return _SignalInfo; }
                set
                {
                    if (AreEqualObjects(_SignalInfo, value))
                        return;

                    var _fieldOldValue = _SignalInfo;

                    _SignalInfo = value;

                    ContextClass.OnSignalInfoChanged(this, new PropertyValueChangedEventArgs<SignalPropertyList>(_fieldOldValue, value));

                    this.OnPropertyChanged("SignalInfo");
                }
            }

            private SignalPropertyList _SignalInfo;

            public EventHandler<PropertyValueChangedEventArgs<SignalPropertyList>> SignalInfoChanged;

            public static void OnSignalInfoChanged(object sender, PropertyValueChangedEventArgs<SignalPropertyList> e)
            {
                var obj = sender as ContextClass;

                if (obj.SignalInfoChanged != null)
                    obj.SignalInfoChanged(obj, e);
            }

            #endregion

            /*
            public class ChannelInfo
            {
                public RpiPicoDaqInterface.Rp2040AdcChannels ChannelId { get; set; }
                public string Title { get; set; }
                public string Description { get; set; }
            }
            */

            internal void Init()
            {
                {
                    this.BitmapSource = new WriteableBitmap(UiState.Instance.RenderBitmapWidth, UiState.Instance.RenderBitmapHeight, 96, 96, pixelFormat: UiState.BitmapPixelFormat, null);
                    this.SampleRate = 500_000;// (long)UiState.AdcConfig.SampleRate;
                }

                {
                    RefreshPorts();
                    this.IsNotConnected = true;
                }

                {
                    //this.SampleRate = 500_000;
                }

                this.ListenToAudioChanged += AudioChanged;

                {
                    /*
                    var lst = new List<ChannelInfo>();
                    var ids = new RpiPicoDaqInterface.Rp2040AdcChannels[]{
                    RpiPicoDaqInterface.Rp2040AdcChannels.Gpio26,
                    RpiPicoDaqInterface.Rp2040AdcChannels.Gpio27,
                    RpiPicoDaqInterface.Rp2040AdcChannels.Gpio28,
                    RpiPicoDaqInterface.Rp2040AdcChannels.InternalReference,
                    RpiPicoDaqInterface.Rp2040AdcChannels.InternalTempratureSensor
                    };

                    var pins = RpiPicoDaqInterface.AdcPins();
                    var g26 = pins.FindFirstIndexOf(i => i == 26);
                    var g27 = pins.FindFirstIndexOf(i => i == 27);
                    var g28 = pins.FindFirstIndexOf(i => i == 28);

                    var titles = new string[] {
                        (g26 + 1).ToString(),
                        (g27 + 1).ToString(),
                        (g28 + 1).ToString(),
                                    "VRef ",
                        "Tmpr"
                    };

                    var descs = new string[] { 
                        "ADC Probe", 
                        "ADC Probe", 
                        "ADC Probe", 
                        "Internal Voltage Reference", 
                        "Internal Temprature Sensor" };


                    for (int i = 0; i < 5; i++)
                    {
                        var inf = new ChannelInfo();
                        inf.ChannelId = ids[i];
                        inf.Title = titles[i];
                        inf.Description = descs[i];
                        lst.Add(inf);
                    }


                    */

                    this.AvailableChannels =
                        //new ObservableCollection<ChannelInfo>(lst.OrderBy(i => i.Title).ToArray());
                        new ObservableCollection<ChannelInfo>(UiState.Instance.Channels);
                }

                {
                    var lastPort = Settings.Default.lastPort;
                    var lastChnIdx = Settings.Default.lastChannelIndex;
                    var lastSrate = Settings.Default.lastSampleRate;

                    if (AvailablePorts.Contains(lastPort))
                        SelectedPort = lastPort;

                    SelectedChannel = AvailableChannels[lastChnIdx];

                    SampleRate = lastSrate;
                }
            }


            IScopeRenderer render =
                //new HarmonicSignalGraphRenderer();
                //new HitBasedSignalGraphRender();
                new FftRender();

            void AudioChanged(object sender, PropertyValueChangedEventArgs<bool> e)
            {
                
            }


            void RenderLoopSync()
            {
                var wait = (1 / UiState.RenderFramerate) * 1000;

                while (true)
                {
                    if (this.IsNotConnected)
                        return;

                    RenderShot();
                    //this.TotalSmaples = UiState.Instance.CurrentRepo.Samples.Index;// s.Sum(i => i.Sets);
                    //this.TotalSamplesStr = Utils.numStr(this.TotalSmaples);
                    Thread.Sleep((int)wait);
                }
            }

            public void StartAdc()
            {

                if (this.SelectedChannel == null)
                {
                    MessageBox.Show("Select Channel");
                    return;
                }
                    
                UiState.Instance.CurrentRepo.Init((int)SampleRate);

                this.IsNotConnected = false;

                {
                    var thr = new Thread(RenderLoopSync);
                    thr.Priority = ThreadPriority.AboveNormal;
                    thr.Start();
                }

                {
                    var ifs = new RpiPicoDaqInterface(this.SelectedPort, SampleRate);
                    //new Stm32Interface(this.SelectedPort, SampleRate);
                    //new ArduinoInterface();
                    //new FakeDaqInterface();

                    ifs.Channel = this.SelectedChannel;

                    //ifs.ChannelMask = RpiPicoDaqInterface.GetChannelMask(this.SelectedChannel.RpChannel);

                    UiState.AdcConfig.Set(ifs);

                    //ifs.PortName = this.SelectedPort;
                    //ifs.AdcSampleRate = (int)SampleRate;
                    ifs.TargetRepository = UiState.Instance.CurrentRepo;
                    //var thr = new Thread(ifs.StartSync);

                    //https://stackoverflow.com/a/42988729

                    /*
                    ifs.OnPacketLoss += (a, b) =>
                    {
                        this.IsNotConnected = true;
                    };
                    */


                    var thr = new Thread(() =>
                      {
                          try
                          {
                              ifs.StartSync();
                          }
                          catch(Exception ex)
                          {
                              this.IsNotConnected = true;

                              ifs.DisConnect();

                              MessageBox.Show(ex.Message);
                          }
                          finally
                          {
                              this.IsNotConnected = true;

                              render.Clear();
                          }
                      });

                    thr.Start();

                }

                //UiState.Instance.CurrentRepo.AdcSampleRate = (int)this.SampleRate;
                UiState.AdcConfig.SampleRate = (int)SampleRate;

                
            }


            void RenderShot()
            {
                var sp = System.Diagnostics.Stopwatch.StartNew();

                double freq;

                double min, max;

                var prps = SignalPropertyCalculator.Calculate();

                this.SignalInfo = prps;

                this.DutyCycle = prps.PwmDutyCycle;


                //var bmp = render.Render2(out freq, out min, out max);
                var bmp = render.Render3(prps);


                {
                    var minF = prps.Min * prps.alpha + prps.beta;
                    var maxF = prps.Max * prps.alpha + prps.beta;

                    this.MinMaxP2p = string.Format("{0:0.00},{1:0.00},{2:0}mv", minF, maxF, (minF - maxF) * 1000);
                }

                var t = bmp;

                using (var ctx = bmp.GetBitmapContext())
                {
                    this.Frequency = prps.Frequency;

                    {
                        Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            //CopyBitmap(bmp);
                            CopyBitmap(ctx);
                        }), System.Windows.Threading.DispatcherPriority.Render);
                    }
                }
            }


            void CopyBitmap(RgbBitmap bmp)
            {
                var dst = this.BitmapSource;

                var w = dst.PixelWidth;
                var h = dst.PixelHeight;

                ImageUtil.CopyToBitmap(bmp, dst);
            }

            void CopyBitmap(BitmapContext ctx)
            {
                var dst = this.BitmapSource;

                var w = dst.PixelWidth;
                var h = dst.PixelHeight;

                
                using (var destCt = dst.GetBitmapContext(ReadWriteMode.ReadWrite))
                {
                    ImageUtil.Copy(ctx, destCt);
                    dst.AddDirtyRect(new Int32Rect(0, 0, dst.PixelWidth, dst.PixelHeight));

                    /*
                    unsafe
                    {

                        var l = destCt.Length;

                        var p1 = destCt.Pixels;
                        var p2 = ctx.Pixels;

                        for (int i = 0; i < l; i++)
                        {
                            *p1 = *p2;
                            p1++;
                            p2++;
                        }
                        //var srcPtr = new IntPtr(ct2.Pixels);

                        //var arr = new Span
                    }
                    */

                }

                
                //ImageUtil.CopyToBitmap(bmp, dst);
            }

            internal void RefreshPorts()
            {
                this.AvailablePorts = new ObservableCollection<string>(SerialPort.GetPortNames());
                this.SelectedPort = this.AvailablePorts.FirstOrDefault();
            }
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            /**/
            if (string.IsNullOrEmpty(Context.SelectedPort))
            {
                MessageBox.Show("invalid port");
                return;
            }

            if (Context.SampleRate <=0)
            {
                MessageBox.Show("invalid sample rate");
                return;
            }
            /**/


            Settings.Default.lastChannelIndex = Context.AvailableChannels.IndexOf(Context.SelectedChannel);
            Settings.Default.lastSampleRate = Context.SampleRate;
            Settings.Default.lastPort = Context.SelectedPort;
            Settings.Default.Save();

            Context.StartAdc();
        }

        private void BtnRefreshPorts_Click(object sender, RoutedEventArgs e)
        {
            Context.RefreshPorts();
        }

        private void BtnCalib_Click(object sender, RoutedEventArgs e)
        {
            Calibration.Calibrate(Context.SelectedPort);
        }
    }
}

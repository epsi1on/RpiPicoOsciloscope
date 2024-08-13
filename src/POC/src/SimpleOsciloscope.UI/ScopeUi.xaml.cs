using NAudio.Mixer;
using SimpleOsciloscope.UI.HardwareInterface;
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
            }


            IScopeRenderer render 
                = new HarmonicSignalGraphRenderer();
                //= new HitBasedSignalGraphRender();


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

            public void Connect()
            {
                UiState.Instance.CurrentRepo.Init((int)SampleRate);

                this.IsNotConnected = false;

                {
                    var thr = new Thread(RenderLoopSync);
                    thr.Priority = ThreadPriority.AboveNormal;
                    thr.Start();
                }

                {
                    var ifs =
                    new RpiPicoDaqInterface(this.SelectedPort, SampleRate);
                    //new Stm32Interface(this.SelectedPort, SampleRate);
                    //new ArduinoInterface();
                    //new FakeDaqInterface();

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

                var bmp = render.Render2(out freq, out min, out max);

                this.MinMaxP2p = string.Format("{0:0.00},{1:0.00},{2:0}mv", min, max, (max - min) * 1000);


                var t = bmp;// new WriteableBitmap(null);

                using (var ctx = bmp.GetBitmapContext())
                {
                    this.Frequency = freq;

                    //Trace.WriteLine(string.Format("Render took {0} ms", sp.ElapsedMilliseconds));

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

        private void Button_Click(object sender, RoutedEventArgs e)
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

            Context.Connect();
        }

        private void BtnRefreshPorts_Click(object sender, RoutedEventArgs e)
        {
            Context.RefreshPorts();
        }

        private void BtnCalib_Click(object sender, RoutedEventArgs e)
        {
            Calibration.Calibrate(0, Context.SelectedPort);
        }
    }
}

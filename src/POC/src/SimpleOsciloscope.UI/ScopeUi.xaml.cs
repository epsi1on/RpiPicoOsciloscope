using FftSharp;
using NAudio.Mixer;
using SimpleOsciloscope.UI.HardwareInterface;
using SimpleOsciloscope.UI.InterfaceUi;
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
using static SimpleOsciloscope.UI.ScopeUi.ContextClass;
using ChannelInfo = SimpleOsciloscope.UI.HardwareInterface.AdcChannelInfo;


namespace SimpleOsciloscope.UI
{
    /// <summary>
    /// Interaction logic for ScopeUi.xaml
    /// </summary>
    public partial class ScopeUi 
    {

        public ScopeUi()
        {
            InitializeComponent();
            this.DataContext = this.Context = new ContextClass();
            Context.Init();


            Context.SelectedDeviceChanged += OnSelectedDeviceChanged;
        }


        public void OnSelectedDeviceChanged(object sender, PropertyValueChangedEventArgs<OsciloscopeDeviceInfo> e)
        {
            if (e.NewValue == null)
                return;

            var t = e.NewValue;

            var ui = t.UI;

            var cfg = ui.LoadUserSettings();
            var ctrl = ui.GenerateUiInterface(cfg);

            this.itmDeviceConfig.Content = ctrl;

        }

        ContextClass Context;


        public class ContextClass : INotifyPropertyChanged
        {

            internal void Init()
            {
                this.BitmapSource = BitmapFactory.New(UiState.Instance.RenderBitmapHeight, UiState.Instance.RenderBitmapWidth);

                this.ScopeBitmapContext = BitmapSource.GetBitmapContext();

                Renderers = new IScopeRenderer[] {
                    new HarmonicSignalGraphRenderer(),
                    new HitBasedSignalGraphRender(),
                    new FftRender(),
                    new ThdRender()
                };

                renderer = Renderers[0];

                this.RenderType = RenderTypes.Harmonic;


                {
                    var dvcs = Interfaces.GetInterfaces();

                    var lst = new List<OsciloscopeDeviceInfo>();

                    foreach (var iface in dvcs)
                    {
                        var inf = new OsciloscopeDeviceInfo();
                        inf.Name = iface.GetName();
                        inf.Description = iface.GetDescription();
                        inf.UI = iface;
                        lst.Add(inf);
                    }

                    this.AvailableDevices = new ObservableCollection<OsciloscopeDeviceInfo>(lst);
                }

                {
                   
                    this.RenderTypeChanged += (a, b) => updateRenderType();
                }

                {
                    //this.BitmapSource = new WriteableBitmap(UiState.Instance.RenderBitmapWidth, UiState.Instance.RenderBitmapHeight, 96, 96, pixelFormat: UiState.BitmapPixelFormat, null);
                    //this.SampleRate = 500_000;// (long)UiState.AdcConfig.SampleRate;
                }

                {
                    //RefreshPorts();
                    this.IsNotConnected = true;
                }
            }

            public class OsciloscopeDeviceInfo
            {
                public string Name { get; set; }
                public string Description { get; set; }

                public InterfaceUi.BaseDeviceInterface UI;
            }



            #region AvailableDevice Property and field

            [Obfuscation(Exclude = true, ApplyToMembers = false)]
            public ObservableCollection<OsciloscopeDeviceInfo> AvailableDevices
            {
                get { return _AvailableDevices; }
                set
                {
                    if (AreEqualObjects(_AvailableDevices, value))
                        return;

                    var _fieldOldValue = _AvailableDevices;

                    _AvailableDevices = value;

                    ContextClass.OnAvailableDevicesChanged(this, new PropertyValueChangedEventArgs<ObservableCollection<OsciloscopeDeviceInfo>>(_fieldOldValue, value));

                    this.OnPropertyChanged("AvailableDevices");
                }
            }

            private ObservableCollection<OsciloscopeDeviceInfo> _AvailableDevices;

            public EventHandler<PropertyValueChangedEventArgs<ObservableCollection<OsciloscopeDeviceInfo>>> AvailableDevicesChanged;

            public static void OnAvailableDevicesChanged(object sender, PropertyValueChangedEventArgs<ObservableCollection<OsciloscopeDeviceInfo>> e)
            {
                var obj = sender as ContextClass;

                if (obj.AvailableDevicesChanged != null)
                    obj.AvailableDevicesChanged(obj, e);
            }

            #endregion

            #region SelectedDevice Property and field

            [Obfuscation(Exclude = true, ApplyToMembers = false)]
            public OsciloscopeDeviceInfo SelectedDevice
            {
                get { return _SelectedDevice; }
                set
                {
                    if (AreEqualObjects(_SelectedDevice, value))
                        return;

                    var _fieldOldValue = _SelectedDevice;

                    _SelectedDevice = value;

                    ContextClass.OnSelectedDeviceChanged(this, new PropertyValueChangedEventArgs<OsciloscopeDeviceInfo>(_fieldOldValue, value));

                    this.OnPropertyChanged("SelectedDevice");
                }
            }

            private OsciloscopeDeviceInfo _SelectedDevice;

            public EventHandler<PropertyValueChangedEventArgs<OsciloscopeDeviceInfo>> SelectedDeviceChanged;

            public static void OnSelectedDeviceChanged(object sender, PropertyValueChangedEventArgs<OsciloscopeDeviceInfo> e)
            {
                var obj = sender as ContextClass;

                if (obj.SelectedDeviceChanged != null)
                    obj.SelectedDeviceChanged(obj, e);
            }

            #endregion

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

            #region HideForeground Property and field

            //hide foreground controls (for connect to hardware)
            [Obfuscation(Exclude = true, ApplyToMembers = false)]
            public bool HideForeground
            {
                get { return _HideForeground; }
                set
                {
                    if (AreEqualObjects(_HideForeground, value))
                        return;

                    var _fieldOldValue = _HideForeground;

                    _HideForeground = value;

                    ContextClass.OnHideForegroundChanged(this, new PropertyValueChangedEventArgs<bool>(_fieldOldValue, value));

                    this.OnPropertyChanged("HideForeground");
                }
            }

            private bool _HideForeground;

            public EventHandler<PropertyValueChangedEventArgs<bool>> HideForegroundChanged;

            public static void OnHideForegroundChanged(object sender, PropertyValueChangedEventArgs<bool> e)
            {
                var obj = sender as ContextClass;

                if (obj.HideForegroundChanged != null)
                    obj.HideForegroundChanged(obj, e);
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

            

            #region RenderType Property and field

            [Obfuscation(Exclude = true, ApplyToMembers = false)]
            public RenderTypes RenderType
            {
                get { return _RenderType; }
                set
                {
                    if (AreEqualObjects(_RenderType, value))
                        return;

                    var _fieldOldValue = _RenderType;

                    _RenderType = value;

                    ContextClass.OnRenderTypeChanged(this, new PropertyValueChangedEventArgs<RenderTypes>(_fieldOldValue, value));

                    this.OnPropertyChanged("RenderType");
                }
            }

            private RenderTypes _RenderType;

            public EventHandler<PropertyValueChangedEventArgs<RenderTypes>> RenderTypeChanged;

            public static void OnRenderTypeChanged(object sender, PropertyValueChangedEventArgs<RenderTypes> e)
            {
                var obj = sender as ContextClass;

                if (obj.RenderTypeChanged != null)
                    obj.RenderTypeChanged(obj, e);
            }

            #endregion

            #region MousePosValue Property and field

            [Obfuscation(Exclude = true, ApplyToMembers = false)]
            public string MousePosValue
            {
                get { return _MousePosValue; }
                set
                {
                    if (AreEqualObjects(_MousePosValue, value))
                        return;

                    var _fieldOldValue = _MousePosValue;

                    _MousePosValue = value;

                    ContextClass.OnMousePosValueChanged(this, new PropertyValueChangedEventArgs<string>(_fieldOldValue, value));

                    this.OnPropertyChanged("MousePosValue");
                }
            }

            private string _MousePosValue;

            public EventHandler<PropertyValueChangedEventArgs<string>> MousePosValueChanged;

            public static void OnMousePosValueChanged(object sender, PropertyValueChangedEventArgs<string> e)
            {
                var obj = sender as ContextClass;

                if (obj.MousePosValueChanged != null)
                    obj.MousePosValueChanged(obj, e);
            }

            #endregion

            public enum RenderTypes
            {
                Fft,
                Harmonic,
                HitBased,
                Thd
            }

            private void updateRenderType()
            {/*
                if (this.ShowFft)
                    render = new FftRender();

                if (this.ShowHarmonic)
                    render = new HarmonicSignalGraphRenderer();
                */

                var curr = this.renderer;

                //curr.SetEnabled(false);


                switch (this.RenderType)
                {
                    case RenderTypes.Fft:
                        curr = Renderers.FirstOrDefault(i => i is FftRender);
                        break;

                    case RenderTypes.Harmonic:
                        curr = Renderers.FirstOrDefault(i => i is HarmonicSignalGraphRenderer);
                        break;

                    case RenderTypes.HitBased:
                        curr = Renderers.FirstOrDefault(i => i is HitBasedSignalGraphRender);
                        break;

                    case RenderTypes.Thd:
                        curr = Renderers.FirstOrDefault(i => i is ThdRender);
                        break;
                    default:
                        throw new NotImplementedException();
                        break;
                }

                //curr.SetEnabled(true);

                
                this.renderer = curr;
            }

            private IScopeRenderer[] Renderers;

            IScopeRenderer renderer;

            private bool RenderLoopFlag = false;



            public void Start(IDaqInterface intfs,int sampleRate)
            {


                foreach (var item in Renderers)
                {
                    //if (item != null)
                    //    item.Clear();
                }

                /*
                if (this.SelectedChannel == null)
                {
                    MessageBox.Show("Select Channel");
                    return;
                }
                */
                //UiState.Instance.CurrentRepo.clear
                UiState.Instance.CurrentRepo.Init(sampleRate);

                {
                    

                    foreach (var item in Renderers)
                    {
                        item.ReSetZoom();
                    }
                }

                this.IsNotConnected = false;

                {
                    var thr = RenderThread = new Thread(RenderLoopSync);
                    RenderLoopFlag = true;
                    thr.Priority = ThreadPriority.AboveNormal;
                    thr.Start();
                }

                {

                    if (DaqInterface != null)
                    {
                        DaqInterface.StopAdc();
                        DaqInterface.DisConnect();
                    }

                    var ifs = DaqInterface = intfs;// new RpiPicoDaqInterface(this.SelectedPort, SampleRate);
                    //new Stm32Interface(this.SelectedPort, SampleRate);
                    //new ArduinoInterface();
                    //new FakeDaqInterface();

                    //ifs.Channel = this.SelectedChannel;

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


                    var thr = DaqThread = new Thread(() =>
                    {
                        try
                        {
                            ifs.StartSync();
                        }
                        catch (Exception ex)
                        {

                            Stop();


                            if (!(ex is ThreadAbortException))
                                Application.Current.Dispatcher.Invoke(() => { MessageBox.Show(ex.Message); });


                        }
                    });

                    thr.Start();

                }

                //UiState.Instance.CurrentRepo.AdcSampleRate = (int)this.SampleRate;
                UiState.AdcConfig.SampleRate = sampleRate;


            }

            public void Stop()
            {
                if (this.IsNotConnected)
                    return;

                this.IsNotConnected = true;

                if (RenderThread != null)
                {
                    RenderLoopFlag = false;


                    if (RenderThread.IsAlive)
                        RenderThread.Abort();
                }

                if (DaqInterface != null)
                {
                    DaqInterface.StopAdc();

                    if (DaqThread.IsAlive)
                        DaqThread.Abort();

                    DaqInterface.DisConnect();
                }

                foreach (var rnd in Renderers)
                    rnd.Clear(this.ScopeBitmapContext);

            }


            
            void RenderLoopSync()
            {
                var fps = UiState.RenderFramerate;

                var rateFrameMs = (1 / fps) * 1000;//how many milis between two consecutive frames


                var tm = Stopwatch.StartNew();


                while (true)
                {
                    if (!RenderLoopFlag)
                        return;
                    
                    tm.Restart();
                    RenderShot();
                    tm.Stop();

                    var wait = rateFrameMs - tm.ElapsedMilliseconds;

                    if (wait > 0)
                        Thread.Sleep((int)wait);
                }
            }

            private Thread RenderThread;
            private Thread DaqThread;
            private IDaqInterface DaqInterface;
            public BitmapContext ScopeBitmapContext;

            void RenderShot()
            {
                var sp = System.Diagnostics.Stopwatch.StartNew();

                double freq;

                double min, max;

                var prps = SignalPropertyCalculator.Calculate();

                {
                    this.Frequency = prps.Frequency;

                    //this.SignalInfo = prps;
                }
                
                renderer.DoRender(ScopeBitmapContext, prps);

                {
                    var w = this.ScopeBitmapContext.Width;
                    var h = this.ScopeBitmapContext.Height;

                    this.BitmapSource.AddDirtyRectThreadSafe(new Int32Rect(0, 0, w, h));

                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        this.BitmapSource.Lock();
                        this.BitmapSource.AddDirtyRect(new Int32Rect(0, 0, w, h));
                        this.BitmapSource.Unlock();
                    }), System.Windows.Threading.DispatcherPriority.Render);
                }

                sp.Stop();

                Log.Info("Render took {0} ms", sp.ElapsedMilliseconds);

                prps.Dispose();
            }



            public void OnMouseWheelZoom(Point center, double delta)
            {
                var sens = 0.001;


                this.renderer.Zoom(delta * sens, (int)center.X, (int)center.Y);
            }

            public void OnMouseMoveStatus(Point center)
            {
                var sens = 0.001;

                var val = this.renderer.GetPointerValue(center.X, center.Y);

                this.MousePosValue = val;
            }

            public void ResetZoom()
            {
                this.renderer.ReSetZoom();
            }
        }


        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            Context.Stop();
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            /*
            return;
            /** /
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
            /** /


            Settings.Default.lastChannelIndex = Context.AvailableChannels.IndexOf(Context.SelectedChannel);
            Settings.Default.lastSampleRate = Context.SampleRate;
            Settings.Default.lastPort = Context.SelectedPort;
            Settings.Default.Save();


            
            Context.Start(null);

            */
        }

        private void BtnRefreshPorts_Click(object sender, RoutedEventArgs e)
        {
            //Context.RefreshPorts();
        }


        private void BtnCalib_Click(object sender, RoutedEventArgs e)
        {
            //Calibration.Calibrate(Context.SelectedPort);
        }
        

        private void btnFftClick_Click(object sender, RoutedEventArgs e)
        {
            Context.RenderType = ContextClass.RenderTypes.Fft;
        }

        private void btnHarmonicClick_Click(object sender, RoutedEventArgs e)
        {
            Context.RenderType = ContextClass.RenderTypes.Harmonic;
        }

        private void btnHitBasedClick_Click(object sender, RoutedEventArgs e)
        {
            Context.RenderType = ContextClass.RenderTypes.HitBased;
        }


        private void btnThdBasedClick_Click(object sender, RoutedEventArgs e)
        {
            Context.RenderType = ContextClass.RenderTypes.Thd;
        }

        private void ImgCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            var pt = e.GetPosition(sender as IInputElement);

            var img = sender as Image;

            var w = img.ActualWidth;
            var h = img.ActualHeight;

            var wp = (img.Source as BitmapSource).PixelWidth;
            var hp = (img.Source as BitmapSource).PixelHeight;

            var x = pt.X / w * wp;
            var y = pt.Y / h * hp;

            Context.OnMouseWheelZoom(new Point(x, y), -e.Delta);
        }

        private void ImgCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle)
                Context.ResetZoom();
        }

        private void ImgCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            var pt = e.GetPosition(sender as IInputElement);

            var img = sender as Image;

            var w = img.ActualWidth;
            var h = img.ActualHeight;

            var wp = (img.Source as BitmapSource).PixelWidth;
            var hp = (img.Source as BitmapSource).PixelHeight;

            var x = pt.X / w * wp;
            var y = pt.Y / h * hp;

            Context.OnMouseMoveStatus(new Point(x, y));
        }

        private void BtnConnect_Click(object sender, RoutedEventArgs e)
        {
            if (Context.SelectedDevice == null)
            {
                ShoeMessageToUser("Select Device");
                return;
            }

            var ctrl = itmDeviceConfig.Content as BaseDaqConfigGUIControl;

            if (ctrl == null)
                return;

            if (!ctrl.IsValidConfig())
            {
                ShoeMessageToUser("Invalid/incomplete config");
                return;
            }


            var ui = Context.SelectedDevice.UI;

            var config = ctrl.GetUserSettings();

            ui.SaveUserSettings(config);

            var calib = ui.LoadCalibrationSettings();

            var daqInterface = Context.SelectedDevice.UI.GenerateDaqInterface(calib, config);
            var sr = UiState.AdcConfig.SampleRate = config.GetAdcSampleRate();

            Context.Start(daqInterface, (int)sr);

            return;
        }

        private void ShoeMessageToUser(string message)
        {
            MessageBox.Show(message);
        }
    }
}

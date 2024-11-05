using SimpleOsciloscope.UI.HardwareInterface;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static SimpleOsciloscope.UI.HardwareInterface.Rp2DaqInterface;
using ChannelInfo = SimpleOsciloscope.UI.HardwareInterface.AdcChannelInfo;


namespace SimpleOsciloscope.UI.InterfaceUi
{
    /// <summary>
    /// Interaction logic for Rp2DaqInterfaceControl.xaml
    /// </summary>
    public partial class Rp2DaqInterfaceControl : UserControl, BaseDaqConfigGUIControl
    {

        ContextClass Context;

        public class ContextClass : INotifyPropertyChanged
        {
            public class Rp2DaqChannelInfo
            {
                public Rp2DaqChannelInfo(Rp2DaqInterface.Rp2040AdcChannels rpChannel, int pin10x, int pinAcDc, int pinAdc)
                {
                    //Id = id;
                    Pin10x = pin10x;
                    PinAcDc = pinAcDc;
                    PinAdc = pinAdc;
                    //NormalAlpha = normalAlpha;
                    //NormalBeta = normalBeta;
                    //this._10xAlpha = _10xAlpha;
                    //this._10xBeta = _10xBeta;
                    ChannelId = rpChannel;
                }

                public readonly int Pin10x = -1;//gpio# for 10x button
                public readonly int PinAcDc = -1;//ac coupling cap button
                public readonly int PinAdc = -1;//gpio# for adc

                public readonly Rp2DaqInterface.Rp2040AdcChannels ChannelId;


                //public double NormalPullupResistor = double.MaxValue;
                //public double NormalPulldownResistor = double.MaxValue;

                //public double _10xPullupResistor = double.MaxValue;
                //public double _10xPulldownResistor = double.MaxValue;
                /*
                public readonly double NormalAlpha = double.NaN;
                public readonly double NormalBeta = double.NaN;

                public readonly double _10xAlpha = double.NaN;
                public readonly double _10xBeta = double.NaN;
                */
                public string Title { get; set; }
                public string Description { get; set; }


                public bool AcDcMode { get; set; }// ac/dc button is pressed
                public bool _10xMode { get; set; }// 10x button is active
            }

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


            #region AvailableChannels Property and field

            [Obfuscation(Exclude = true, ApplyToMembers = false)]
            public ObservableCollection<Rp2DaqChannelInfo> AvailableChannels
            {
                get { return _AvailableChannels; }
                set
                {
                    if (AreEqualObjects(_AvailableChannels, value))
                        return;

                    var _fieldOldValue = _AvailableChannels;
                    _AvailableChannels = value;

                    ContextClass.OnAvailableChannelsChanged(this, new PropertyValueChangedEventArgs<ObservableCollection<Rp2DaqChannelInfo>>(_fieldOldValue, value));

                    this.OnPropertyChanged("AvailableChannels");
                }
            }

            private ObservableCollection<Rp2DaqChannelInfo> _AvailableChannels;

            public EventHandler<PropertyValueChangedEventArgs<ObservableCollection<Rp2DaqChannelInfo>>> AvailableChannelsChanged;

            public static void OnAvailableChannelsChanged(object sender, PropertyValueChangedEventArgs<ObservableCollection<Rp2DaqChannelInfo>> e)
            {
                var obj = sender as ContextClass;

                if (obj.AvailableChannelsChanged != null)
                    obj.AvailableChannelsChanged(obj, e);
            }

            #endregion

            #region SelectedChannel Property and field

            [Obfuscation(Exclude = true, ApplyToMembers = false)]
            public Rp2DaqChannelInfo SelectedChannel
            {
                get { return _SelectedChannel; }
                set
                {
                    if (AreEqualObjects(_SelectedChannel, value))
                        return;

                    var _fieldOldValue = _SelectedChannel;
                    _SelectedChannel = value;

                    ContextClass.OnSelectedChannelChanged(this, new PropertyValueChangedEventArgs<Rp2DaqChannelInfo>(_fieldOldValue, value));

                    this.OnPropertyChanged("SelectedChannel");
                }
            }

            private Rp2DaqChannelInfo _SelectedChannel;

            public EventHandler<PropertyValueChangedEventArgs<Rp2DaqChannelInfo>> SelectedChannelChanged;

            public static void OnSelectedChannelChanged(object sender, PropertyValueChangedEventArgs<Rp2DaqChannelInfo> e)
            {
                var obj = sender as ContextClass;

                if (obj.SelectedChannelChanged != null)
                    obj.SelectedChannelChanged(obj, e);
            }

            #endregion

            #region BitWidth Property and field

            [Obfuscation(Exclude = true, ApplyToMembers = false)]
            public int BitWidth
            {
                get { return _BitWidth; }
                set
                {
                    if (AreEqualObjects(_BitWidth, value))
                        return;

                    var _fieldOldValue = _BitWidth;

                    _BitWidth = value;

                    ContextClass.OnBitWidthChanged(this, new PropertyValueChangedEventArgs<int>(_fieldOldValue, value));

                    this.OnPropertyChanged("BitWidth");
                }
            }

            private int _BitWidth;

            public EventHandler<PropertyValueChangedEventArgs<int>> BitWidthChanged;

            public static void OnBitWidthChanged(object sender, PropertyValueChangedEventArgs<int> e)
            {
                var obj = sender as ContextClass;

                if (obj.BitWidthChanged != null)
                    obj.BitWidthChanged(obj, e);
            }

            #endregion


            public static Rp2DaqChannelInfo[] InitChannels()
            {
                var lst = new List<Rp2DaqChannelInfo>();

                var ids = new Rp2DaqInterface.Rp2040AdcChannels[]{
                    Rp2DaqInterface.Rp2040AdcChannels.Gpio28,
                    Rp2DaqInterface.Rp2040AdcChannels.Gpio26,
                    Rp2DaqInterface.Rp2040AdcChannels.Gpio27,
                    //RpiPicoDaqInterface.Rp2040AdcChannels.InternalReference,
                    //RpiPicoDaqInterface.Rp2040AdcChannels.InternalTempratureSensor
                    };


                var SwPins = new int[] { 19, 21, 18 };
                var AcDcPins = new int[] { 20, -1, -1 };
                var adcPins = new byte[] { 28, 26, 27 };

                var pins = adcPins;

                //var g26 = pins.FindFirstIndexOf(i => i == 26);
                //var g27 = pins.FindFirstIndexOf(i => i == 27);
                //var g28 = pins.FindFirstIndexOf(i => i == 28);

                var titles = new string[] {
                        "1",
                        "2",
                        "3",
                        "VRef",
                        "Tmpr"
                    };

                var descs = new string[] {
                        "ADC Probe",
                        "ADC Probe",
                        "ADC Probe",
                        "Internal Voltage Reference",
                        "Internal Temprature Sensor" };

                for (var i = 0; i < 3; i++)
                {
                    var ch = new Rp2DaqChannelInfo(ids[i], SwPins[i], AcDcPins[i], adcPins[i]);

                    /*
                    ch.RpChannel = ids[i];
                    
                        InitChannel();
                    */

                    ch.Title = titles[i];
                    ch.Description = descs[i];

                    lst.Add(ch);
                }


                var tmp = lst.ToArray();//.OrderBy(i => i.Title).ToArray();

                return tmp;
            }


            public void Init()
            {
                RefreshPorts();

                this.AvailableChannels = new ObservableCollection<Rp2DaqChannelInfo>(InitChannels());
            }

            internal void RefreshPorts()
            {
                this.AvailablePorts = new ObservableCollection<string>(SerialPort.GetPortNames());
                this.SelectedPort = this.AvailablePorts.FirstOrDefault();
            }
            
            public Rp2daqUserSettings GetCurrentUserSettings()
            {
                var cfg = new Rp2daqUserSettings();

                cfg.SampleRate = (int)this.SampleRate;
                cfg.ComPortName = this.SelectedPort;
                cfg.ChannelId = this.SelectedChannel.ChannelId;
                cfg.BitWidth = this.BitWidth;
                return (cfg);
            }

            public void SetCurrentUserSettings(Rp2daqUserSettings data)
            {
                var cfg = (data);

                this.SampleRate = cfg.SampleRate;
                this.SelectedPort = cfg.ComPortName;

                this.SelectedChannel = AvailableChannels.FirstOrDefault(ii => ii.ChannelId == cfg.ChannelId);

                this.BitWidth= cfg.BitWidth;
            }
        }

        public Rp2DaqInterfaceControl()
        {
            InitializeComponent();

            this.DataContext = this.Context = new ContextClass();

            
        }

        public BaseDeviceUserSettingsData GetUserSettings()
        {
            return Context.GetCurrentUserSettings();
        }

        public void SetDefaultUserSettings( BaseDeviceUserSettingsData config)
        {
            Context.SetCurrentUserSettings(config as Rp2daqUserSettings);
        }

        public bool IsValidConfig()
        {
            if (Context.SampleRate <= 0) return false;

            if (Context.SampleRate > 500_000) return false;


            if (Context.SelectedChannel == null) return false;

            if (Context.SelectedPort == null) return false;

            var availableBitWidths = new int[] { 12 };

            if (!availableBitWidths.Contains( Context.BitWidth )) return false;

            return true;
        }

        private void BtnRefreshPorts_Click(object sender, RoutedEventArgs e)
        {
            Context.RefreshPorts();
        }

        public void Init()
        {
            Context.Init();
        }
    }
}

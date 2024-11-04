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
using static SimpleOsciloscope.UI.HardwareInterface.RpiPicoDaqInterface;
using ChannelInfo = SimpleOsciloscope.UI.HardwareInterface.AdcChannelInfo;


namespace SimpleOsciloscope.UI.InterfaceUi
{
    /// <summary>
    /// Interaction logic for Rp2DaqInterfaceControl.xaml
    /// </summary>
    public partial class Rp2DaqInterfaceControl : UserControl, BaseDaqConfigControl
    {

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


            public static AdcChannelInfo[] InitChannels()
            {
                var lst = new List<AdcChannelInfo>();

                var ids = new RpiPicoDaqInterface.Rp2040AdcChannels[]{
                    RpiPicoDaqInterface.Rp2040AdcChannels.Gpio28,
                    RpiPicoDaqInterface.Rp2040AdcChannels.Gpio26,
                    RpiPicoDaqInterface.Rp2040AdcChannels.Gpio27,
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
                        "VRef ",
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
                    var ch = InitChannel(i, ids[i], SwPins[i], AcDcPins[i]);

                    ch.Title = titles[i];
                    ch.Description = descs[i];

                    lst.Add(ch);
                }


                var tmp = lst.ToArray();//.OrderBy(i => i.Title).ToArray();

                return tmp;
            }

            private static AdcChannelInfo InitChannel(int index, Rp2040AdcChannels chn, int swPin, int acdcPin)
            {

                //var AdcPins = new int[] { 26, 27, 28 };
                //var SwPins = new int[] { 19, 21, 18 };
                //var AcDcPins = new int[] { 20, -1, -1 };

                double normalAlpha, normalBeta;
                double _10xAlpha, _10xBeta;

                normalAlpha = double.Parse(ConfigurationManager.AppSettings["ch" + (index) + "_alpha_off"]);
                normalBeta = double.Parse(ConfigurationManager.AppSettings["ch" + (index) + "_beta_off"]);

                _10xAlpha = double.Parse(ConfigurationManager.AppSettings["ch" + (index) + "_alpha_on"]);
                _10xBeta = double.Parse(ConfigurationManager.AppSettings["ch" + (index) + "_beta_on"]);

                //var pns = AdcPins();
                //var adcPin = pns[chnId];
                //var swPin = SwPins[chnId];
                //var acdcPin = AcDcPins[chnId];

                var ch1 = new AdcChannelInfo(index, swPin, acdcPin,
                    normalAlpha, normalBeta,
                    _10xAlpha, _10xBeta, chn);

                return ch1;
            }

            public void Init()
            {
                RefreshPorts();

                this.AvailableChannels = new ObservableCollection<ChannelInfo>(InitChannels());
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
                cfg.ChannelId = this.SelectedChannel.Id;

                return (cfg);
            }

            public void SetCurrentUserSettings(Rp2daqUserSettings data)
            {
                var cfg = (data);

                this.SampleRate = cfg.SampleRate;
                this.SelectedPort = cfg.ComPortName;

                this.SelectedChannel = AvailableChannels.FirstOrDefault(ii => ii.Id == cfg.ChannelId);
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

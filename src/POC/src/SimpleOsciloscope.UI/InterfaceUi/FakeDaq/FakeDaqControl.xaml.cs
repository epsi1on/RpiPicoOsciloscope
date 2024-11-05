using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
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

namespace SimpleOsciloscope.UI.InterfaceUi.FakeDaq
{
    /// <summary>
    /// Interaction logic for FakeDaqControl.xaml
    /// </summary>
    public partial class FakeDaqControl : UserControl, BaseDaqConfigGUIControl
    {
        public FakeDaqControl()
        {
            InitializeComponent();
            this.DataContext = this.Context = new ContextClass();
        }

        ContextClass Context;

        public class ContextClass:INotifyPropertyChanged
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
            public int SampleRate
            {
                get { return _SampleRate; }
                set
                {
                    if (AreEqualObjects(_SampleRate, value))
                        return;

                    var _fieldOldValue = _SampleRate;

                    _SampleRate = value;

                    ContextClass.OnSampleRateChanged(this, new PropertyValueChangedEventArgs<int>(_fieldOldValue, value));

                    this.OnPropertyChanged("SampleRate");
                }
            }

            private int _SampleRate;

            public EventHandler<PropertyValueChangedEventArgs<int>> SampleRateChanged;

            public static void OnSampleRateChanged(object sender, PropertyValueChangedEventArgs<int> e)
            {
                var obj = sender as ContextClass;

                if (obj.SampleRateChanged != null)
                    obj.SampleRateChanged(obj, e);
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

            #region Offset Property and field

            [Obfuscation(Exclude = true, ApplyToMembers = false)]
            public double Offset
            {
                get { return _Offset; }
                set
                {
                    if (AreEqualObjects(_Offset, value))
                        return;

                    var _fieldOldValue = _Offset;

                    _Offset = value;

                    ContextClass.OnOffsetChanged(this, new PropertyValueChangedEventArgs<double>(_fieldOldValue, value));

                    this.OnPropertyChanged("Offset");
                }
            }

            private double _Offset;

            public EventHandler<PropertyValueChangedEventArgs<double>> OffsetChanged;

            public static void OnOffsetChanged(object sender, PropertyValueChangedEventArgs<double> e)
            {
                var obj = sender as ContextClass;

                if (obj.OffsetChanged != null)
                    obj.OffsetChanged(obj, e);
            }

            #endregion

            #region Amplitude Property and field

            [Obfuscation(Exclude = true, ApplyToMembers = false)]
            public double Amplitude
            {
                get { return _Amplitude; }
                set
                {
                    if (AreEqualObjects(_Amplitude, value))
                        return;

                    var _fieldOldValue = _Amplitude;

                    _Amplitude = value;

                    ContextClass.OnAmplitudeChanged(this, new PropertyValueChangedEventArgs<double>(_fieldOldValue, value));

                    this.OnPropertyChanged("Amplitude");
                }
            }

            private double _Amplitude;

            public EventHandler<PropertyValueChangedEventArgs<double>> AmplitudeChanged;

            public static void OnAmplitudeChanged(object sender, PropertyValueChangedEventArgs<double> e)
            {
                var obj = sender as ContextClass;

                if (obj.AmplitudeChanged != null)
                    obj.AmplitudeChanged(obj, e);
            }

            #endregion

        }

        public BaseDeviceUserSettingsData GetUserSettings()
        {
            var buf = new FakeDaqUserSettings();
            buf.SampleRate = Context.SampleRate;
            buf.Frequency = Context.Frequency;
            buf.Offset = Context.Offset;
            buf.Amplitude = Context.Amplitude;

            return buf;
        }

        public void Init()
        {
            
        }

        public bool IsValidConfig()
        {
            return true;
        }

        public void SetDefaultUserSettings(BaseDeviceUserSettingsData config)
        {
            var c = (config as FakeDaqUserSettings);
            this.Context.SampleRate = c.SampleRate;
            this.Context.Offset = c.Offset;
            this.Context.Frequency = c.Frequency;
            this.Context.Amplitude = c.Amplitude;

        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
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



namespace SimpleOsciloscope.UI
{

    /// <summary>
    /// Interaction logic for SignalPropertiesVisualizer.xaml
    /// </summary>
    public partial class SignalPropertiesVisualizer : UserControl
    {
        public SignalPropertiesVisualizer()
        {
            InitializeComponent();
            this.mainGrid.DataContext = this.Context = new ParentClass();
        }


        ParentClass Context;

        public SignalPropertyList SignalInfo
        {
            get { return (SignalPropertyList)GetValue(SignalInfoProperty); }
            set { 
                SetValue(SignalInfoProperty, value);
                Context.Update(value);
            
            }
        }

        // Using a DependencyProperty as the backing store for SignalInfo.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SignalInfoProperty =
            DependencyProperty.Register("SignalInfo", typeof(SignalPropertyList), typeof(SignalPropertiesVisualizer), new PropertyMetadata(null, OnSignalInfoChangedCallback));


        public static void OnSignalInfoChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var val = e.NewValue as SignalPropertyList;

            (d as SignalPropertiesVisualizer).Context.Update(val);
        }

        public class ParentClass : INotifyPropertyChanged
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


            public void Update(SignalPropertyList prp)
            {
                var alpha = prp.alpha;
                var beta = prp.beta;

                this.AbsMax = AdcToVolt(prp.Max, alpha, beta);
                this.AbsMin = AdcToVolt(prp.Min, alpha, beta);
                this.AbsDomain = DeltaAdcToVolt(Math.Abs(prp.Max - prp.Min), alpha, beta);

                this.Prc1Max = AdcToVolt(prp.MaxPercentile1, alpha, beta);
                this.Prc1Min = AdcToVolt(prp.MinPercentile1, alpha, beta);

                this.Prc1Domain = DeltaAdcToVolt(Math.Abs(prp.MaxPercentile1 - prp.MinPercentile1), alpha, beta);

                this.Frequency = FrequencyToString(prp.Frequency);
                this.PwmDutyCycle = string.Format("{0:0.00} %",prp.PwmDutyCycle*100);

                //minimum possible measurement, due to alpha beta (related to adc 0 value)
                this.MinMeasurable = AdcToVolt(0, alpha, beta);

                //maximum possible measurement, due to alpha beta (related to adc 4095 value)
                this.MaxMeasurable = AdcToVolt(4096, alpha, beta);


            }

            private string AdcToVolt(int adc,double alpha, double beta)
            {
                var volt = alpha * adc + beta;
                return VoltToString(volt);
            }

            private string DeltaAdcToVolt(int deltaAdc, double alpha, double beta)
            {
                var volt = alpha * deltaAdc;
                return VoltToString(volt);
            }

            private string PercentToString(double prc)
            {
                return string.Format("{0:0.000} %", prc * 100);
            }

            private string FrequencyToString(double freq)
            {
                if (freq > 1e9)
                    return string.Format("{0:0.000} GHz", freq / 1e9);

                if (freq > 1e6)
                    return string.Format("{0:0.000} MHz", freq / 1e6);

                if (freq > 1e3)
                    return string.Format("{0:0.000} KHz", freq / 1e3);

                return string.Format("{0:0} Hz", freq );
            }

            private string VoltToString(double volt)
            {
                if (Math.Abs(volt) > 10)
                {
                    return string.Format("{0:0.0} V", volt);
                }
                else if (Math.Abs(volt) > 1)
                {
                    return string.Format("{0:0.000} V", volt);
                }
                else
                {
                    return string.Format("{0:0} mV", volt * 1000);
                }
            }

            #region AbsMax Property and field

            [Obfuscation(Exclude = true, ApplyToMembers = false)]
            public string AbsMax
            {
                get { return _AbsMax; }
                set
                {
                    if (AreEqualObjects(_AbsMax, value))
                        return;

                    var _fieldOldValue = _AbsMax;

                    _AbsMax = value;

                    ParentClass.OnAbsMaxChanged(this, new PropertyValueChangedEventArgs<string>(_fieldOldValue, value));

                    this.OnPropertyChanged("AbsMax");
                }
            }

            private string _AbsMax;

            public EventHandler<PropertyValueChangedEventArgs<string>> AbsMaxChanged;

            public static void OnAbsMaxChanged(object sender, PropertyValueChangedEventArgs<string> e)
            {
                var obj = sender as ParentClass;

                if (obj.AbsMaxChanged != null)
                    obj.AbsMaxChanged(obj, e);
            }

            #endregion

            #region AbsMin Property and field

            [Obfuscation(Exclude = true, ApplyToMembers = false)]
            public string AbsMin
            {
                get { return _AbsMin; }
                set
                {
                    if (AreEqualObjects(_AbsMin, value))
                        return;

                    var _fieldOldValue = _AbsMin;

                    _AbsMin = value;

                    ParentClass.OnAbsMinChanged(this, new PropertyValueChangedEventArgs<string>(_fieldOldValue, value));

                    this.OnPropertyChanged("AbsMin");
                }
            }

            private string _AbsMin;

            public EventHandler<PropertyValueChangedEventArgs<string>> AbsMinChanged;

            public static void OnAbsMinChanged(object sender, PropertyValueChangedEventArgs<string> e)
            {
                var obj = sender as ParentClass;

                if (obj.AbsMinChanged != null)
                    obj.AbsMinChanged(obj, e);
            }

            #endregion

            #region AbsDomain Property and field

            [Obfuscation(Exclude = true, ApplyToMembers = false)]
            public string AbsDomain
            {
                get { return _AbsDomain; }
                set
                {
                    if (AreEqualObjects(_AbsDomain, value))
                        return;

                    var _fieldOldValue = _AbsDomain;

                    _AbsDomain = value;

                    ParentClass.OnAbsDomainChanged(this, new PropertyValueChangedEventArgs<string>(_fieldOldValue, value));

                    this.OnPropertyChanged("AbsDomain");
                }
            }

            private string _AbsDomain;

            public EventHandler<PropertyValueChangedEventArgs<string>> AbsDomainChanged;

            public static void OnAbsDomainChanged(object sender, PropertyValueChangedEventArgs<string> e)
            {
                var obj = sender as ParentClass;

                if (obj.AbsDomainChanged != null)
                    obj.AbsDomainChanged(obj, e);
            }

            #endregion



            #region Prc1Max Property and field

            [Obfuscation(Exclude = true, ApplyToMembers = false)]
            public string Prc1Max
            {
                get { return _Prc1Max; }
                set
                {
                    if (AreEqualObjects(_Prc1Max, value))
                        return;

                    var _fieldOldValue = _Prc1Max;

                    _Prc1Max = value;

                    ParentClass.OnPrc1MaxChanged(this, new PropertyValueChangedEventArgs<string>(_fieldOldValue, value));

                    this.OnPropertyChanged("Prc1Max");
                }
            }

            private string _Prc1Max;

            public EventHandler<PropertyValueChangedEventArgs<string>> Prc1MaxChanged;

            public static void OnPrc1MaxChanged(object sender, PropertyValueChangedEventArgs<string> e)
            {
                var obj = sender as ParentClass;

                if (obj.Prc1MaxChanged != null)
                    obj.Prc1MaxChanged(obj, e);
            }

            #endregion

            #region Prc1Min Property and field

            [Obfuscation(Exclude = true, ApplyToMembers = false)]
            public string Prc1Min
            {
                get { return _Prc1Min; }
                set
                {
                    if (AreEqualObjects(_Prc1Min, value))
                        return;

                    var _fieldOldValue = _Prc1Min;

                    _Prc1Min = value;

                    ParentClass.OnPrc1MinChanged(this, new PropertyValueChangedEventArgs<string>(_fieldOldValue, value));

                    this.OnPropertyChanged("Prc1Min");
                }
            }

            private string _Prc1Min;

            public EventHandler<PropertyValueChangedEventArgs<string>> Prc1MinChanged;

            public static void OnPrc1MinChanged(object sender, PropertyValueChangedEventArgs<string> e)
            {
                var obj = sender as ParentClass;

                if (obj.Prc1MinChanged != null)
                    obj.Prc1MinChanged(obj, e);
            }

            #endregion

            #region Prc1Domain Property and field

            [Obfuscation(Exclude = true, ApplyToMembers = false)]
            public string Prc1Domain
            {
                get { return _Prc1Domain; }
                set
                {
                    if (AreEqualObjects(_Prc1Domain, value))
                        return;

                    var _fieldOldValue = _Prc1Domain;

                    _Prc1Domain = value;

                    ParentClass.OnPrc1DomainChanged(this, new PropertyValueChangedEventArgs<string>(_fieldOldValue, value));

                    this.OnPropertyChanged("Prc1Domain");
                }
            }

            private string _Prc1Domain;

            public EventHandler<PropertyValueChangedEventArgs<string>> Prc1DomainChanged;

            public static void OnPrc1DomainChanged(object sender, PropertyValueChangedEventArgs<string> e)
            {
                var obj = sender as ParentClass;

                if (obj.Prc1DomainChanged != null)
                    obj.Prc1DomainChanged(obj, e);
            }

            #endregion


            #region Frequency Property and field

            [Obfuscation(Exclude = true, ApplyToMembers = false)]
            public string Frequency
            {
                get { return _Frequency; }
                set
                {
                    if (AreEqualObjects(_Frequency, value))
                        return;

                    var _fieldOldValue = _Frequency;

                    _Frequency = value;

                    ParentClass.OnFrequencyChanged(this, new PropertyValueChangedEventArgs<string>(_fieldOldValue, value));

                    this.OnPropertyChanged("Frequency");
                }
            }

            private string _Frequency;

            public EventHandler<PropertyValueChangedEventArgs<string>> FrequencyChanged;

            public static void OnFrequencyChanged(object sender, PropertyValueChangedEventArgs<string> e)
            {
                var obj = sender as ParentClass;

                if (obj.FrequencyChanged != null)
                    obj.FrequencyChanged(obj, e);
            }

            #endregion

            #region PwmDutyCycle Property and field

            [Obfuscation(Exclude = true, ApplyToMembers = false)]
            public string PwmDutyCycle
            {
                get { return _PwmDutyCycle; }
                set
                {
                    if (AreEqualObjects(_PwmDutyCycle, value))
                        return;

                    var _fieldOldValue = _PwmDutyCycle;

                    _PwmDutyCycle = value;

                    ParentClass.OnPwmDutyCycleChanged(this, new PropertyValueChangedEventArgs<string>(_fieldOldValue, value));

                    this.OnPropertyChanged("PwmDutyCycle");
                }
            }

            private string _PwmDutyCycle;

            public EventHandler<PropertyValueChangedEventArgs<string>> PwmDutyCycleChanged;

            public static void OnPwmDutyCycleChanged(object sender, PropertyValueChangedEventArgs<string> e)
            {
                var obj = sender as ParentClass;

                if (obj.PwmDutyCycleChanged != null)
                    obj.PwmDutyCycleChanged(obj, e);
            }

            #endregion


            #region MinMeasurable Property and field

            [Obfuscation(Exclude = true, ApplyToMembers = false)]
            public string MinMeasurable
            {
                get { return _MinMeasurable; }
                set
                {
                    if (AreEqualObjects(_MinMeasurable, value))
                        return;

                    var _fieldOldValue = _MinMeasurable;

                    _MinMeasurable = value;

                    ParentClass.OnMinMeasurableChanged(this, new PropertyValueChangedEventArgs<string>(_fieldOldValue, value));

                    this.OnPropertyChanged("MinMeasurable");
                }
            }

            private string _MinMeasurable;

            public EventHandler<PropertyValueChangedEventArgs<string>> MinMeasurableChanged;

            public static void OnMinMeasurableChanged(object sender, PropertyValueChangedEventArgs<string> e)
            {
                var obj = sender as ParentClass;

                if (obj.MinMeasurableChanged != null)
                    obj.MinMeasurableChanged(obj, e);
            }

            #endregion

            #region MaxMeasurable Property and field

            [Obfuscation(Exclude = true, ApplyToMembers = false)]
            public string MaxMeasurable
            {
                get { return _MaxMeasurable; }
                set
                {
                    if (AreEqualObjects(_MaxMeasurable, value))
                        return;

                    var _fieldOldValue = _MaxMeasurable;

                    _MaxMeasurable = value;

                    ParentClass.OnMaxMeasurableChanged(this, new PropertyValueChangedEventArgs<string>(_fieldOldValue, value));

                    this.OnPropertyChanged("MaxMeasurable");
                }
            }

            private string _MaxMeasurable;

            public EventHandler<PropertyValueChangedEventArgs<string>> MaxMeasurableChanged;

            public static void OnMaxMeasurableChanged(object sender, PropertyValueChangedEventArgs<string> e)
            {
                var obj = sender as ParentClass;

                if (obj.MaxMeasurableChanged != null)
                    obj.MaxMeasurableChanged(obj, e);
            }

            #endregion

        }
    }

    public class AdcToVoltConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

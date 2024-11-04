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
using ParentClass = SignalGenerator.MainWindow.DataContextClass;



namespace SignalGenerator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
			this.DataContext = this.Context = new ParentClass();
        }


		public DataContextClass Context;


		public class DataContextClass : INotifyPropertyChanged
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

					ParentClass.OnFrequencyChanged(this, new PropertyValueChangedEventArgs<double>(_fieldOldValue, value));

					this.OnPropertyChanged("Frequency");
				}
			}

			private double _Frequency;

			public EventHandler<PropertyValueChangedEventArgs<double>> FrequencyChanged;

			public static void OnFrequencyChanged(object sender, PropertyValueChangedEventArgs<double> e)
			{
				var obj = sender as ParentClass;

				if (obj.FrequencyChanged != null)
					obj.FrequencyChanged(obj, e);
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

					ParentClass.OnSampleRateChanged(this, new PropertyValueChangedEventArgs<int>(_fieldOldValue, value));

					this.OnPropertyChanged("SampleRate");
				}
			}

			private int _SampleRate;

			public EventHandler<PropertyValueChangedEventArgs<int>> SampleRateChanged;

			public static void OnSampleRateChanged(object sender, PropertyValueChangedEventArgs<int> e)
			{
				var obj = sender as ParentClass;

				if (obj.SampleRateChanged != null)
					obj.SampleRateChanged(obj, e);
			}

			#endregion

			#region Min Property and field

			[Obfuscation(Exclude = true, ApplyToMembers = false)]
			public short Min
			{
				get { return _Min; }
				set
				{
					if (AreEqualObjects(_Min, value))
						return;

					var _fieldOldValue = _Min;

					_Min = value;

					ParentClass.OnMinChanged(this, new PropertyValueChangedEventArgs<short>(_fieldOldValue, value));

					this.OnPropertyChanged("Min");
				}
			}

			private short _Min;

			public EventHandler<PropertyValueChangedEventArgs<short>> MinChanged;

			public static void OnMinChanged(object sender, PropertyValueChangedEventArgs<short> e)
			{
				var obj = sender as ParentClass;

				if (obj.MinChanged != null)
					obj.MinChanged(obj, e);
			}

			#endregion

			#region Max Property and field

			[Obfuscation(Exclude = true, ApplyToMembers = false)]
			public short Max
			{
				get { return _Max; }
				set
				{
					if (AreEqualObjects(_Max, value))
						return;

					var _fieldOldValue = _Max;

					_Max = value;

					ParentClass.OnMaxChanged(this, new PropertyValueChangedEventArgs<short>(_fieldOldValue, value));

					this.OnPropertyChanged("Max");
				}
			}

			private short _Max;

			public EventHandler<PropertyValueChangedEventArgs<short>> MaxChanged;

			public static void OnMaxChanged(object sender, PropertyValueChangedEventArgs<short> e)
			{
				var obj = sender as ParentClass;

				if (obj.MaxChanged != null)
					obj.MaxChanged(obj, e);
			}

			#endregion

			#region PipeAddress Property and field

			[Obfuscation(Exclude = true, ApplyToMembers = false)]
			public string PipeAddress
			{
				get { return _PipeAddress; }
				set
				{
					if (AreEqualObjects(_PipeAddress, value))
						return;

					var _fieldOldValue = _PipeAddress;

					_PipeAddress = value;

					ParentClass.OnPipeAddressChanged(this, new PropertyValueChangedEventArgs<string>(_fieldOldValue, value));

					this.OnPropertyChanged("PipeAddress");
				}
			}

			private string _PipeAddress;

			public EventHandler<PropertyValueChangedEventArgs<string>> PipeAddressChanged;

			public static void OnPipeAddressChanged(object sender, PropertyValueChangedEventArgs<string> e)
			{
				var obj = sender as ParentClass;

				if (obj.PipeAddressChanged != null)
					obj.PipeAddressChanged(obj, e);
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

					ParentClass.OnIsIdleChanged(this, new PropertyValueChangedEventArgs<bool>(_fieldOldValue, value));

					this.OnPropertyChanged("IsIdle");
				}
			}

			private bool _IsIdle;

			public EventHandler<PropertyValueChangedEventArgs<bool>> IsIdleChanged;

			public static void OnIsIdleChanged(object sender, PropertyValueChangedEventArgs<bool> e)
			{
				var obj = sender as ParentClass;

				if (obj.IsIdleChanged != null)
					obj.IsIdleChanged(obj, e);
			}

			#endregion

		}
    }

}

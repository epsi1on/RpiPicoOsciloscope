using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Timers;

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


		internal void Init()
        {
			{
                var tmr = new Timer();
				tmr.Elapsed += (a, b) =>
				{
                    this.TotalSmaples = UiState.Instance.CurrentRepo.Channels.Sum(i => i.TotalWrites);
					this.TotalSamplesStr = Utils.numStr(this.TotalSmaples);
                };

                tmr.Interval = 1000;
                tmr.AutoReset = true;
                tmr.Start();
            }


        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using System.ComponentModel;
using Confabulation.Chat;

namespace Confabulation
{
	public abstract class ConnectionListItem : DispatcherObject, INotifyPropertyChanged
	{
		public ConnectionListItem() : base()
		{
		}

		public abstract ChatWindow ChatWindow
		{
			get;
		}

		public abstract IrcConnection Connection
		{
			get;
		}

		public bool IsSelected
		{
			get
			{
				return isSelected;
			}
			set
			{
				isSelected = value;
				NotifyPropertyChanged("IsSelected");
			}
		}

		public bool IsExpanded
		{
			get
			{
				return isExpanded;
			}
			set
			{
				isExpanded = value;
				NotifyPropertyChanged("IsExpanded");
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void NotifyPropertyChanged(String info)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(info));
			}
		}

		private bool isSelected = false;
		private bool isExpanded = false;
	}
}

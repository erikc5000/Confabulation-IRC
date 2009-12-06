using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Confabulation
{
	/// <summary>
	/// Interaction logic for JoinChannelWindow.xaml
	/// </summary>
	public partial class JoinChannelWindow : Window
	{
		public JoinChannelWindow(ConnectionItem connection)
		{
			InitializeComponent();

			this.connection = connection;
			
			App app = (App)App.Current;
			connectionComboBox.DataContext = app.Connections;
			//connectionComboBox.SelectedValue = connection;
		}

		private ConnectionItem connection;
	}
}

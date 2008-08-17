using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using Confabulation.Chat;
using Confabulation.Controls;

namespace Confabulation
{
	/// <summary>
	/// Interaction logic for NetworkSelectionPage.xaml
	/// </summary>
	public partial class NetworkSelectionPage : AeroWizardPageFunction<IrcConnectionSettings>
	{
		public NetworkSelectionPage()
		{
			InitializeComponent();

			XmlDataProvider dp = NetworkSP.TryFindResource("NetworkData") as XmlDataProvider;
			XmlDocument doc = new XmlDocument();
			doc.Load(App.GetUserServersFile());
			dp.Document = doc;
			dp.XPath = "Servers";

			SetNextButtonState();
			//Loaded += new RoutedEventHandler(NetworkSelectionPage_Loaded);
		}

		void NetworkSelectionPage_Loaded(object sender, RoutedEventArgs e)
		{
			XmlDataProvider dp = NetworkSP.TryFindResource("NetworkData") as XmlDataProvider;
			XmlDocument doc = new XmlDocument();
			doc.Load(App.GetUserServersFile());
			dp.Document = doc;
			dp.XPath = "Servers";
			
			//dp.IsInitialLoadEnabled = false;
			//dp.IsAsynchronous = false;
			//dp.Source = new Uri("Servers.xml", UriKind.Relative);
			//dp.Refresh();
			//UpdateLayout();
		}

		void NetworkSelectionPage_NextButtonClick(object sender, RoutedEventArgs e)
		{
			if (ExistingNetworkRB.IsChecked == true)
				NavigationService.Navigate(new Uri("UserSettingsPage.xaml", UriKind.Relative));
			else
				NavigationService.Navigate(new Uri("ServerSettingsPage.xaml", UriKind.Relative));
		}

		private void NetworkCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			SetNextButtonState();
		}

		private void ExistingNetworkRB_Checked(object sender, RoutedEventArgs e)
		{
			SetNextButtonState();
		}

		private void ManualServerRB_Checked(object sender, RoutedEventArgs e)
		{
			SetNextButtonState();
		}

		private void NetworkCB_Loaded(object sender, RoutedEventArgs e)
		{
			//foreach (IrcNetwork network in App.ServerList.Networks)
			//{
			//    NetworkCB.Items.Add(network.Name);
			//}

			//NetworkCB.SelectedIndex = 0;
			Keyboard.Focus(NetworkCB);
		}

		private void SetNextButtonState()
		{
			if (ExistingNetworkRB == null || (ExistingNetworkRB.IsChecked == true
			    && (NetworkCB == null || NetworkCB.SelectedItem == null)))
			{
				IsNextButtonEnabled = false;
			}
			else
			{
				IsNextButtonEnabled = true;
			}
		}
	}
}

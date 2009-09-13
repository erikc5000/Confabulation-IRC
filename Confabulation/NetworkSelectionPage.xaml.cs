using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.Xml;
using System.Xml.Linq;
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

			SetNextButtonState();
		}

		~NetworkSelectionPage()
		{

		}

		void NetworkSelectionPage_Loaded(object sender, RoutedEventArgs e)
		{
			App app = (App)App.Current;
			NetworkCB.DataContext = app.ServerList.Networks;

			if (app.ServerList.Networks.Count() > 0)
			{
			    Keyboard.Focus(NetworkCB);
			}
			else
			{
				ExistingNetworkRB.IsEnabled = false;
				ManualServerRB.IsChecked = true;
			}
		}

		void NetworkSelectionPage_NextButtonClick(object sender, RoutedEventArgs e)
		{
			if (ExistingNetworkRB.IsChecked == true)
			{
				UserSettingsPage page = new UserSettingsPage();
				page.Return += new ReturnEventHandler<IrcConnectionSettings>(NetworkSelectionPage_Return);
				NavigationService.Navigate(page);
			}
			else
			{
				ServerSettingsPage page = new ServerSettingsPage();
				page.Return += new ReturnEventHandler<IrcConnectionSettings>(NetworkSelectionPage_Return);
				NavigationService.Navigate(page);
			}
		}

		void NetworkSelectionPage_Return(object sender, ReturnEventArgs<IrcConnectionSettings> e)
		{
			IrcNetwork network = (IrcNetwork)NetworkCB.SelectedItem;

			if (network != null)
			{
				IrcConnectionSettings settings = e.Result;

				// Make sure the server wasn't already set in the ServerSettingsPage
				if (settings.Server == null)
				{
					settings.Name = network.Name;
					settings.Server = network.GetFirstServer();
				}

				Properties.Settings.Default.LastNetworkName = network.Name;
			}

			OnReturn(e);
		}

		private void NetworkCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			SetNextButtonState();
		}

		private void ExistingNetworkRB_Checked(object sender, RoutedEventArgs e)
		{
			if (NetworkCB != null)
				NetworkCB.IsEnabled = true;
			
			SetNextButtonState();
		}

		private void ManualServerRB_Checked(object sender, RoutedEventArgs e)
		{
			if (NetworkCB != null)
				NetworkCB.IsEnabled = false;

			SetNextButtonState();
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

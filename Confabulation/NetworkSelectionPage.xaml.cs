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
			Loaded += new RoutedEventHandler(NetworkSelectionPage_Loaded);
			Unloaded += new RoutedEventHandler(NetworkSelectionPage_Unloaded);
		}

		~NetworkSelectionPage()
		{

		}

		void NetworkSelectionPage_Unloaded(object sender, RoutedEventArgs e)
		{
			IrcNetwork network = (IrcNetwork)NetworkCB.SelectedItem;

			if (network != null)
				Properties.Settings.Default.LastNetworkName = network.Name;
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
			IrcConnectionSettings settings = e.Result;

			if (settings.Server == null)
			{
				settings.Name = ((IrcNetwork)NetworkCB.SelectedItem).Name;
				settings.Server = ((IrcNetwork)NetworkCB.SelectedItem).GetFirstServer();
			}

			OnReturn(e);
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

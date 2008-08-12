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

			if (ExistingNetwork.IsChecked == true && networkCB.SelectedItem == null)
				IsNextButtonEnabled = false;
		}

		void NetworkSelectionPage_NextButtonClick(object sender, RoutedEventArgs e)
		{
			if (ExistingNetwork.IsChecked == true)
				NavigationService.Navigate(new Uri("UserSettingsPage.xaml", UriKind.Relative));
			else
				NavigationService.Navigate(new Uri("ServerSettingsPage.xaml", UriKind.Relative));
		}

		private void networkCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (ExistingNetwork.IsChecked == true && networkCB.SelectedItem == null)
				IsNextButtonEnabled = false;
			else
				IsNextButtonEnabled = true;
		}

		private void ExistingNetwork_Checked(object sender, RoutedEventArgs e)
		{
			if (ExistingNetwork.IsChecked == true && networkCB != null && networkCB.SelectedItem == null)
				IsNextButtonEnabled = false;
			else
				IsNextButtonEnabled = true;
		}

		private void ManualServer_Checked(object sender, RoutedEventArgs e)
		{
			IsNextButtonEnabled = true;
		}

		private void networkCB_Loaded(object sender, RoutedEventArgs e)
		{
			Keyboard.Focus(networkCB);
		}
	}
}

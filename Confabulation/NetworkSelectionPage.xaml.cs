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
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			NextButtonClick += new RoutedEventHandler(NetworkSelectionPage_NextButtonClick);
		}

		void NetworkSelectionPage_NextButtonClick(object sender, RoutedEventArgs e)
		{
			NextButtonClick -= new RoutedEventHandler(NetworkSelectionPage_NextButtonClick);
			NavigationService.Navigate(new Uri("UserSettingsPage.xaml", UriKind.Relative));
		}
	}
}

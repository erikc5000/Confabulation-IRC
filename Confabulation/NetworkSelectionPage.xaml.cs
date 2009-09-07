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
			//Unloaded += new RoutedEventHandler(NetworkSelectionPage_Unloaded);
		}

		~NetworkSelectionPage()
		{

		}

		//void NetworkSelectionPage_Unloaded(object sender, RoutedEventArgs e)
		//{
		//    //RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\Confabulation", true);

		//    //key.SetValue("LastNetworkName", ((XElement)NetworkCB.SelectedItem).Attribute("Name").Value, RegistryValueKind.String );
		//}

		void NetworkSelectionPage_Loaded(object sender, RoutedEventArgs e)
		{
			App app = (App)App.Current;
			NetworkCB.DataContext = app.ServerList.Networks;
			//NavigationService.LoadCompleted += new LoadCompletedEventHandler(NavigationService_LoadCompleted);
			//XDocument serverList = App.ServerList;

			//if (serverList == null)
			//{
			//    ExistingNetworkRB.IsEnabled = false;
			//    NetworkCB.IsEnabled = false;
			//}
			//else
			//{
			//    try
			//    {
			//        //NetworkCB.DataContextChanged += new DependencyPropertyChangedEventHandler(NetworkCB_DataContextChanged);
			//        NetworkCB.DataContext = serverList.Element("Servers").Elements();

			//        RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\Confabulation");

			//        if (key == null)
			//            key = Registry.CurrentUser.CreateSubKey("Software\\Confabulation");

			//        string lastNetworkName = (string)key.GetValue("LastNetworkName", null);

			//        if (lastNetworkName == null)
			//        {
			//            NetworkCB.SelectedIndex = 0;
			//        }
			//        else
			//        {
			//            //NetworkCB.Item
			//            NetworkCB.SelectedItem =
			//                (from c in serverList.Element("Servers").Elements()
			//                 where c.Attribute("Name").Value.Equals(lastNetworkName)
			//                 select c).First();

			//            if (NetworkCB.SelectedItem == null)
			//                NetworkCB.SelectedIndex = 0;
			//        }

			//        Keyboard.Focus(NetworkCB);
			//    }
			//    catch (NullReferenceException)
			//    {
			//        ExistingNetworkRB.IsEnabled = false;
			//        NetworkCB.IsEnabled = false;
			//    }
			//}
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

		//void NavigationService_LoadCompleted(object sender, NavigationEventArgs e)
		//{
		//    //if (e.ExtraData == this)
		//    //{
		//    //    ((PageFunction<IrcConnectionSettings>)e.Content).Return
		//    //        += new ReturnEventHandler<IrcConnectionSettings>(NetworkSelectionPage_Return);
		//    //}
		//}

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

		private void NetworkCB_Loaded(object sender, RoutedEventArgs e)
		{
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

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
using System.Windows.Shapes;
using System.Windows.Navigation;
using Confabulation.Controls;
using Confabulation.Chat;
using System.Xml;
using System.Windows.Markup;

namespace Confabulation
{
	public class NewConnectionDummyPage : AeroWizardPageFunction<IrcConnectionSettings>
	{
		public NewConnectionDummyPage()
		{
			this.KeepAlive = true;
			Loaded += new RoutedEventHandler(NewConnectionDummyPage_Loaded);
		}

		public NewConnectionDummyPage(NewConnectionWindow window)
		{
			this.KeepAlive = true;
			this.window = window;
			Loaded += new RoutedEventHandler(NewConnectionDummyPage_Loaded);
		}

		void NewConnectionDummyPage_Loaded(object sender, RoutedEventArgs e)
		{
			NetworkSelectionPage page = new NetworkSelectionPage();
			page.Return += new ReturnEventHandler<IrcConnectionSettings>(page_Return);
			NavigationService.Navigate(page, this);
		}

		void page_Return(object sender, ReturnEventArgs<IrcConnectionSettings> e)
		{
			IrcConnectionSettings settings = e.Result;

			IrcConnection connection = new IrcConnection(settings);
			App app = (App)App.Current;
			app.AddConnection(connection);

			connection.Initiate();

			window.Close();
			OnReturn(e);
		}

		private NewConnectionWindow window;
	}

	/// <summary>
	/// Interaction logic for NewConnectionWindow.xaml
	/// </summary>
	public partial class NewConnectionWindow : AeroWizard
	{
		public NewConnectionWindow()
		{
			InitializeComponent();
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			Frame frame = Template.FindName("PART_Frame", this) as Frame;

			if (frame != null)
			{
				NewConnectionDummyPage page = new NewConnectionDummyPage(this);
				page.Return += new ReturnEventHandler<IrcConnectionSettings>(page_Return);
				frame.NavigationService.Navigate(page, this);
				//frame.NavigationService.LoadCompleted += new LoadCompletedEventHandler(NavigationService_LoadCompleted);
			}
		}

		void page_Return(object sender, ReturnEventArgs<IrcConnectionSettings> e)
		{
			Close();
		}

		//void NavigationService_LoadCompleted(object sender, NavigationEventArgs e)
		//{
		//    if (e.ExtraData == this)
		//    {
		//        ((PageFunction<IrcConnectionSettings>)e.Content).Return +=
		//            new ReturnEventHandler<IrcConnectionSettings>(NewConnectionWindow_Return);
		//    }
		//}

		//void NewConnectionWindow_Return(object sender, ReturnEventArgs<IrcConnectionSettings> e)
		//{
		//    IrcConnectionSettings settings = e.Result;

		//    IrcConnection connection = new IrcConnection(settings);
			
		//    // App.AddConnection

		//    Close();
		//}
	}
}

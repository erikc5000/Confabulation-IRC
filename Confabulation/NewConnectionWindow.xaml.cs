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
	/// <summary>
	/// A window cannot process a the result of a PageFunction, so we have to use
	/// a start page that launches the first real page of the wizard.
	/// </summary>
	public class NewConnectionStartPage : AeroWizardPageFunction<IrcConnectionSettings>
	{
		public NewConnectionStartPage()
		{
			this.KeepAlive = true;
			Loaded += new RoutedEventHandler(NewConnectionStartPage_Loaded);
		}

		public NewConnectionStartPage(NewConnectionWindow window)
		{
			this.KeepAlive = true;
			this.window = window;
			Loaded += new RoutedEventHandler(NewConnectionStartPage_Loaded);
		}

		private void NewConnectionStartPage_Loaded(object sender, RoutedEventArgs e)
		{
			NetworkSelectionPage page = new NetworkSelectionPage();
			page.Return += new ReturnEventHandler<IrcConnectionSettings>(page_Return);
			NavigationService.Navigate(page, this);
		}

		private void page_Return(object sender, ReturnEventArgs<IrcConnectionSettings> e)
		{
			IrcConnectionSettings settings = e.Result;

			IrcConnection connection = new IrcConnection(settings);
			App app = (App)App.Current;
			app.AddConnection(connection);

			connection.Initiate();

			Properties.Settings.Default.Save();
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
				NewConnectionStartPage page = new NewConnectionStartPage(this);
				frame.NavigationService.Navigate(page, this);
			}
		}
	}
}

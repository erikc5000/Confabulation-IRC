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
				frame.NavigationService.LoadCompleted += new LoadCompletedEventHandler(NavigationService_LoadCompleted);
			}
		}

		void NavigationService_LoadCompleted(object sender, NavigationEventArgs e)
		{
			if (e.ExtraData == this)
			{
				((PageFunction<IrcConnectionSettings>)e.Content).Return +=
					new ReturnEventHandler<IrcConnectionSettings>(NewConnectionWindow_Return);
			}
		}

		void NewConnectionWindow_Return(object sender, ReturnEventArgs<IrcConnectionSettings> e)
		{
			IrcConnectionSettings settings = e.Result;

			IrcConnection connection = new IrcConnection(settings);
			
			// App.AddConnection

			Close();
		}
	}
}

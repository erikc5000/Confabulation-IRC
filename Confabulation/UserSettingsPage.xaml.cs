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
	/// Interaction logic for UserSettingsPage.xaml
	/// </summary>
	public partial class UserSettingsPage : AeroWizardPageFunction<IrcConnectionSettings>
	{
		public UserSettingsPage()
		{
			InitializeComponent();
		}

		private void Nickname_Loaded(object sender, RoutedEventArgs e)
		{
			Keyboard.Focus(Nickname);
		}
	}
}

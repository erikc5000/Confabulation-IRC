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
using System.Security.Principal;
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

			SetNextButtonState();
		}

		private void Nickname_Loaded(object sender, RoutedEventArgs e)
		{
			Keyboard.Focus(Nickname);
			Nickname.CaretIndex = Nickname.Text.Length;
		}

		private void SetNextButtonState()
		{
			if (Nickname == null || Nickname.Text == null || Nickname.Text.Length == 0
				|| UserName == null || UserName.Text == null || UserName.Text.Length == 0
				|| RealName == null || RealName.Text == null || RealName.Text.Length == 0)
			{
				IsNextButtonEnabled = false;
			}
			else
			{
				IsNextButtonEnabled = true;
			}
		}

		private void UserName_TextChanged(object sender, TextChangedEventArgs e)
		{
			SetNextButtonState();
		}

		private void RealName_TextChanged(object sender, TextChangedEventArgs e)
		{
			SetNextButtonState();
		}

		private void Nickname_TextChanged(object sender, TextChangedEventArgs e)
		{
			SetNextButtonState();
		}

		private void NextButton_Click(object sender, RoutedEventArgs e)
		{
			IrcConnectionSettings settings = new IrcConnectionSettings();
			settings.User.Nicknames.Add(Nickname.Text);
			settings.User.Nicknames.Add(Nickname.Text + "`");
			settings.User.Nicknames.Add(Nickname.Text + "_");
			settings.User.Nicknames.Add(Nickname.Text + "^");
			settings.User.UserName = UserName.Text;
			settings.User.RealName = RealName.Text;

			if (Invisible.IsChecked == true)
				settings.InitialUserModes = InitialUserMode.Invisible;
			else
				settings.InitialUserModes = InitialUserMode.None;

			Properties.Settings.Default.Nickname = Nickname.Text;
			Properties.Settings.Default.UserName = UserName.Text;
			Properties.Settings.Default.RealName = RealName.Text;

			OnReturn(new ReturnEventArgs<IrcConnectionSettings>(settings));
		}

		private void UserName_Loaded(object sender, RoutedEventArgs e)
		{
			if (UserName.Text.Length == 0)
				UserName.Text = WindowsIdentity.GetCurrent().Name.Split('\\')[1];
		}
	}
}

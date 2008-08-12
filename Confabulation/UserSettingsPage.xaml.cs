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
	public partial class UserSettingsPage : AeroWizardPageFunction<IrcUserSettings>
	{
		public UserSettingsPage()
		{
			InitializeComponent();

			SetNextButtonState();
		}

		private void Nickname_Loaded(object sender, RoutedEventArgs e)
		{
			Keyboard.Focus(Nickname);
		}

		private void SetNextButtonState()
		{
			if (Nickname.Text == null || Nickname.Text.Length == 0
				|| UserName.Text == null || UserName.Text.Length == 0
				|| RealName.Text == null || RealName.Text.Length == 0)
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
			IrcUserSettings settings = new IrcUserSettings();
			settings.Nicknames.Add(Nickname.Text);
			settings.Nicknames.Add(Nickname.Text + "`");
			settings.Nicknames.Add(Nickname.Text + "_");
			settings.Nicknames.Add(Nickname.Text + "^");
			settings.UserName = UserName.Text;
			settings.RealName = RealName.Text;
			settings.Invisible = (Invisible.IsChecked == true);

			OnReturn(new ReturnEventArgs<IrcUserSettings>(settings));
		}
	}
}

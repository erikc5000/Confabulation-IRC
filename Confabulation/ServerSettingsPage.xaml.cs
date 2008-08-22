﻿using System;
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
using Confabulation.Controls;
using Confabulation.Chat;

namespace Confabulation
{
	/// <summary>
	/// Interaction logic for ServerSettingsPage.xaml
	/// </summary>
	public partial class ServerSettingsPage : AeroWizardPageFunction<IrcConnectionSettings>
	{
		public ServerSettingsPage()
		{
			InitializeComponent();

			SetNextButtonState();
		}

		private void Address_Loaded(object sender, RoutedEventArgs e)
		{
			Keyboard.Focus(Address);
		}

		void NextButton_Click(object sender, RoutedEventArgs e)
		{
			NavigationService.Navigate(new Uri("UserSettingsPage.xaml", UriKind.Relative));
		}

		private void Address_TextChanged(object sender, TextChangedEventArgs e)
		{
			SetNextButtonState();
		}

		private void Ports_TextChanged(object sender, TextChangedEventArgs e)
		{
			SetNextButtonState();
		}

		private void SetNextButtonState()
		{
			if (Address.Text == null || Address.Text.Length == 0
				|| Ports.Text == null || Ports.Text.Length == 0)
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
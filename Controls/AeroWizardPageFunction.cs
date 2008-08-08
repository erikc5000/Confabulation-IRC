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

namespace Confabulation.Controls
{
	//public struct AeroWizardData
	//{
	//    Object extraData;


	//}

	interface IAeroWizardPage
	{
		void SetButtons(Button nextButton, Button backButton, Button cancelButton);
	}


	public class AeroWizardPageFunction<T> : PageFunction<T>, IAeroWizardPage
	{
		public event RoutedEventHandler NextButtonClick;
		public event RoutedEventHandler BackButtonClick;
		public event RoutedEventHandler CancelButtonClick;

		public bool IsNextButtonVisible
		{
			get { return (bool)GetValue(IsNextButtonVisibleProperty); }
			set { SetValue(IsNextButtonVisibleProperty, value); }
		}

		public static readonly DependencyProperty IsNextButtonVisibleProperty =
			DependencyProperty.Register("IsNextButtonVisible",
			typeof(bool),
			typeof(AeroWizardPageFunction<T>),
			new FrameworkPropertyMetadata(true));

		public bool IsNextButtonEnabled
		{
			get { return (bool)GetValue(IsNextButtonEnabledProperty); }
			set { SetValue(IsNextButtonEnabledProperty, value); }
		}

		public static readonly DependencyProperty IsNextButtonEnabledProperty =
			DependencyProperty.Register("IsNextButtonEnabled",
			typeof(bool),
			typeof(AeroWizardPageFunction<T>),
			new FrameworkPropertyMetadata(true));


		public string NextButtonText
		{
			get { return (string)GetValue(NextButtonTextProperty); }
			set { SetValue(NextButtonTextProperty, value); }
		}

		public static readonly DependencyProperty NextButtonTextProperty =
			DependencyProperty.Register("NextButtonText",
			typeof(string),
			typeof(AeroWizardPageFunction<T>),
			new FrameworkPropertyMetadata("_Next"));


		static AeroWizardPageFunction()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(AeroWizardPageFunction<T>), new FrameworkPropertyMetadata(typeof(Page)));
		}

		#region IAeroWizardPage Members

		public void SetButtons(Button nextButton, Button backButton, Button cancelButton)
		{
			nextButton.Click += new RoutedEventHandler(nextButton_Click);
			backButton.Click += new RoutedEventHandler(backButton_Click);
			cancelButton.Click += new RoutedEventHandler(cancelButton_Click);

			Binding binding = new Binding("IsNextButtonEnabled");
			binding.Source = this;
			nextButton.SetBinding(Button.IsEnabledProperty, binding);

			//binding = new Binding("IsNextButtonVisible");
			//binding.Source = this;
			//nextButton.SetBinding(Button.IsVisibleProperty, binding);

			binding = new Binding("NextButtonText");
			binding.Source = this;
			nextButton.SetBinding(Button.ContentProperty, binding);
		}

		#endregion

		void cancelButton_Click(object sender, RoutedEventArgs e)
		{
			RoutedEventHandler handler = CancelButtonClick;

			if (handler != null)
				handler(this, e);
		}

		void backButton_Click(object sender, RoutedEventArgs e)
		{
			RoutedEventHandler handler = BackButtonClick;

			if (handler != null)
				handler(this, e);
		}

		void nextButton_Click(object sender, RoutedEventArgs e)
		{
			RoutedEventHandler handler = NextButtonClick;

			if (handler != null)
				handler(this, e);
		}
	}
}

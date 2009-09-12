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
	interface IAeroWizardPage
	{
		bool IsNextButtonVisible
		{
			get;
			set;
		}

		bool IsNextButtonEnabled
		{
			get;
			set;
		}

		string NextButtonText
		{
			get;
			set;
		}

		void OnNextButtonClick(object sender, RoutedEventArgs e);

		void OnBackButtonClick(object sender, RoutedEventArgs e);

		void OnCancelButtonClick(object sender, RoutedEventArgs e);
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

		public void OnCancelButtonClick(object sender, RoutedEventArgs e)
		{
			RoutedEventHandler handler = CancelButtonClick;

			if (handler != null)
				handler(this, e);
		}

		public void OnBackButtonClick(object sender, RoutedEventArgs e)
		{
			RoutedEventHandler handler = BackButtonClick;

			if (handler != null)
				handler(this, e);
		}

		public void OnNextButtonClick(object sender, RoutedEventArgs e)
		{
			RoutedEventHandler handler = NextButtonClick;

			if (handler != null)
				handler(this, e);
		}
	}
}

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
using System.Windows.Interop;
using System.Runtime.InteropServices;

namespace Confabulation.Controls
{
	[TemplatePart(Name = "PART_Frame", Type = typeof(Frame))]
	[TemplatePart(Name = "PART_NextButton", Type = typeof(Button))]
	[TemplatePart(Name = "PART_BackButton", Type = typeof(Button))]
	[TemplatePart(Name = "PART_CancelButton", Type = typeof(Button))]
	public class AeroWizard : Window
	{
		[DllImport("user32.dll")]
		static extern int GetWindowLong(IntPtr hwnd, int index);

		[DllImport("user32.dll")]
		static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

		[DllImport("user32.dll")]
		static extern bool SetWindowPos(IntPtr hwnd, IntPtr hwndInsertAfter, int x, int y, int width, int height, uint flags);

		[DllImport("user32.dll")]
		static extern IntPtr SendMessage(IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam);

		public static readonly DependencyProperty InitialPageProperty =
			DependencyProperty.Register("InitialPage", typeof(Uri), typeof(AeroWizard), new UIPropertyMetadata());

		public static new readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title",
			typeof(string),
			typeof(AeroWizard),
			new FrameworkPropertyMetadata());

		public static new readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon",
			typeof(ImageSource),
			typeof(AeroWizard),
			new FrameworkPropertyMetadata());

		public static readonly DependencyProperty IsGlassEnabledProperty = DependencyProperty.Register("IsGlassEnabled",
			typeof(bool),
			typeof(AeroWizard),
			new FrameworkPropertyMetadata(true));

		public Uri InitialPage
		{
			get { return (Uri)GetValue(InitialPageProperty); }
			set { SetValue(InitialPageProperty, value); }
		}

		public new string Title
		{
			get
			{
				return (string)GetValue(TitleProperty);
			}
			set
			{
				SetValue(TitleProperty, value);
			}
		}

		public new ImageSource Icon
		{
			get
			{
				return (ImageSource)GetValue(IconProperty);
			}
			set
			{
				SetValue(IconProperty, value);
			}
		}

		public bool IsGlassEnabled
		{
			get
			{
				return (bool)GetValue(IsGlassEnabledProperty);
			}
			set
			{
				SetValue(IsGlassEnabledProperty, value);
			}
		}

		static AeroWizard()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(AeroWizard), new FrameworkPropertyMetadata(typeof(AeroWizard)));
		}

		public AeroWizard()
		{
			CommandBinding cb = new CommandBinding(NavigationCommands.BrowseBack);
			cb.CanExecute += new CanExecuteRoutedEventHandler(cb_CanExecute);
			cb.Executed += new ExecutedRoutedEventHandler(cb_Executed);

			CommandBindings.Add(cb);
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			//Style style = FindResource("NavigationWindow.Template") as Style; //Get button's default Style
			//if (style != null)
			//{
			//    XmlWriterSettings settings = new XmlWriterSettings();
			//    settings.Indent = true;
			//    settings.IndentChars = new string(' ', 4);
			//    XmlWriter xmlwrite = XmlWriter.Create("NavStyle.xml", settings);
			//    XamlWriter.Save(style, xmlwrite);//Use XamlWriter to dump the style
			//    //return strbuild.ToString();
			//}

			Button nextButton = Template.FindName("PART_NextButton", this) as Button;
			Button backButton = Template.FindName("PART_BackButton", this) as Button;
			Button cancelButton = Template.FindName("PART_CancelButton", this) as Button;

			if (InitialPage != null)
			{
				frame = Template.FindName("PART_Frame", this) as Frame;

				if (frame != null)
				{
					frame.NavigationUIVisibility = NavigationUIVisibility.Hidden;

					if (backButton != null)
					{
						Binding binding = new Binding("CanGoBack");
						binding.Source = frame;
						backButton.SetBinding(Button.IsEnabledProperty, binding);
					}

					frame.Navigate(InitialPage, this);
					frame.LoadCompleted += new LoadCompletedEventHandler(frame_LoadCompleted);
				}
			}

			if (nextButton != null)
				nextButton.Click += new RoutedEventHandler(nextButton_Click);

			if (backButton != null)
				backButton.Click += new RoutedEventHandler(backButton_Click);

			if (cancelButton != null)
				cancelButton.Click += new RoutedEventHandler(cancelButton_Click);
		}

		private void cancelButton_Click(object sender, RoutedEventArgs e)
		{
			if (frame != null)
				((IAeroWizardPage)frame.NavigationService.Content).OnCancelButtonClick(sender, e);
		}

		private void nextButton_Click(object sender, RoutedEventArgs e)
		{
			if (frame != null)
				((IAeroWizardPage)frame.NavigationService.Content).OnNextButtonClick(sender, e);
		}

		private void backButton_Click(object sender, RoutedEventArgs e)
		{
			if (frame != null)
				((IAeroWizardPage)frame.NavigationService.Content).OnBackButtonClick(sender, e);
		}

		private void cb_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			if (frame != null && frame.CanGoBack)
				frame.GoBack();
		}

		private void cb_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = true;
		}

		private void frame_LoadCompleted(object sender, NavigationEventArgs e)
		{
			Button nextButton = Template.FindName("PART_NextButton", this) as Button;
			Button backButton = Template.FindName("PART_BackButton", this) as Button;
			Button cancelButton = Template.FindName("PART_CancelButton", this) as Button;

			IAeroWizardPage page = ((IAeroWizardPage)e.Content);
			
			Binding binding = new Binding("IsNextButtonEnabled");
			binding.Source = page;
			nextButton.SetBinding(Button.IsEnabledProperty, binding);

			//binding = new Binding("IsNextButtonVisible");
			//binding.Source = this;
			//nextButton.SetBinding(Button.IsVisibleProperty, binding);

			binding = new Binding("NextButtonText");
			binding.Source = page;
			nextButton.SetBinding(Button.ContentProperty, binding);
		}

		protected override void OnSourceInitialized(EventArgs e)
		{
			base.OnSourceInitialized(e);
			
			// This can't be done any earlier than the SourceInitialized event:
			if (!GlassHelper.ExtendGlassFrame(this, new Thickness(0, 40, 0, 0)))
				IsGlassEnabled = false;
			else
				IsGlassEnabled = true;

			IntPtr hwnd = new WindowInteropHelper(this).Handle;
			HwndSource.FromHwnd(hwnd).AddHook(new HwndSourceHook(WndProc));

			// Change the extended window style to not show a window icon
			int extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
			SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_DLGMODALFRAME);

			// Update the window's non-client area to reflect the changes
			SetWindowPos(hwnd, IntPtr.Zero, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOZORDER | SWP_FRAMECHANGED);
		}

		private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			if (msg == WM_DWMCOMPOSITIONCHANGED)
			{
				// Re-enable glass
				if (!GlassHelper.ExtendGlassFrame(this, new Thickness(0, 40, 0, 0)))
					IsGlassEnabled = false;
				else
					IsGlassEnabled = true;

				handled = true;
			}

			return IntPtr.Zero;
		}

		private const int WM_DWMCOMPOSITIONCHANGED = 0x031E;
		private const int GWL_EXSTYLE = -20;
		private const int WS_EX_DLGMODALFRAME = 0x0001;
		private const int SWP_NOSIZE = 0x0001;
		private const int SWP_NOMOVE = 0x0002;
		private const int SWP_NOZORDER = 0x0004;
		private const int SWP_FRAMECHANGED = 0x0020;
		private const uint WM_SETICON = 0x0080;

		private Frame frame = null;
	}
}

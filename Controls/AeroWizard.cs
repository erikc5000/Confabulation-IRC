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
	[TemplatePart(Name = "NextButton", Type = typeof(Button))]
	[TemplatePart(Name = "BackButton", Type = typeof(Button))]
	[TemplatePart(Name = "CancelButton", Type = typeof(Button))]
	public class AeroWizard : NavigationWindow
	{
		[DllImport("user32.dll")]
		static extern int GetWindowLong(IntPtr hwnd, int index);

		[DllImport("user32.dll")]
		static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

		[DllImport("user32.dll")]
		static extern bool SetWindowPos(IntPtr hwnd, IntPtr hwndInsertAfter, int x, int y, int width, int height, uint flags);

		[DllImport("user32.dll")]
		static extern IntPtr SendMessage(IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam);

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

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			//if (InitialPage != null)
			//{
			//    frame = Template.FindName("PART_Frame", this) as Frame;

			//    if (frame != null)
			//    {
			//        Button backButton = Template.FindName("BackButton", this) as Button;

			//        if (backButton != null)
			//        {
			//            Binding binding = new Binding("CanGoBack");
			//            binding.Source = frame;
			//            backButton.SetBinding(Button.IsEnabledProperty, binding);
			//        }

			//        frame.Navigate(InitialPage);
			        LoadCompleted += new LoadCompletedEventHandler(frame_LoadCompleted);
			//    }
			//}
		}

		private void frame_LoadCompleted(object sender, NavigationEventArgs e)
		{
			Button nextButton = Template.FindName("NextButton", this) as Button;
			Button backButton = Template.FindName("BackButton", this) as Button;
			Button cancelButton = Template.FindName("CancelButton", this) as Button;

			((IAeroWizardPage)e.Content).SetButtons(nextButton, backButton, cancelButton);
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
	}
}

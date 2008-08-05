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
	/// <summary>
	/// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
	///
	/// Step 1a) Using this custom control in a XAML file that exists in the current project.
	/// Add this XmlNamespace attribute to the root element of the markup file where it is 
	/// to be used:
	///
	///     xmlns:MyNamespace="clr-namespace:Confabulation.Controls"
	///
	///
	/// Step 1b) Using this custom control in a XAML file that exists in a different project.
	/// Add this XmlNamespace attribute to the root element of the markup file where it is 
	/// to be used:
	///
	///     xmlns:MyNamespace="clr-namespace:Confabulation.Controls;assembly=Confabulation.Controls"
	///
	/// You will also need to add a project reference from the project where the XAML file lives
	/// to this project and Rebuild to avoid compilation errors:
	///
	///     Right click on the target project in the Solution Explorer and
	///     "Add Reference"->"Projects"->[Browse to and select this project]
	///
	///
	/// Step 2)
	/// Go ahead and use your control in the XAML file.
	///
	///     <MyNamespace:AeroWizard/>
	///
	/// </summary>
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

		public static new readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title",
			typeof(string),
			typeof(AeroWizard),
			new FrameworkPropertyMetadata());

		public static new readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon",
			typeof(ImageSource),
			typeof(AeroWizard),
			new FrameworkPropertyMetadata());

		public static readonly DependencyProperty InitialPageProperty = DependencyProperty.Register("InitialPage",
			typeof(Uri),
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

		public Uri InitialPage
		{
			get
			{
				return (Uri)GetValue(InitialPageProperty);
			}
			set
			{
				SetValue(InitialPageProperty, value);
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

			if (InitialPage != null)
			{
				frame = Template.FindName("PART_Frame", this) as Frame;

				if (frame != null)
				{
					frame.Navigate(InitialPage, null);
				}
			}
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

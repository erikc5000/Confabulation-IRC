using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace Confabulation.Controls
{
	public enum TaskDialogResult
	{
		Ok = 1,
		Cancel = 2,
		Retry = 4,
		Yes = 6,
		No = 7,
		Close = 8
	}

	[Flags]
	public enum TaskDialogButtons
	{
		Ok = 0x0001,
		Yes = 0x0002,
		No = 0x0004,
		Cancel = 0x0008,
		Retry = 0x0010,
		Close = 0x0020
	}

	public enum TaskDialogIcon
	{
		Warning = 65535,
		Error = 65534,
		Information = 65533,
		Shield = 65532
	}

	public static class TaskDialog
	{
		[DllImport("comctl32.dll", CharSet = CharSet.Unicode, EntryPoint = "TaskDialog")]
		static extern int _TaskDialog(IntPtr hWndParent,
		                              IntPtr hInstance,
		                              String pszWindowTitle,
		                              String pszMainInstruction,
		                              String pszContent,
		                              int dwCommonButtons,
		                              IntPtr pszIcon,
		                              out int pnButton);

		public static TaskDialogResult Show(IWin32Window owner, string text)
		{
			return Show(owner, text, null, null, TaskDialogButtons.Ok);
		}

		public static TaskDialogResult Show(IWin32Window owner, string text, string instruction)
		{
			return Show(owner, text, instruction, null, TaskDialogButtons.Ok);
		}

		public static TaskDialogResult Show(IWin32Window owner, string text, string instruction, string caption)
		{
			return Show(owner, text, instruction, caption, TaskDialogButtons.Ok);
		}

		public static TaskDialogResult Show(IWin32Window owner,
		                                    string text,
		                                    string instruction,
		                                    string caption,
		                                    TaskDialogButtons buttons)
		{
			return Show(owner, text, instruction, caption, buttons, 0);
		}

		public static TaskDialogResult Show(IWin32Window owner,
		                                    string text,
		                                    string instruction,
		                                    string caption,
		                                    TaskDialogButtons buttons,
		                                    TaskDialogIcon icon)
		{
			return ShowInternal(owner.Handle, text, instruction, caption, buttons, icon);
		}

		public static TaskDialogResult Show(string text)
		{
			return Show(text, null, null, TaskDialogButtons.Ok);
		}

		public static TaskDialogResult Show(string text, string instruction)
		{
			return Show(text, instruction, null, TaskDialogButtons.Ok, 0);
		}

		public static TaskDialogResult Show(string text, string instruction, string caption)
		{
			return Show(text, instruction, caption, TaskDialogButtons.Ok, 0);
		}

		public static TaskDialogResult Show(string text, string instruction, string caption, TaskDialogButtons buttons)
		{
			return Show(text, instruction, caption, buttons, 0);
		}

		public static TaskDialogResult Show(string text,
		                                    string instruction,
		                                    string caption,
		                                    TaskDialogButtons buttons,
		                                    TaskDialogIcon icon)
		{
			return ShowInternal(IntPtr.Zero, text, instruction, caption, buttons, icon);
		}

		private static TaskDialogResult ShowInternal(IntPtr owner,
													 string text,
													 string instruction,
													 string caption,
													 TaskDialogButtons buttons,
													 TaskDialogIcon icon)
		{
			int p;

			if (_TaskDialog(owner, IntPtr.Zero, caption, instruction, text, (int)buttons, new IntPtr((int)icon), out p) != 0)
				throw new InvalidOperationException("An error occurred displaying the task dialog.");

			return (TaskDialogResult)p;
		}
	}
}

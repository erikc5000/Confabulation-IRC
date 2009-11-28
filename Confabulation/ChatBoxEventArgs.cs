using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Confabulation
{
	public class ChatBoxEventArgs : EventArgs
	{
		public ChatBoxEventArgs(string text)
		{
			Text = text;
		}

		public string Text
		{
			get;
			private set;
		}
	}
}

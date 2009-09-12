using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Confabulation.Chat
{
	public class IrcMessageEventArgs : EventArgs
	{
		public IrcMessageEventArgs(IrcMessage message)
		{
			this.message = message;
		}

		public IrcMessage Message
		{
			get
			{
				return message;
			}
		}

		private IrcMessage message;
	}
}

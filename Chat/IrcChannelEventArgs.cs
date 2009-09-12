using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Confabulation.Chat
{
	public class IrcChannelEventArgs : EventArgs
	{
		public IrcChannelEventArgs(IrcChannel channel)
		{
			if (channel == null)
				throw new ArgumentNullException("channel");

			Channel = channel;
		}

		public IrcChannel Channel
		{
			get;
			private set;
		}
	}
}

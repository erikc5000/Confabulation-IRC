using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Confabulation.Chat
{
	public enum IrcConnectionEventType
	{
		ResolveFailed,
		ResolveSucceeded,
		ConnectFailed,
		Connected,
		Registered,
		Disconnected
	}

	public class IrcConnectionEventArgs : EventArgs
	{
		public IrcConnectionEventArgs(IrcConnectionEventType eventType)
		{
			this.eventType = eventType;
		}

		public IrcConnectionEventType EventType
		{
			get
			{
				return eventType;
			}
		}

		private IrcConnectionEventType eventType;
	}
}

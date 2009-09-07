using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Confabulation.Chat;

namespace Confabulation
{
	public class ConnectionEventArgs : EventArgs
	{
		public ConnectionEventArgs(IrcConnection connection)
		{
			this.Connection = connection;
		}

		public IrcConnection Connection
		{
			get;
			private set;
		}
	}
}

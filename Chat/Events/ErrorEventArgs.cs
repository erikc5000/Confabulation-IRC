using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Confabulation.Chat
{
	public class ErrorEventArgs : EventArgs
	{
		public ErrorEventArgs(IrcNumericReply errorCode, string message)
		{
			ErrorCode = errorCode;
			Message = message;
		}

		public IrcNumericReply ErrorCode
		{
			get;
			private set;
		}

		public string Message
		{
			get;
			private set;
		}
	}
}

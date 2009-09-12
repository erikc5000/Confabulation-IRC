using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Confabulation.Chat
{
	public enum IrcCommandExceptionType
	{
		InvalidCommand,
		TooFewParameters,
		TooManyParameters,
		InvalidParameter
	}

	public class IrcCommandException : Exception
	{
		public IrcCommandException(IrcCommandExceptionType type)
		{
			this.type = type;
		}

		public IrcCommandException(IrcCommandExceptionType type, string associatedString)
		{
			this.type = type;
			this.associatedString = associatedString;
		}

		public IrcCommandExceptionType Type
		{
			get
			{
				return type;
			}
		}

		public string AssociatedString
		{
			get
			{
				return associatedString;
			}
		}

		private IrcCommandExceptionType type;
		private string associatedString = null;
	}
}

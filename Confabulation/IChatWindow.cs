using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Confabulation.Chat;

namespace Confabulation
{
	public interface IChatWindow
	{
		IrcConnection Connection
		{
			get;
		}
	}
}

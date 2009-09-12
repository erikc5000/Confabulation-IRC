using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Confabulation.Chat
{
	public class IrcNameComparer : IEqualityComparer<string>
	{
		public IrcNameComparer(IrcServerProperties properties)
		{
			this.properties = properties;
		}

		public bool Equals(string x, string y)
		{
			return Irc.NamesEqual(x, y, properties);
		}

		public int GetHashCode(string obj)
		{
			if (obj == null)
				throw new ArgumentNullException("obj");

			return obj.GetHashCode();
		}

		IrcServerProperties properties;
	}
}

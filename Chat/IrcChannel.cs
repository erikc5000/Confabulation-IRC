using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Confabulation.Chat
{
    public class IrcChannel
    {
		public IrcChannel(byte[] name)
		{
			if (name == null)
				throw new ArgumentNullException("name");

			this.name = name;
		}

		public byte[] Name
		{
			get
			{
				return name;
			}
		}

		private byte[] name;
		private List<IrcUser> users;
    }
}

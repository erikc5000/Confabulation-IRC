using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Confabulation.Chat
{
    public class IrcChannel
    {
		public IrcChannel(string name)
		{
			if (name == null)
				throw new ArgumentNullException("name");

			this.Name = name;
		}

		public string Name
		{
			get;
			private set;
		}

		public List<IrcUser> Users
		{
			get
			{
				return users;
			}
		}

		private List<IrcUser> users = new List<IrcUser>();
    }
}

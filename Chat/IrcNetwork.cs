using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Confabulation.Chat
{
	public class IrcNetwork
	{
		public IrcNetwork()
		{
			Name = null;
		}

		public IrcServer GetFirstServer()
		{
			if (servers.Count == 0)
				return null;

			return servers.First();
		}

		public IrcServer GetNextServer(IrcServer currentServer)
		{
			if (servers.Count == 0)
				return null;

			int currentIndex = servers.IndexOf(currentServer);

			if (currentIndex < 0 || currentIndex == servers.Count - 1)
				return servers.First();

			return servers[currentIndex + 1];
		}

		public string Name
		{
			get;
			set;
		}

		public List<IrcServer> Servers
		{
			get
			{
				return servers;
			}
		}

		private List<IrcServer> servers = new List<IrcServer>();
	}
}

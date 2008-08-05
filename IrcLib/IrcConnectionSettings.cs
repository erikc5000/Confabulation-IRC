using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Confabulation.Chat
{
	public class UserSettings
	{
		public UserSettings()
		{
			UserName = null;
			RealName = null;
		}

		public List<string> Nicknames
		{
			get
			{
				return nicknames;
			}
			set
			{
				if (value == null)
					throw new ArgumentNullException("value");

				nicknames = value;
			}
		}

		public string UserName
		{
			get;
			set;
		}

		public string RealName
		{
			get;
			set;
		}

		private List<string> nicknames = new List<string>();
	}

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

	public class IrcServer
	{
		public IrcServer()
		{
			Description = null;
			Hostname = null;
			Port = Irc.DefaultServerPort;
			Password = null;
			Network = null;
		}

		public string Description
		{
			get;
			set;
		}

		public string Hostname
		{
			get;
			set;
		}

		public int Port
		{
			get;
			set;
		}

		public string Password
		{
			get;
			set;
		}

		public IrcNetwork Network
		{
			get;
			set;
		}
	}

	public class IrcConnectionSettings
	{
		public IrcConnectionSettings()
		{
			Name = null;
			InititialUserModes = InitialUserMode.Invisible;
			TryOtherServersInNetwork = true;
		}

		public string Name
		{
			get;
			set;
		}

		public UserSettings User
		{
			get
			{
				return userSettings;
			}
		}

		public IrcServer Server
		{
			get
			{
				return server;
			}
		}

		public InitialUserMode InititialUserModes
		{
			get;
			set;
		}

		public bool TryOtherServersInNetwork
		{
			get;
			set;
		}

		private UserSettings userSettings = new UserSettings();
		private IrcServer server;
	}
}

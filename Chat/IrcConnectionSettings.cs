using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Confabulation.Chat
{
	public class IrcUserSettings
	{
		public IrcUserSettings()
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

	public class IrcConnectionSettings
	{
		public IrcConnectionSettings()
		{
			Name = null;
			Server = null;
			InitialUserModes = InitialUserMode.Invisible;
			TryOtherServersInNetwork = true;
		}

		public string Name
		{
			get;
			set;
		}

		public IrcUserSettings User
		{
			get
			{
				return userSettings;
			}
		}

		public IrcServer Server
		{
			get;
			set;
		}

		public InitialUserMode InitialUserModes
		{
			get;
			set;
		}

		public bool TryOtherServersInNetwork
		{
			get;
			set;
		}

		private IrcUserSettings userSettings = new IrcUserSettings();
	}
}

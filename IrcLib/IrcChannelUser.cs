using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Confabulation.Chat
{
	//[FlagsAttribute]
	//public enum IrcUserChannelMode
	//{
	//    None = 0,
	//    Operator = 1,
	//    HalfOperator = 2,
	//    Voice = 4
	//}

	public class IrcChannelUser
	{
		public IrcChannelUser(IrcUser user)
		{
			if (user == null)
				throw new ArgumentNullException("user");

			this.user = user;
		}

		public IrcChannelUser(IrcUser user, params char[] modes)
		{
			if (user == null)
				throw new ArgumentNullException("user");

			this.user = user;

			if (modes != null)
			{
				foreach (char c in modes)
					this.modes.Add(c);
			}
		}

		public bool IsOperator()
		{
			return modes.Contains('o');
		}

		public bool IsHalfOperator()
		{
			return modes.Contains('h');
		}

		public bool HasVoice()
		{
			return modes.Contains('v');
		}

		//public IrcUserChannelMode Modes
		//{
		//    get;
		//    private set;
		//}

		public string Nickname
		{
			get
			{
				return user.Nickname;
			}
		}

		public string UserName
		{
			get
			{
				return user.UserName;
			}
		}

		public string Hostname
		{
			get
			{
				return user.Hostname;
			}
		}

		internal IrcUser GetUser()
		{
			return user;
		}

		internal void AddMode(char mode)
		{
			modes.Add(mode);
		}

		internal void RemoveMode(char mode)
		{
			modes.Remove(mode);
		}

		private IrcUser user;
		private HashSet<char> modes = new HashSet<char>();
	}
}

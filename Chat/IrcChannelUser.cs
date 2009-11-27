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
			user.NicknameChanged += new EventHandler<IrcUserEventArgs>(ProcessNicknameChanged);
		}

		public IrcChannelUser(IrcUser user, params char[] modes)
		{
			if (user == null)
				throw new ArgumentNullException("user");

			this.user = user;
			user.NicknameChanged += new EventHandler<IrcUserEventArgs>(ProcessNicknameChanged);

			if (modes != null)
			{
				foreach (char c in modes)
					this.modes.Add(c);
			}
		}

		//public override bool Equals(object obj)
		//{
		//    if (obj == null)
		//        return false;

		//    if (obj is IrcChannelUser)
		//        return user.Equals(((IrcChannelUser)obj).user);

		//    return user.Equals(obj);
		//}

		public bool Equals(string nickname)
		{
			return user.Equals(nickname);
		}

		public bool IsOperator
		{
			get
			{
				return modes.Contains('o');
			}
		}

		public bool IsHalfOperator
		{
			get
			{
				return modes.Contains('h');
			}
		}

		public bool HasVoice
		{
			get
			{
				return modes.Contains('v');
			}
		}

		public IrcUser GetUser()
		{
			return user;
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

		public bool IsSelf
		{
			get
			{
				return user.IsSelf;
			}
		}

		public event EventHandler<IrcUserEventArgs> NicknameChanged;

		internal void AddMode(char mode)
		{
			if (!modes.Contains(mode))
				modes.Add(mode);
		}

		internal void RemoveMode(char mode)
		{
			modes.Remove(mode);
		}

		private void ProcessNicknameChanged(object sender, IrcUserEventArgs e)
		{
			EventHandler<IrcUserEventArgs> handler = NicknameChanged;

			if (handler != null)
				handler(this, e);
		}

		private IrcUser user;
		private List<char> modes = new List<char>();
	}
}

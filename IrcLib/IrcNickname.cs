using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Confabulation.Chat
{
	public class IrcNickname
	{
		public IrcNickname(byte[] nickname)
		{
			this.nickname = nickname;
		}

		public override bool Equals(object obj)
		{
			if (obj is IrcNickname)
				return Equals(obj as string);

			return false;
		}

		public bool Equals(IrcNickname other)
		{
			return false;
		}

		private byte[] nickname;
	}
}

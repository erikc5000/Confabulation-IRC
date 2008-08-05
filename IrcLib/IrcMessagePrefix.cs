using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Confabulation.Chat
{
	public class IrcMessagePrefix
	{
		public IrcMessagePrefix(byte[] prefix)
		{
			if (prefix == null)
				throw new ArgumentNullException("prefix");

			
			this.prefix = prefix;
		}

		public byte[] ExtractNickname()
		{
			for (int i = 0; i < prefix.Length; i++)
			{
				if (prefix[i] == (byte)'!' || prefix[i] == (byte)'@')
				{
					byte[] nickname = new byte[i];
					Array.Copy(prefix, nickname, i);
					return nickname;
				}

				// The presence of a "." implies this prefix is a server
				// instead of a user.
				if (prefix[i] == (byte)'.')
					return null;
			}

			return prefix;
		}

		public byte[] ExtractUserName()
		{
			/*int start_index = -1;

			for (int i = 0; i < prefix.Length; i++)
			{
				if (prefix[i] == (byte)'@')
					return null;

				if (prefix[i] == (byte)'!')
				{
					start_index = i + 1;
					break;
				}
				{
					byte[] nickname = new byte[i];

					for (int j = 0; j < i; j++)
						nickname[j] = prefix[j];

					return nickname;
				}
			}*/

			return prefix;
		}

		public byte[] Text
		{
			get
			{
				return prefix;
			}
		}

		private byte[] prefix;
	}
}

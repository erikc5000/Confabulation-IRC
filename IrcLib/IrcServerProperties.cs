using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Confabulation.Chat
{
	public enum IrcCaseMapping
	{
		Ascii,
		Rfc1459,
		StrictRfc1459
	}

	public enum IrcChannelModeType
	{
		AddressRequired,
		ParameterRequired,
		ParameterRequiredOnSet,
		NoParameter
	}

	public class IrcServerProperties
	{
		public IrcServerProperties()
		{
		}

		internal void Add(string property, string value)
		{
			if (property == null)
				throw new ArgumentNullException("property");

			properties[property] = value;
		}

		internal void Clear()
		{
			properties.Clear();
			channelTypes = null;
			channelModes = null;
		}

		public int MaxChannelNameLength
		{
			get
			{
				if (properties.ContainsKey("CHANNELLEN"))
				{
					string length = properties["CHANNELLEN"];

					if (length == null)
						return 0;
					else
						return int.Parse(length);
				}

				return 200;
			}
		}

		public int MaxKickMessageLength
		{
			get
			{
				if (properties.ContainsKey("KICKLEN"))
				{
					string length = properties["KICKLEN"];

					if (length != null)
						return int.Parse(length);
				}

				return 0;
			}
		}

		public int MaxNicknameLength
		{
			get
			{
				if (properties.ContainsKey("NICKLEN"))
				{
					string length = properties["NICKLEN"];

					if (length == null)
						return 0;
					else
						return int.Parse(length);
				}

				return 9;
			}
		}

		public int MaxTopicLength
		{
			get
			{
				if (properties.ContainsKey("TOPICLEN"))
				{
					string length = properties["TOPICLEN"];

					if (length != null)
						return int.Parse(length);
				}

				return 0;
			}
		}

		public string NetworkName
		{
			get
			{
				if (properties.ContainsKey("NETWORK"))
					return properties["NETWORK"];

				return null;
			}
		}

		public IrcCaseMapping CaseMapping
		{
			get
			{
				if (properties.ContainsKey("CASEMAPPING"))
				{
					string mapping = properties["CASEMAPPING"];

					if (mapping.Equals("ascii", StringComparison.OrdinalIgnoreCase))
						return IrcCaseMapping.Ascii;
					else if (mapping.Equals("strict-rfc1459", StringComparison.OrdinalIgnoreCase))
						return IrcCaseMapping.StrictRfc1459;
				}

				return IrcCaseMapping.Rfc1459;
			}
		}

		public List<char> ChannelTypes
		{
			get
			{
				if (channelTypes == null)
				{
					channelTypes = new List<char>();

					if (properties.ContainsKey("CHANTYPES"))
					{
						string typeString = properties["CHANTYPES"];

						foreach (char c in typeString)
							channelTypes.Add(c);
					}
					else
					{
						channelTypes.Add('#');
						channelTypes.Add('&');
					}
				}

				return channelTypes;
			}
		}

		public bool SupportsBanExceptions
		{
			get
			{
				if (properties.ContainsKey("EXCEPTS"))
					return true;

				return false;
			}
		}

		public char BanExceptionChannelMode
		{
			get
			{
				if (!properties.ContainsKey("EXCEPTS"))
					throw new InvalidOperationException("Ban exceptions are not supported");

				string modeString = properties["EXCEPTS"];

				if (modeString == null)
					return 'e';

				return modeString.ToCharArray().First();
			}
		}

		public bool SupportsInviteExceptions
		{
			get
			{
				if (properties.ContainsKey("INVEX"))
					return true;

				return false;
			}
		}

		public char InviteExceptionChannelMode
		{
			get
			{
				if (!properties.ContainsKey("INVEX"))
					throw new InvalidOperationException("Invite exceptions are not supported");

				string modeString = properties["INVEX"];

				if (modeString == null)
					return 'I';

				return modeString.ToCharArray().First();
			}
		}

		public int MaxVariableModes
		{
			get
			{
				if (!properties.ContainsKey("MODES"))
					return 3;

				string modesString = properties["MODES"];

				if (modesString == null)
					return 0;

				try
				{
					return int.Parse(modesString);
				}
				catch (FormatException)
				{
					return 3;
				}
			}
		}

		public char GetModeFromPrefix(char prefix)
		{
			//if (prefixMap == null)
			//{

			//}

			return '\0';
		}

		public List<char> GetChannelModes(IrcChannelModeType type)
		{
			// Construct channel modes
			if (channelModes == null)
			{
				channelModes = new Dictionary<IrcChannelModeType, List<char>>();
				channelModes[IrcChannelModeType.AddressRequired] = new List<char>();
				channelModes[IrcChannelModeType.ParameterRequired] = new List<char>();
				channelModes[IrcChannelModeType.ParameterRequiredOnSet] = new List<char>();
				channelModes[IrcChannelModeType.NoParameter] = new List<char>();

				if (properties.ContainsKey("CHANMODES"))
				{
					string modeString = properties["CHANMODES"];
					string[] modes = modeString.Split(',');

					if (modes.Length >= 1)
					{
						foreach (char c in modes[0])
							channelModes[IrcChannelModeType.AddressRequired].Add(c);
					}

					if (modes.Length >= 2)
					{
						foreach (char c in modes[1])
							channelModes[IrcChannelModeType.ParameterRequired].Add(c);
					}

					if (modes.Length >= 3)
					{
						foreach (char c in modes[2])
							channelModes[IrcChannelModeType.ParameterRequiredOnSet].Add(c);
					}

					if (modes.Length >= 4)
					{
						foreach (char c in modes[3])
							channelModes[IrcChannelModeType.NoParameter].Add(c);
					}
				}
			}

			return channelModes[type];
		}

		public bool SupportsChannelType(char type)
		{
			foreach (char t in channelTypes)
			{
				if (t == type)
					return true;
			}

			return false;
		}

		private List<char> channelTypes = null;
		private Dictionary<IrcChannelModeType, List<char>> channelModes = null;
		private Dictionary<string, string> properties = new Dictionary<string,string>();
	}
}

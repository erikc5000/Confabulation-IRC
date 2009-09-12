using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Confabulation.Chat
{
	public class IrcServer
	{
		public IrcServer()
		{
			Provider = null;
			Details = null;
			Hostname = null;
			Password = null;
			Network = null;
		}

		public string Provider
		{
			get;
			set;
		}

		public string Details
		{
			get;
			set;
		}

		public string Hostname
		{
			get;
			set;
		}

		public string Ports
		{
			get
			{
				return ports;
			}
			set
			{
				ports = value;
				portsDirty = true;
			}
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

		public int GetFirstPort()
		{
			List<int> portList = GetProcessedPorts();

			if (portList.Count == 0)
				return Irc.DefaultServerPort;

			return portList.First();
		}

		public List<int> GetProcessedPorts()
		{
			if (!portsDirty)
				return processedPorts;

			processedPorts = new List<int>();

			string[] separatedPorts = ports.Split(new char[] { ',', ';' },
			                                      StringSplitOptions.RemoveEmptyEntries);

			foreach (string separatedPort in separatedPorts)
			{
				if (separatedPort.Contains('-'))
				{
					string[] range = separatedPort.Split('-');

					if (range.Length != 2)
						continue;

					try
					{
						int startPort = int.Parse(range[0].Trim());
						int endPort = int.Parse(range[1].Trim());

						if (endPort >= startPort)
						{
							for (int i = startPort; i <= endPort; i++)
								processedPorts.Add(i);
						}
					}
					catch (Exception)
					{
					}
				}
				else
				{
					try
					{
						int port = int.Parse(separatedPort.Trim());
						processedPorts.Add(port);
					}
					catch (Exception)
					{
					}
				}
			}

			portsDirty = false;

			return processedPorts;
		}

		string ports = "";
		private List<int> processedPorts = null;
		bool portsDirty = true;
	}
}

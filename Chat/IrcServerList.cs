using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Confabulation.Chat
{
	public class IrcServerList
	{
		public IrcServerList()
		{
		}

		public IrcServerList(string serversFile)
		{
			if (serversFile == null)
				throw new ArgumentNullException("serversFile");

			XmlReaderSettings settings = new XmlReaderSettings();
			settings.ProhibitDtd = false;
			settings.ValidationType = ValidationType.DTD;

			XmlReader reader = XmlReader.Create(serversFile, settings);
			reader.Read();
			reader.ReadStartElement("Servers");

			while (reader.Read())
			{
				if (!reader.IsStartElement())
					continue;

				if (!reader.Name.Equals("Network"))
					continue;
				
				IrcNetwork newNetwork = new IrcNetwork();
				networks.Add(newNetwork);
				newNetwork.Name = reader.GetAttribute("Name");

				while (reader.Read())
				{
					if (!reader.IsStartElement())
						continue;

					if (!reader.Name.Equals("Server"))
						continue;

					IrcServer newServer = new IrcServer();
					servers.Add(newServer);
					newNetwork.Servers.Add(newServer);

					newServer.Network = newNetwork;
					newServer.Hostname = reader.GetAttribute("Hostname");
					newServer.Ports = reader.GetAttribute("Ports");

					break;
				}

			}
		}

		public IEnumerable<IrcNetwork> Networks
		{
			get
			{
				return networks;
			}
		}

		private List<IrcNetwork> networks = new List<IrcNetwork>();
		private List<IrcServer> servers = new List<IrcServer>();
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Confabulation.Chat
{
	public class IrcServerList
	{
		public IrcServerList(string serversFile)
		{
			if (serversFile == null)
				throw new ArgumentNullException("serversFile");

			XmlReaderSettings settings = new XmlReaderSettings();
			settings.ProhibitDtd = false;
			settings.ValidationType = ValidationType.DTD;

			XmlReader reader = XmlReader.Create(serversFile, settings);
			//reader.MoveToContent();
			reader.Read();
			reader.ReadStartElement("servers");

			while (reader.Read())
			{
				if (!reader.IsStartElement())
					continue;

				if (!reader.Name.Equals("network"))
					continue;
				
				IrcNetwork newNetwork = new IrcNetwork();
				networks.Add(newNetwork);
				newNetwork.Name = reader.GetAttribute("name");

				while (reader.Read())
				{
					if (!reader.IsStartElement())
						continue;

					if (!reader.Name.Equals("server"))
						continue;

					IrcServer newServer = new IrcServer();
					servers.Add(newServer);
					newNetwork.Servers.Add(newServer);

					newServer.Network = newNetwork;
					newServer.Hostname = reader.GetAttribute("hostname");
					newServer.Ports = reader.GetAttribute("ports");
				}

			}
		}

		public List<IrcNetwork> Networks
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

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.IO;
using System.Xml.Linq;
using Confabulation.Chat;

namespace Confabulation
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
		App() : base()
		{
			Initialize();
		}

		public static string GetUserFolder()
		{
			string userFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			userFolder += "\\Confabulation";

			return userFolder;
		}

		public static string GetUserServersFile()
		{
			return GetUserFolder() + "\\Servers.xml";
		}

		private void Initialize()
		{
			string userFolder = GetUserFolder();

			try
			{
				if (!Directory.Exists(userFolder))
					Directory.CreateDirectory(userFolder);
			}
			catch (Exception)
			{
				// TODO: Maybe some more sophisticated handling
			}

			string serversFile = GetUserServersFile();
			string serversDtd = GetUserFolder() + "\\Servers.dtd";

			try
			{
				if (!File.Exists(serversFile))
					File.Copy("Servers.xml", serversFile);

				if (!File.Exists(serversDtd))
					File.Copy("Servers.dtd", serversDtd);

				serverList = new IrcServerList(GetUserServersFile());
				//serverList = XDocument.Load(GetUserServersFile());;
			}
			catch (Exception)
			{
				// TODO: Maybe some more sophisticated handling
			}
		}

		public event EventHandler<ConnectionEventArgs> ConnectionAdded;
		public event EventHandler<ConnectionEventArgs> ConnectionRemoved;

		public void AddConnection(IrcConnection connection)
		{
			connections.Add(connection);

			EventHandler<ConnectionEventArgs> handler = ConnectionAdded;

			if (handler != null)
				handler(this, new ConnectionEventArgs(connection));
		}

		public void RemoveConnection(IrcConnection connection)
		{
			connections.Remove(connection);

			EventHandler<ConnectionEventArgs> handler = ConnectionRemoved;

			if (handler != null)
				handler(this, new ConnectionEventArgs(connection));
		}

		public IrcServerList ServerList
		{
			get
			{
				return serverList;
			}
		}

		public IEnumerable<IrcConnection> Connections
		{
			get
			{
				return connections;
			}
		}

		private IrcServerList serverList = null;
		private List<IrcConnection> connections = new List<IrcConnection>();
    }
}

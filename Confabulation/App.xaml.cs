﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.IO;
using System.Xml.Linq;
using System.Collections.ObjectModel;
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
				Log.WriteLine("Failed to create the Confabulation user folder");
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
			}
			catch (Exception)
			{
				// TODO: Maybe some more sophisticated handling
				Log.WriteLine("Failed to copy data to the Confabulation user folder");
				serverList = new IrcServerList();
			}
		}

		public event EventHandler<ConnectionEventArgs> ConnectionAdded;
		public event EventHandler<ConnectionEventArgs> ConnectionRemoved;

		public void AddConnection(IrcConnection connection)
		{
			connections.Add(new ConnectionItem(connection));

			EventHandler<ConnectionEventArgs> handler = ConnectionAdded;

			if (handler != null)
				handler(this, new ConnectionEventArgs(connection));
		}

		public void RemoveConnection(IrcConnection connection)
		{
			foreach (ConnectionItem item in connections)
			{
				if (item.Connection == connection)
				{
					connections.Remove(item);
					break;
				}
			}

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

		public IEnumerable<ConnectionItem> Connections
		{
			get
			{
				return connections;
			}
		}

		private IrcServerList serverList = null;
		private ObservableCollection<ConnectionItem> connections = new ObservableCollection<ConnectionItem>();
    }
}

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
		static App()
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

		private static void Initialize()
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

		public static IrcServerList ServerList
		{
			get
			{
				return serverList;
			}
		}

		private static IrcServerList serverList = null;
    }
}

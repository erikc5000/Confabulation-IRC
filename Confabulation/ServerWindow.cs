using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Confabulation;
using Confabulation.Chat;
using Confabulation.Chat.Commands;

namespace Confabulation
{
	public class ServerWindow : ChatWindow
	{
		public ServerWindow(IrcConnection connection) : base()
		{
            InitializeComponent();

            this.connection = connection;
            connection.StateChanged += new EventHandler<IrcConnectionEventArgs>(connection_StateChanged);
            connection.RawMessageReceived += new EventHandler<IrcMessageEventArgs>(connection_RawMessageReceived);
		}

        protected override void TextEntered(string text)
        {
            if (Connection != null)
            {
                try
                {
                    Connection.Execute(IrcCommand.Parse(text));
                }
                catch (IrcCommandException)
                {
                    AddTextToWindow("*Invalid command*");
                }
                catch (ArgumentException ae)
                {
                    AddTextToWindow("*Invalid argument*: " + ae.ParamName);
                }
            }
            else
            {
                AddTextToWindow("*Not connected*");
            }
        }

        private void connection_StateChanged(object sender, IrcConnectionEventArgs e)
        {
            switch (e.EventType)
            {
                case IrcConnectionEventType.Connected:
                    chatLogDocument.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new AddToWindowDelegate(AddTextToWindow), (Object)"*Connected*");
                    break;

                case IrcConnectionEventType.ConnectFailed:
                    chatLogDocument.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new AddToWindowDelegate(AddTextToWindow), (Object)"*Connection Failed*");
                    break;

                case IrcConnectionEventType.Disconnected:
                    chatLogDocument.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new AddToWindowDelegate(AddTextToWindow), (Object)"*Disconnected*");
                    break;
            }
        }

        private void connection_RawMessageReceived(object sender, IrcMessageEventArgs e)
        {
            chatLogDocument.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                new AddToWindowDelegate(AddTextToWindow), (Object)e.Message.ToString());
        }

        public override IrcConnection Connection
        {
            get
            {
                return connection;
            }
        }

        private IrcConnection connection = null;
	}
}

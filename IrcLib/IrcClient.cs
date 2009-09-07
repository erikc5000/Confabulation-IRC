using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Confabulation.Chat
{
	/// <summary>
	/// The current state of the IRC client
	/// </summary>
	public enum IrcConnectionState
	{
		Disconnected,
		Resolving,
		Connecting,
		Connected
	}

	/// <summary>
	/// Used to control the priority when sending a message
	/// </summary>
	public enum MessagePriority
	{
		Low,
		Normal,
		High,
		Critical
	}

    public class IrcClient
    {
		/// <summary>
		/// Create an IRC client.
		/// </summary>
		/// <param name="server">The host name or IP address of the server</param>
        public IrcClient(string hostname)
        {
			if (hostname == null)
				throw new ArgumentNullException("server");

			Initialize(hostname, Irc.DefaultServerPort);
        }

		/// <summary>
		/// Create an IRC client.
		/// </summary>
		/// <param name="server">The host name or IP address of the server</param>
		/// <param name="port">The port number</param>
        public IrcClient(string hostname, int port)
        {
			if (hostname == null)
				throw new ArgumentNullException("server");

			Initialize(hostname, port);
        }

		~IrcClient()
		{
			Disconnect();
		}

		public void Connect()
		{
			lock (syncObject)
			{
				if (connectionState != IrcConnectionState.Disconnected)
					throw new InvalidOperationException("Already connecting/connected.  You must disconnect first.");

				connectionState = IrcConnectionState.Resolving;

				try
				{
					lock (hostnamePortSyncObject)
					{
						IAsyncResult result = Dns.BeginGetHostAddresses(hostname, resolveCallback, null);
					}
				}
				catch (Exception e)
				{
					if (e is ArgumentOutOfRangeException || e is ArgumentException
						|| e is SocketException)
					{
						connectionState = IrcConnectionState.Disconnected;
						OnConnectEvent(new IrcConnectionEventArgs(IrcConnectionEventType.ResolveFailed));
					}
					else
					{
						throw;
					}
				}
			}
		}

        public void Disconnect()
        {
			lock (syncObject)
			{
				if (connectionState == IrcConnectionState.Disconnected)
					return;

				if (socket != null)
				{
					try
					{
						socket.Shutdown(SocketShutdown.Both);
					}
					catch (Exception)
					{
					}

					socket.Close();
					socket = null;
				}

				connectionState = IrcConnectionState.Disconnected;

				OnConnectEvent(new IrcConnectionEventArgs(IrcConnectionEventType.Disconnected));
			}
        }

		/// <summary>
		/// Send a message to the server with normal priority
		/// </summary>
		/// <param name="message">The message to send</param>
		public void Send(IrcMessage message)
		{
			Send(message, MessagePriority.Normal);
		}

		/// <summary>
		/// Send a message to the server
		/// </summary>
		/// <param name="message">The message to send</param>
		/// <param name="priority">The priority of the message</param>
        public void Send(IrcMessage message, MessagePriority priority)
        {
			if (message == null)
				throw new ArgumentNullException("message");

			lock (syncObject)
			{
				if (connectionState != IrcConnectionState.Connected)
					throw new InvalidOperationException("A message cannot be sent before establishing a connection.");

				byte[] byteMessage = message.ToByteArray();

				SocketAsyncEventArgs sendEventArgs = new SocketAsyncEventArgs();
				sendEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(ProcessSend);
				sendEventArgs.SetBuffer(byteMessage, 0, byteMessage.Length);

				if (!socket.SendAsync(sendEventArgs))
					ProcessSend(socket, sendEventArgs);
			}
        }

		public string Hostname
		{
			get
			{
				lock (hostnamePortSyncObject)
				{
					return hostname;
				}
			}
			set
			{
				lock (hostnamePortSyncObject)
				{
					hostname = value;
				}
			}
		}

		public int Port
		{
			get
			{
				lock (hostnamePortSyncObject)
				{
					return port;
				}
			}
			set
			{
				lock (hostnamePortSyncObject)
				{
					port = value;
				}
			}
		}

		public IrcConnectionState ConnectionState
		{
			get
			{
				lock (syncObject)
				{
					return connectionState;
				}
			}
		}

		public event EventHandler<IrcConnectionEventArgs> ConnectionEvent;
		public event EventHandler<IrcMessageEventArgs> MessageReceived;

		private void Initialize(string hostname, int port)
		{
			this.hostname = hostname;
			this.port = port;

			resolveCallback = new AsyncCallback(OnResolveComplete);
			connectEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(ProcessConnect);
			receiveEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(ProcessReceive);
		}

        private void OnResolveComplete(IAsyncResult result)
        {
			lock (syncObject)
			{
				if (connectionState != IrcConnectionState.Resolving)
					return;

				IPAddress[] addresses = null;

				try
				{
					addresses = Dns.EndGetHostAddresses(result);
				}
				catch (SocketException)
				{
					addresses = new IPAddress[0];
				}

				if (addresses.Length == 0)
				{
					connectionState = IrcConnectionState.Disconnected;
					OnConnectEvent(new IrcConnectionEventArgs(IrcConnectionEventType.ResolveFailed));
					return;
				}

				IPAddress ip = addresses.First();
				connectionState = IrcConnectionState.Connecting;
				OnConnectEvent(new IrcConnectionEventArgs(IrcConnectionEventType.ResolveSucceeded));

				try
				{
					socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
					connectEventArgs.RemoteEndPoint = new IPEndPoint(ip, port);

					if (!socket.ConnectAsync(connectEventArgs))
						ProcessConnect(socket, connectEventArgs);
				}
				catch (Exception)
				{
					// Handle specific types
				}
			}
        }

        private void ProcessConnect(object sender, SocketAsyncEventArgs args)
        {
			lock (syncObject)
			{
				if (connectionState != IrcConnectionState.Connecting)
					return;

				if (args.SocketError != SocketError.Success)
				{
					connectionState = IrcConnectionState.Disconnected;
					OnConnectEvent(new IrcConnectionEventArgs(IrcConnectionEventType.ConnectFailed));
					return;
				}

				connectionState = IrcConnectionState.Connected;
				OnConnectEvent(new IrcConnectionEventArgs(IrcConnectionEventType.Connected));
			}

			InitiateReceive();
        }

		private void ProcessSend(object sender, SocketAsyncEventArgs args)
		{
			if (args.BytesTransferred <= 0 || args.SocketError != SocketError.Success)
			{
				Disconnect();
				return;
			}
		}

		private void ProcessReceive(object sender, SocketAsyncEventArgs args)
		{
			lock (syncObject)
			{
				if (connectionState != IrcConnectionState.Connected)
					return;
			}

			if (args.BytesTransferred <= 0 || args.SocketError != SocketError.Success)
			{
				Disconnect();
				return;
			}

			int offset = 0;
			int length = 0;

			for (int i = 0; i < args.BytesTransferred; i++)
			{
				byte currentByte = args.Buffer[i];

				// Check for end of message
				if (currentByte == (byte)'\r' || currentByte == (byte)'\n')
				{
					if (currentMessageLength == 0 && length == 0)
					{
						offset++;
					}
					else
					{
						// End characters have been passed, so we can create a new message object
						IrcMessage message;

						// If there is a saved message fragment, it needs to be merged with the other half
						if (currentMessageLength > 0)
						{
							if (length > 0)
							{
								Array.Copy(args.Buffer, offset, currentMessage, currentMessageLength, length);
								currentMessageLength += length;
							}

							message = new IrcMessage(currentMessage, 0, currentMessageLength);
							currentMessageLength = 0;
						}
						else
						{
							message = new IrcMessage(args.Buffer, offset, length);
						}

						OnMessageReceived(new IrcMessageEventArgs(message));

						// Reset variables for the next message.  Note that length will
						// be incremented afterward since the first character of the new
						// message has already been reached.
						offset += length + 1;
						length = 0;
					}
				}
				else
				{
					length++;
				}
			}

			if (length > 0)
			{
				Array.Copy(args.Buffer, offset, currentMessage, 0, length);
				currentMessageLength = length;
			}

			// Begin another receive operation
			InitiateReceive();
		}

		private void InitiateReceive()
		{
			receiveEventArgs.SetBuffer(receiveBuffer, 0, receiveBuffer.Length);

			if (!socket.ReceiveAsync(receiveEventArgs))
				ProcessReceive(socket, receiveEventArgs);
		}

		private void OnConnectEvent(IrcConnectionEventArgs args)
		{
			EventHandler<IrcConnectionEventArgs> handler = ConnectionEvent;

			if (handler != null)
				handler(this, args);
		}

		private void OnMessageReceived(IrcMessageEventArgs args)
		{
			EventHandler<IrcMessageEventArgs> handler = MessageReceived;

			if (handler != null)
				handler(this, args);
		}

        private string hostname;
        private int port;
		private IrcConnectionState connectionState = IrcConnectionState.Disconnected;
        private Socket socket = null;
		private AsyncCallback resolveCallback;
		private SocketAsyncEventArgs connectEventArgs = new SocketAsyncEventArgs();
		private SocketAsyncEventArgs receiveEventArgs = new SocketAsyncEventArgs();
		//private SocketAsyncEventArgs sendEventArgs = new SocketAsyncEventArgs();
		private byte[] receiveBuffer = new byte[512];
		private byte[] currentMessage = new byte[512];
		private int currentMessageLength = 0;
		private Object syncObject = new Object();
		private Object hostnamePortSyncObject = new Object();
    }
}

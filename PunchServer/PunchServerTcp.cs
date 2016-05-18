using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.IO;
using System.Diagnostics;
using SimpleTCP;

namespace PunchServer
{
	public class PunchServerTcp
	{
		public PunchServerTcp(int port)
		{
			_port = port;
			_connectionIndex = 0;

			_server = new SimpleTcpServer();
			_server.ClientConnected += _server_ClientConnected;
			_server.ClientDisconnected += _server_ClientDisconnected;
			_server.DelimiterDataReceived += _server_DelimiterDataReceived;
			_server.DataReceived += _server_DataReceived;
		}

		internal void Start()
		{
			if (_server.IsStarted)
				throw new NotSupportedException("PunchServerTcp is already started.");

			_server.Start(_port);
			DebugInfo("Server started");
		}

		internal void Stop()
		{
			if (!_server.IsStarted)
				throw new NotSupportedException("PunchServerTcp is not started.");

			_server.Stop();
			DebugInfo("Server stopped.");
		}

		private void _server_ClientConnected(object sender, TcpClient e)
		{
			//e.Client.Tag = new ClientMetaData { ConnectionIndex = ++_connectionIndex };
			DebugInfo("{0}: Client connected from {1}", 0, endPointOf(e));//metaDataOf(e).ConnectionIndex

			tryPairClients();
			DebugInfo("Connected listener done");
		}

		private void _server_ClientDisconnected(object sender, TcpClient e)
		{
			DebugInfo("{0}: Client disconnected", 0); //metaDataOf(e).ConnectionIndex
		}

		private void _server_DelimiterDataReceived(object sender, Message e)
		{
			DebugInfo("{0}: Server received delimited {1}", 0, e.MessageString);
			//var metaData = metaDataOf(e);

			var parts = e.MessageString.Split(new[] { ' ' }, 2);
			var verb = parts[0];

			switch (verb)
			{
				case "EndPoint":
					//metaData.EndPoint = parts[1];
					DebugInfo("{0}: EndPoint = \"{1}\"", 0, parts[1]);//metaData.ConnectionIndex
					break;

				default:
					DebugInfo("{0}: Ignoring message \"{1}\"", 0, e.MessageString);//metaData.ConnectionIndex
					break;
			}
		}

		private void _server_DataReceived(object sender, Message e)
		{
			DebugInfo("{0}: Server received {1}", 0, e.MessageString); //metaDataOf(e).ConnectionIndex
		}



		private void tryPairClients()
		{
			if (_server.ConnectedClientsCount == 2)
			{
				var client1 = _server.ConnectedClients.First();
				var client2 = _server.ConnectedClients.Skip(1).First();
				
				var client1Endpoint = endPointOf(client1);
				var client2Endpoint = endPointOf(client2);

				send(client1, client2Endpoint.ToString());
				send(client2, client1Endpoint.ToString());
				DebugInfo("Sent messages pairing clients");
			}
		}

		static IPEndPoint endPointOf(TcpClient client)
		{
			return client.Client.RemoteEndPoint as IPEndPoint;
		}

		static void send(TcpClient client, string message)
		{
			var buffer = Encoding.UTF8.GetBytes(message + Encoding.UTF8.GetString(new byte[] { 0x13 }));
			client.Client.Send(buffer);
			//var stream = client.GetStream();
			//var sw = new StreamWriter(stream);
			//sw.Write(message + Encoding.UTF8.GetString(new byte[] { 0x13 }));
		}

		//private ClientMetaData metaDataOf(TcpClientInfo client)
		//{
		//	return client.Tag as ClientMetaData;
		//}

		#region Debug logging

		[System.Diagnostics.Conditional("DEBUG")]
		void DebugInfo(string format, params object[] args)
		{
			if (_debugInfoTime == null)
			{
				_debugInfoTime = new System.Diagnostics.Stopwatch();
				_debugInfoTime.Start();
			}
			System.Diagnostics.Debug.WriteLine(_debugInfoTime.ElapsedMilliseconds + ": " + format, args);
		}
		System.Diagnostics.Stopwatch _debugInfoTime;

		#endregion Debug logging




		readonly SimpleTcpServer _server;
		readonly int _port;
		int _connectionIndex;
	}
}

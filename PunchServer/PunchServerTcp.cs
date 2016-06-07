using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Diagnostics;
using SocketMessaging.Server;
using SocketMessaging;

namespace PunchServer
{
	public class PunchServerTcp
	{
		public PunchServerTcp(int port)
		{
			_port = port;
			_server = new TcpServer();
            _server.Connected += _server_Connected;
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

        private void _server_Connected(object sender, ConnectionEventArgs e)
        {
            DebugInfo("{0}: Client connected from {1}", e.Connection.Id, endPointOf(e.Connection));//metaDataOf(e).ConnectionIndex
            e.Connection.Disconnected += _server_Disconnected;
            e.Connection.ReceivedMessage += _server_ReceivedMessage;
            e.Connection.SetMode(MessageMode.PrefixedLength);

            tryPairClients();
            DebugInfo("Connected listener done");
        }

        private void _server_Disconnected(object sender, EventArgs e)
        {
            var connection = sender as Connection;
            DebugInfo("{0}: Client disconnected", connection.Id);
        }

        private void _server_ReceivedMessage(object sender, EventArgs e)
        {
            var connection = sender as Connection;
            var message = connection.ReceiveMessageString();
            DebugInfo("{0}: Server received delimited {1}", connection.Id, message);

            var parts = message.Split(new[] { ' ' }, 2);
            var verb = parts[0];

            switch (verb)
            {
                case "EndPoint":
                    //metaData.EndPoint = parts[1];
                    DebugInfo("{0}: EndPoint = \"{1}\"", connection.Id, parts[1]);//metaData.ConnectionIndex
                    break;

                default:
                    DebugInfo("{0}: Ignoring message \"{1}\"", connection.Id, message);//metaData.ConnectionIndex
                    break;
            }
        }

		private void tryPairClients()
		{
			if (_server.Connections.Count() == 2)
			{
				var client1 = _server.Connections.First();
				var client2 = _server.Connections.Last();

                var nounce = Guid.NewGuid();
                client1.Send(nounce.ToByteArray());
                client1.Send(endPointOf(client2).ToString());

                client2.Send(nounce.ToByteArray());
				client2.Send(endPointOf(client1).ToString());

                DebugInfo($"Sent pairing info to clients with nounce '{nounce}'.");
			}
		}

		static System.Net.IPEndPoint endPointOf(Connection connection)
		{
			return connection.Socket.RemoteEndPoint as System.Net.IPEndPoint;
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




		readonly TcpServer _server;
		readonly int _port;
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using SocketMessaging;

namespace PunchClient
{
	public class PunchClientTcp
	{
		internal bool Start(string hostNameOrAddress, int port)
		{
			if (_centralServerClient != null)
				throw new NotSupportedException("PunchClientTcp is already started.");

            var ipAddress = Dns.GetHostAddresses(hostNameOrAddress).First();
			_centralServerClient = TcpClient.Connect(ipAddress, port);
            _centralServerClient.SetMode(MessageMode.PrefixedLength);

            _centralServerClient.ReceivedMessage += _centralServerClient_ReceivedMessage;
			_centralServerClient.Disconnected += _centralServerClient_Disconnected;

			DebugInfo("CentralServerClient connected from {0} to {1}.", localEndpointOf(_centralServerClient), remoteEndpointOf(_centralServerClient));
			return true;
		}

        internal void Stop()
		{
			DebugInfo("Disconnecting from server.");
			_centralServerClient.Close();
			_centralServerClient = null;
		}

		internal void RegisterLocalEndpoint()
		{
            Send("EndPoint " + localEndpointOf(_centralServerClient));
        }

        internal void Send(string message)
        {
            _centralServerClient.Send(message);
            DebugInfo("CentralServerClient sent: {0}.", message);
        }

        private void _centralServerClient_ReceivedMessage(object sender, EventArgs e)
        {
            var client = sender as TcpClient;

            if (_nounce == Guid.Empty)
            {
                _nounce = new Guid(client.ReceiveMessage());
                DebugInfo("CentralServerClient received nounce '{0}'.", _nounce);
            }
            else
            {
                var message = client.ReceiveMessageString();
                var localEndpoint = localEndpointOf(client);
                client.Close();
                DebugInfo("Expecting other side to listening to {0}.", message);

                DebugInfo("Starting Peer Server at {0}.", localEndpoint.Port);
                _peerServer = new SocketMessaging.Server.TcpServer();
                _peerServer.Connected += _peerServer_Connected;
                _peerServer.Start(localEndpoint.Port);

                DebugInfo("Peer client connecting...");
                var messageParts = message.Split(':');
                var peerAddress = messageParts[0].Split('.').Select(x => byte.Parse(x)).ToArray();
                var peerPort = int.Parse(messageParts[1]);
                _peerClient = TcpClient.Connect(new IPAddress(peerAddress), peerPort);
                _peerClient.SetMode(MessageMode.PrefixedLength);
                _peerClient.Disconnected += _peerClient_Disconnected;
                _peerClient.ReceivedMessage += _peerClient_ReceivedMessage;
                DebugInfo("Peer client sending nounce '{1}' to {0}.", remoteEndpointOf(_peerClient), _nounce);
                _peerClient.Send(_nounce.ToByteArray());
            }
        }

        private void _centralServerClient_Disconnected(object sender, EventArgs e)
		{
			DebugInfo("CentralServerClient disconnected.");
		}

        private void _peerServer_Connected(object sender, SocketMessaging.Server.ConnectionEventArgs e)
        {
            DebugInfo("Peer Server got connection from {0}.", e.Connection.Socket.RemoteEndPoint);
            e.Connection.SetMode(MessageMode.PrefixedLength);
            e.Connection.ReceivedMessage += _peerServer_ReceivedMessage;
            //e.Connection.Close(/*true*/);
            //DebugInfo("Local Server disconnected client.");
        }

        private void _peerServer_ReceivedMessage(object sender, EventArgs e)
        {
            var connection = sender as SocketMessaging.Server.Connection;
            var nounceToCheck = new Guid(connection.ReceiveMessage());
            lock (_nounceCheckLock)
            {
                if (_peerConnectionEstablished)
                {
                    DebugInfo("Peer server refusing counterpart because connection already established: {0}", nounceToCheck);
                    connection.Send("Server-connection already established.");
                    connection.Close();
                }
                else if (_nounce != nounceToCheck)
                {
                    DebugInfo("Peer server refusing counterpart because nounce mismatch: {0}", nounceToCheck);
                    connection.Send("Incorrect Nounce");
                    connection.Close();
                }
                else
                {
                    DebugInfo("Peer server accepting counterpart and establishing connection: {0}", nounceToCheck);
                    _peerConnectionEstablished = true;
                    connection.Send("OK");
                }
            }
        }

        private void _peerClient_ReceivedMessage(object sender, EventArgs e)
        {
            var connection = sender as SocketMessaging.Server.Connection;
            var nounceResponse = connection.ReceiveMessageString();
            lock (_nounceCheckLock)
            {
                if (_peerConnectionEstablished)
                {
                    DebugInfo("Peer client got response '{0}' from counterpart when connection already got established.", nounceResponse);
                    //connection.Send("Client-connection already established.");
                    connection.Close();
                }
                else if (nounceResponse != "OK")
                {
                    DebugInfo("Peer client got rejected nounce from counterpart with message: {0}", nounceResponse);
                    //connection.Send("Incorrect Nounce");
                    connection.Close();
                }
                else
                {
                    DebugInfo("Peer client got OK from counterpart to establish connection: {0}", nounceResponse);
                    _peerConnectionEstablished = true;
                    //connection.Send("OK");
                }
            }
        }

        private void _peerServer_ClientDisconnected(object sender, System.Net.Sockets.TcpClient e)
		{
			DebugInfo("Peer server identified a disconnected client.");
			_centralServerClient = null;
		}

		private void _peerClient_Disconnected(object sender, EventArgs e)
		{
			DebugInfo("Peer client disconnected.");
		}




		static IPEndPoint localEndpointOf(TcpClient client)
		{
			return client.Socket.LocalEndPoint as IPEndPoint;
		}

		static IPEndPoint remoteEndpointOf(TcpClient client)
		{
			return client.Socket.RemoteEndPoint as IPEndPoint;
		}

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


		TcpClient _centralServerClient = null;
        SocketMessaging.Server.TcpServer _peerServer = null;
		TcpClient _peerClient = null;

        Guid _nounce = Guid.Empty;
        bool _peerConnectionEstablished = false;
        object _nounceCheckLock = new object();
    }
}

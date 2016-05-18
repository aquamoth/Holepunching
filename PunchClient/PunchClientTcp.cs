using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleTCP;
using System.Net;

namespace PunchClient
{
	public class PunchClientTcp
	{
		internal bool Start(string hostNameOrAddress, int port)
		{
			if (_centralServerClient != null)
				throw new NotSupportedException("PunchClientTcp is already started.");

			_centralServerClient = new SimpleTcpClient();
			_centralServerClient.DelimiterDataReceived += _centralServerClient_DelimiterDataReceived;
			//_centralServerClient.DataReceived += _centralServerClient_DataReceived;
			_centralServerClient.Disconnected += _centralServerClient_Disconnected;
			_centralServerClient.Connect(hostNameOrAddress, port);

			DebugInfo("Client connected to {0}.", remoteEndpointOf(_centralServerClient));
			return true;
		}

		internal void Stop()
		{
			DebugInfo("Disconnecting from server.");
			_centralServerClient.Disconnect();
			_centralServerClient = null;
		}

		internal void RegisterLocalEndpoint()
		{
			Send("EndPoint " + localEndpointOf(_centralServerClient));
		}

		internal void Send(string message)
		{
			_centralServerClient.WriteLine(message);
			DebugInfo("Client sent: {0}.", message);
		}

		private void _centralServerClient_DelimiterDataReceived(object sender, Message e)
		{
			var client = sender as SimpleTcpClient;

			DebugInfo("CentralServerClient received: {0}.", e.MessageString);
			var messageParts = e.MessageString.Split(':');
			var peerAddress = messageParts[0];
			var peerPort = int.Parse(messageParts[1]);
			var localEndpoint = localEndpointOf(client);

			client.Disconnect();

			_peerServer = new SimpleTcpServer();
			_peerServer.ClientConnected += _peerServer_ClientConnected;
			_peerServer.ClientDisconnected += _peerServer_ClientDisconnected;
			_peerServer.Start(localEndpoint.Address, localEndpoint.Port);
			DebugInfo("PeerServer started.");


			_peerClient = new SimpleTcpClient();
			_peerClient.Disconnected += _peerClient_Disconnected;
			_peerClient.Connect(peerAddress, peerPort);
			DebugInfo("Local peer client connected to {0}", remoteEndpointOf(_peerClient));
		}

		//private void _centralServerClient_DataReceived(object sender, Message e)
		//{
		//	DebugInfo("Client received {0}.", e.MessageString);
		//}

		private void _centralServerClient_Disconnected(object sender, EventArgs e)
		{
			DebugInfo("CentralServerClient disconnected.");
		}

		private void _peerServer_ClientConnected(object sender, System.Net.Sockets.TcpClient e)
		{
			DebugInfo("Local Server got connection from {0}.", e.Client.RemoteEndPoint);
			e.Client.Disconnect(true);
			DebugInfo("Local Server disconnected client.");
		}

		private void _peerServer_ClientDisconnected(object sender, System.Net.Sockets.TcpClient e)
		{
			DebugInfo("Client has been disconnected.");
			_centralServerClient = null;
		}

		private void _peerClient_Disconnected(object sender, EventArgs e)
		{
			DebugInfo("Local PeerClient disconnected.");
		}




		static IPEndPoint localEndpointOf(SimpleTcpClient client)
		{
			return client.TcpClient.Client.LocalEndPoint as IPEndPoint;
		}

		static IPEndPoint remoteEndpointOf(SimpleTcpClient client)
		{
			return client.TcpClient.Client.RemoteEndPoint as IPEndPoint;
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


		SimpleTcpClient _centralServerClient = null;
		SimpleTcpServer _peerServer = null;
		SimpleTcpClient _peerClient = null;

	}
}

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
using System.Net.NetworkInformation;
using AltarNet;

namespace PunchClient
{
	public class PunchClientTcp
	{
		internal async Task<bool> Start(string hostNameOrAddress, int port)
		{
			if (_centralServerClient != null)
				throw new NotSupportedException("PunchClientTcp is already started.");

			_centralServerClient = await connectToCentralServer(hostNameOrAddress, port);
			if (_centralServerClient == null)
				return false;

			Trace.TraceInformation("Client connected to {0}.", remoteEndpointOf(_centralServerClient.InfoHandler));
			return true;
		}

		internal void Stop()
		{
			Trace.TraceInformation("Disconnecting from server.");
			_centralServerClient.Disconnect();
			_centralServerClient = null;
		}

		internal async Task RegisterLocalEndpoint()
		{
			await Send("EndPoint " + localEndpointOf(_centralServerClient.InfoHandler));
		}

		internal async Task Send(string message)
		{
			var data = System.Text.Encoding.UTF8.GetBytes(message);
			await _centralServerClient.SendAsync(data);
			Trace.TraceInformation("Client sent: {0}.", message);
		}





		private void _client_ReceivedFull(object sender, AltarNet.TcpReceivedEventArgs e)
		{
			var message = System.Text.Encoding.UTF8.GetString(e.Data);
			Trace.TraceInformation("Client received: {0}.", message);

			var localPort = localEndpointOf(_centralServerClient.InfoHandler).Port;
			_centralServerClient.Disconnect();
			_peerServer = new TcpServerHandler(new IPAddress(0), localPort);
			_peerServer.Connected += _localServer_Connected;
			_peerServer.Start();

			var parts = message.Split(':');
			var address = Dns.GetHostAddresses(parts[0]).First();
			var port = int.Parse(parts[1]);
			_peerClient = new TcpClientHandler(address, port);
			var success = _peerClient.Connect();
			if (success)
			{
				Trace.TraceInformation("Local peer client is connected to {0}", _peerClient.InfoHandler.Client.Client.RemoteEndPoint);
			}
			else
			{
				Trace.TraceWarning("Local peer failed to connect.");
			}
		}

		private void _localServer_Connected(object sender, TcpEventArgs e)
		{
			Trace.TraceInformation("Local Server got connection from {0}.", e.Client.Client.Client.RemoteEndPoint);
			_peerServer.DisconnectClient(e.Client);
			Trace.TraceInformation("Local Server disconnected client.");
		}

		private void _client_Disconnected(object sender, AltarNet.TcpEventArgs e)
		{
			Trace.TraceInformation("Client has been disconnected.");
			_centralServerClient = null;
		}





		async Task<TcpClientHandler> connectToCentralServer(string hostNameOrAddress, int port)
		{
			var address = (await Dns.GetHostAddressesAsync(hostNameOrAddress)).Where(x => x.AddressFamily == AddressFamily.InterNetwork).First();
			var client = new AltarNet.TcpClientHandler(address, port);
			client.ReceivedFull += _client_ReceivedFull;
			client.Disconnected += _client_Disconnected;

			await client.ConnectAsync();
			if (client.LastConnectError != null)
			{
				Trace.TraceError("Failed to connect. Reason: {0}.", client.LastConnectError);
				client = null;
			}

			return client;
		}

		static IPEndPoint localEndpointOf(AltarNet.TcpClientInfo clientInfo)
		{
			return clientInfo.Client.Client.LocalEndPoint as IPEndPoint;
		}

		static IPEndPoint remoteEndpointOf(AltarNet.TcpClientInfo clientInfo)
		{
			return clientInfo.Client.Client.RemoteEndPoint as IPEndPoint;
		}



		TcpClientHandler _centralServerClient = null;
		TcpServerHandler _peerServer = null;
		TcpClientHandler _peerClient = null;

	}
}

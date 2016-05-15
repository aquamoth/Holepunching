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
			if (_client != null)
				throw new NotSupportedException("PunchClientTcp is already started.");

			_client = await connectToCentralServer(hostNameOrAddress, port);
			if (_client == null)
				return false;

			Trace.TraceInformation("Client connected to {0}.", remoteEndpointOf(_client.InfoHandler));
			return true;
		}

		internal void Stop()
		{
			Trace.TraceInformation("Disconnecting from server.");
			_client.Disconnect();
			_client = null;
		}

		internal async Task RegisterLocalEndpoint()
		{
			await Send("Endpoint " + localEndpointOf(_client.InfoHandler));
		}

		internal async Task Send(string message)
		{
			var data = System.Text.Encoding.UTF8.GetBytes(message);
			await _client.SendAsync(data);
			Trace.TraceInformation("Client sent: {0}.", message);
		}





		private void _client_ReceivedFull(object sender, AltarNet.TcpReceivedEventArgs e)
		{
			var message = System.Text.Encoding.UTF8.GetString(e.Data);
			Trace.TraceInformation("Client received: {0}.", message);
		}

		private void _client_Disconnected(object sender, AltarNet.TcpEventArgs e)
		{
			Trace.TraceInformation("Client has been disconnected.");
			_client = null;
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



		AltarNet.TcpClientHandler _client = null;
	}
}

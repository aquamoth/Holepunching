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

namespace PunchClient
{
	public class PunchClientTcp
	{
		internal async Task Start()
		{
			if (_client != null)
				throw new NotSupportedException("PunchClientTcp is already started.");

			var address = new System.Net.IPAddress(new byte[] { 127, 0, 0, 1 });
			_client = new AltarNet.TcpClientHandler(address, 7888);
			_client.ReceivedFull += _client_ReceivedFull;
			_client.Disconnected += _client_Disconnected;

			await _client.ConnectAsync();
		}

		internal async Task Send(string text)
		{
			var data = System.Text.Encoding.UTF8.GetBytes(text);
			await _client.SendAsync(data);
		}

		private void _client_ReceivedFull(object sender, AltarNet.TcpReceivedEventArgs e)
		{
			var message = System.Text.Encoding.UTF8.GetString(e.Data);
			Trace.TraceInformation("Client received: {0}.", message);
		}

		private void _client_Disconnected(object sender, AltarNet.TcpEventArgs e)
		{
			_client = null;
		}

		internal void Stop()
		{
			_client.Disconnect();
		}

		AltarNet.TcpClientHandler _client = null;
	}
}

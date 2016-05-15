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

namespace PunchServer
{
	public class PunchServerTcp
	{
		public PunchServerTcp(int port)
		{
			_connectionIndex = 0;

			var localaddr = new IPAddress(0);
			_server = new AltarNet.TcpServerHandler(localaddr, port);
			_server.Connected += _server_Connected;
			_server.Disconnected += _server_Disconnected;
			_server.ReceivedFragment += _listener_ReceivedFragment;
			_server.ReceivedFull += _listener_ReceivedFull;
		}

		internal void Start()
		{
			if (_server.IsListening)
				throw new NotSupportedException("PunchServerTcp is already started.");
			_server.Start();
		}

		internal async Task Stop()
		{
			if (!_server.IsListening)
				throw new NotSupportedException("PunchServerTcp is not started.");
			_server.Stop();
			Trace.TraceInformation("Server stopped listening for connections.");
			_server.DisconnectAll();
			Trace.TraceInformation("Server disconnected all existing clients.");
		}

		private void _server_Connected(object sender, AltarNet.TcpEventArgs e)
		{
			e.Client.Tag = ++_connectionIndex;
			Trace.TraceInformation("{0}: Client connected", e.Client.Tag);
		}

		private void _server_Disconnected(object sender, AltarNet.TcpEventArgs e)
		{
			Trace.TraceInformation("{0}: Client disconnected", e.Client.Tag);
		}

		private void _listener_ReceivedFragment(object sender, AltarNet.TcpFragmentReceivedEventArgs e)
		{
			var message = System.Text.Encoding.UTF8.GetString(e.Packet.Data);
			Trace.TraceInformation("{0}: Received fragment \"{1}\"", e.Client.Tag, message);
		}

		private void _listener_ReceivedFull(object sender, AltarNet.TcpReceivedEventArgs e)
		{
			var message = System.Text.Encoding.UTF8.GetString(e.Data);
			Trace.TraceInformation("{0}: Received full \"{1}\"", e.Client.Tag, message);
		}
		

		readonly AltarNet.TcpServerHandler _server;
		int _connectionIndex;
	}
}

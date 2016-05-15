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
using AltarNet;

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
			_server.ReceivedFull += _server_ReceivedFull;
		}

		internal void Start()
		{
			if (_server.IsListening)
				throw new NotSupportedException("PunchServerTcp is already started.");
			_server.Start();
		}

		internal void Stop()
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
			e.Client.Tag = new ClientMetaData { ConnectionIndex = ++_connectionIndex };
			Trace.TraceInformation("{0}: Client connected from {1}", metaDataOf(e.Client).ConnectionIndex, endPointOf(e.Client));
			
			tryPairClients();
			Trace.TraceInformation("Connected listener done");
		}

		private void _server_Disconnected(object sender, AltarNet.TcpEventArgs e)
		{
			Trace.TraceInformation("{0}: Client disconnected", metaDataOf(e.Client).ConnectionIndex);
		}

		private void _server_ReceivedFull(object sender, AltarNet.TcpReceivedEventArgs e)
		{
			var metaData = metaDataOf(e.Client);

			var dataAsString = System.Text.Encoding.UTF8.GetString(e.Data);
			var parts = dataAsString.Split(new[] { ' ' }, 2);
			var verb = parts[0];

			switch (verb)
			{
				case "EndPoint":
					metaData.EndPoint = parts[1];
					Trace.TraceInformation("{0}: EndPoint = \"{1}\"", metaData.ConnectionIndex, metaData.EndPoint);
					break;

				default:
					Trace.TraceInformation("{0}: Ignoring message \"{1}\"", metaData.ConnectionIndex, dataAsString);
					break;
			}

		}



		private void tryPairClients()
		{
			if (_server.Clients.Count == 2)
			{
				var client1 = _server.Clients.First().Value;
				var client2 = _server.Clients.Skip(1).First().Value;

				//var localEndpoint = client1.Client.Client.LocalEndPoint;
				var client1Endpoint = endPointOf(client1);
				var client2Endpoint = endPointOf(client2);

				var task1 = sendAsync(client1, client2Endpoint.ToString());
				var task2 = sendAsync(client2, client1Endpoint.ToString());
				Trace.TraceInformation("Sent messages pairing clients");
				Task.WaitAll(new[] { task1, task2 });
			}
		}

		static IPEndPoint endPointOf(AltarNet.TcpClientInfo clientInfo)
		{
			return clientInfo.Client.Client.RemoteEndPoint as IPEndPoint;
		}

		static async Task sendAsync(AltarNet.TcpClientInfo client1, string message)
		{
			var data = Encoding.UTF8.GetBytes(message);
			await client1.SendAsync(data);
		}

		private ClientMetaData metaDataOf(TcpClientInfo client)
		{
			return client.Tag as ClientMetaData;
		}




		readonly AltarNet.TcpServerHandler _server;
		int _connectionIndex;
	}
}

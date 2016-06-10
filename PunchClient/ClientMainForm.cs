using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PunchClient
{
	public partial class ClientMainForm : Form
	{
		public ClientMainForm()
		{
			InitializeComponent();
			_client = new PunchClientTcp();
			buttonDisconnect.Enabled = false;
		}

		void buttonConnect_Click(object sender, EventArgs e)
		{
			buttonConnect.Enabled = false;
			var hostnameOrAddress = textBoxHostname.Text;
			var port = int.Parse(textBoxPort.Text);
			var success = _client.Start(hostnameOrAddress, port);
			if (success)
			{
				buttonDisconnect.Enabled = true;
			}
			else
			{
				buttonConnect.Enabled = true;
			}
		}

		private void buttonregisterEndpoint_Click(object sender, EventArgs e)
		{
			_client.RegisterLocalEndpoint();
		}

		private void buttonSend_Click(object sender, EventArgs e)
		{
			_client.Send(textBoxMessage.Text);
		}

		private void buttonDisconnect_Click(object sender, EventArgs e)
		{
			_client.Stop();
			buttonConnect.Enabled = true;
			buttonDisconnect.Enabled = false;
		}


		readonly PunchClientTcp _client;
	}
}

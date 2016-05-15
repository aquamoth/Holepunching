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

		async void buttonConnect_Click(object sender, EventArgs e)
		{
			buttonConnect.Enabled = false;
			var success = await _client.Start(textBoxHostname.Text, int.Parse(textBoxPort.Text));
			if (success)
			{
				buttonDisconnect.Enabled = true;
			}
			else
			{
				buttonConnect.Enabled = true;
			}
		}

		private async void buttonregisterEndpoint_Click(object sender, EventArgs e)
		{
			await _client.RegisterLocalEndpoint();
		}

		private async void buttonSend_Click(object sender, EventArgs e)
		{
			await _client.Send(textBoxMessage.Text);
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

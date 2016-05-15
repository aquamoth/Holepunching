using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
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
		}

		async void buttonConnect_Click(object sender, EventArgs e)
		{
			await _client.Start();
		}

		private void buttonSend_Click(object sender, EventArgs e)
		{
			_client.Send(textBoxMessage.Text);
		}

		private void buttonDisconnect_Click(object sender, EventArgs e)
		{
			_client.Stop();
		}


		readonly PunchClientTcp _client;
	}
}

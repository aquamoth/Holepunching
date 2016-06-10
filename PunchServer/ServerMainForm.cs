using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PunchServer
{
	public partial class ServerMainForm : Form
	{
		public ServerMainForm()
		{
			InitializeComponent();
			_tcpServer = new PunchServerTcp(7888);
		}

		void buttonStartTcp_Click(object sender, EventArgs e)
		{
			_tcpServer.Start();
		}

		void buttonStopTcp_Click(object sender, EventArgs e)
		{
			_tcpServer.Stop();
		}



		readonly PunchServerTcp _tcpServer;
	}
}

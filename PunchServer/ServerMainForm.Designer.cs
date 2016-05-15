namespace PunchServer
{
	partial class ServerMainForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.buttonStartTcp = new System.Windows.Forms.Button();
			this.buttonStopTcp = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// buttonStartTcp
			// 
			this.buttonStartTcp.Location = new System.Drawing.Point(24, 22);
			this.buttonStartTcp.Name = "buttonStartTcp";
			this.buttonStartTcp.Size = new System.Drawing.Size(364, 122);
			this.buttonStartTcp.TabIndex = 0;
			this.buttonStartTcp.Text = "Start TCP";
			this.buttonStartTcp.UseVisualStyleBackColor = true;
			this.buttonStartTcp.Click += new System.EventHandler(this.buttonStartTcp_Click);
			// 
			// buttonStopTcp
			// 
			this.buttonStopTcp.Location = new System.Drawing.Point(24, 181);
			this.buttonStopTcp.Name = "buttonStopTcp";
			this.buttonStopTcp.Size = new System.Drawing.Size(364, 122);
			this.buttonStopTcp.TabIndex = 0;
			this.buttonStopTcp.Text = "Stop TCP";
			this.buttonStopTcp.UseVisualStyleBackColor = true;
			this.buttonStopTcp.Click += new System.EventHandler(this.buttonStopTcp_Click);
			// 
			// ServerMainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(428, 330);
			this.Controls.Add(this.buttonStopTcp);
			this.Controls.Add(this.buttonStartTcp);
			this.Name = "ServerMainForm";
			this.Text = "PunchServer";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button buttonStartTcp;
		private System.Windows.Forms.Button buttonStopTcp;
	}
}


namespace PunchClient
{
	partial class ClientMainForm
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
			this.buttonConnect = new System.Windows.Forms.Button();
			this.textBoxMessage = new System.Windows.Forms.TextBox();
			this.buttonSend = new System.Windows.Forms.Button();
			this.buttonDisconnect = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// buttonConnect
			// 
			this.buttonConnect.Location = new System.Drawing.Point(25, 21);
			this.buttonConnect.Name = "buttonConnect";
			this.buttonConnect.Size = new System.Drawing.Size(258, 129);
			this.buttonConnect.TabIndex = 0;
			this.buttonConnect.Text = "Connect to Server";
			this.buttonConnect.UseVisualStyleBackColor = true;
			this.buttonConnect.Click += new System.EventHandler(this.buttonConnect_Click);
			// 
			// textBoxMessage
			// 
			this.textBoxMessage.Location = new System.Drawing.Point(25, 171);
			this.textBoxMessage.Name = "textBoxMessage";
			this.textBoxMessage.Size = new System.Drawing.Size(258, 20);
			this.textBoxMessage.TabIndex = 1;
			// 
			// buttonSend
			// 
			this.buttonSend.Location = new System.Drawing.Point(25, 197);
			this.buttonSend.Name = "buttonSend";
			this.buttonSend.Size = new System.Drawing.Size(258, 55);
			this.buttonSend.TabIndex = 0;
			this.buttonSend.Text = "Send";
			this.buttonSend.UseVisualStyleBackColor = true;
			this.buttonSend.Click += new System.EventHandler(this.buttonSend_Click);
			// 
			// buttonDisconnect
			// 
			this.buttonDisconnect.Location = new System.Drawing.Point(25, 279);
			this.buttonDisconnect.Name = "buttonDisconnect";
			this.buttonDisconnect.Size = new System.Drawing.Size(258, 129);
			this.buttonDisconnect.TabIndex = 0;
			this.buttonDisconnect.Text = "Disconnect";
			this.buttonDisconnect.UseVisualStyleBackColor = true;
			this.buttonDisconnect.Click += new System.EventHandler(this.buttonDisconnect_Click);
			// 
			// ClientMainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(362, 420);
			this.Controls.Add(this.textBoxMessage);
			this.Controls.Add(this.buttonSend);
			this.Controls.Add(this.buttonDisconnect);
			this.Controls.Add(this.buttonConnect);
			this.Name = "ClientMainForm";
			this.Text = "PunchClient";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button buttonConnect;
		private System.Windows.Forms.TextBox textBoxMessage;
		private System.Windows.Forms.Button buttonSend;
		private System.Windows.Forms.Button buttonDisconnect;
	}
}


namespace BackgroundWorkerService.Service
{
	partial class UserInterface
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UserInterface));
			this.btnForceExit = new System.Windows.Forms.Button();
			this.pnlLayout = new System.Windows.Forms.TableLayoutPanel();
			this.lblMessage = new System.Windows.Forms.Label();
			this.pnlLayout.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnForceExit
			// 
			this.btnForceExit.Location = new System.Drawing.Point(417, 103);
			this.btnForceExit.Name = "btnForceExit";
			this.btnForceExit.Size = new System.Drawing.Size(75, 23);
			this.btnForceExit.TabIndex = 0;
			this.btnForceExit.Text = "Exit";
			this.btnForceExit.UseVisualStyleBackColor = true;
			this.btnForceExit.Click += new System.EventHandler(this.btnForceExit_Click);
			// 
			// pnlLayout
			// 
			this.pnlLayout.ColumnCount = 1;
			this.pnlLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.pnlLayout.Controls.Add(this.lblMessage, 0, 0);
			this.pnlLayout.Location = new System.Drawing.Point(12, 12);
			this.pnlLayout.Name = "pnlLayout";
			this.pnlLayout.RowCount = 1;
			this.pnlLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.pnlLayout.Size = new System.Drawing.Size(480, 85);
			this.pnlLayout.TabIndex = 2;
			// 
			// lblMessage
			// 
			this.lblMessage.AutoSize = true;
			this.lblMessage.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lblMessage.Location = new System.Drawing.Point(3, 0);
			this.lblMessage.Name = "lblMessage";
			this.lblMessage.Size = new System.Drawing.Size(474, 85);
			this.lblMessage.TabIndex = 2;
			this.lblMessage.Text = "Waiting...";
			this.lblMessage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// UserInterface
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(504, 133);
			this.ControlBox = false;
			this.Controls.Add(this.pnlLayout);
			this.Controls.Add(this.btnForceExit);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "UserInterface";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Background Worker Service";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.UserInterface_FormClosing);
			this.Load += new System.EventHandler(this.UserInterface_Load);
			this.pnlLayout.ResumeLayout(false);
			this.pnlLayout.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button btnForceExit;
		private System.Windows.Forms.TableLayoutPanel pnlLayout;
		private System.Windows.Forms.Label lblMessage;
	}
}
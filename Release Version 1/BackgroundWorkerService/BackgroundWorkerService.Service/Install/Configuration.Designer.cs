namespace BackgroundWorkerService.Service.Install
{
	partial class Configuration
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Configuration));
			this.NewSettingValue = new System.Windows.Forms.ToolStripMenuItem();
			this.SettingValuesMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.MainLayout = new System.Windows.Forms.TableLayoutPanel();
			this.ButtonLayout = new System.Windows.Forms.TableLayoutPanel();
			this.btnOk = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.HeaderPanel = new System.Windows.Forms.TableLayoutPanel();
			this.ConfigSettingsLayout = new System.Windows.Forms.TableLayoutPanel();
			this.lblSettingsHeader = new System.Windows.Forms.Label();
			this.lblSettingsSubHeader = new System.Windows.Forms.Label();
			this.HeaderPicture = new System.Windows.Forms.PictureBox();
			this.SettingsGrid = new System.Windows.Forms.PropertyGrid();
			this.listImages = new System.Windows.Forms.ImageList(this.components);
			this.SettingValueMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.RemoveSettingValue = new System.Windows.Forms.ToolStripMenuItem();
			this.SettingsMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.NewBooleanSetting = new System.Windows.Forms.ToolStripMenuItem();
			this.NewInt32Setting = new System.Windows.Forms.ToolStripMenuItem();
			this.NewStringSetting = new System.Windows.Forms.ToolStripMenuItem();
			this.SettingMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.RemoveSetting = new System.Windows.Forms.ToolStripMenuItem();
			this.SettingValuesMenu.SuspendLayout();
			this.MainLayout.SuspendLayout();
			this.ButtonLayout.SuspendLayout();
			this.HeaderPanel.SuspendLayout();
			this.ConfigSettingsLayout.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.HeaderPicture)).BeginInit();
			this.SettingValueMenu.SuspendLayout();
			this.SettingsMenu.SuspendLayout();
			this.SettingMenu.SuspendLayout();
			this.SuspendLayout();
			// 
			// NewSettingValue
			// 
			this.NewSettingValue.Name = "NewSettingValue";
			this.NewSettingValue.Size = new System.Drawing.Size(135, 22);
			this.NewSettingValue.Text = "New Value";
			// 
			// SettingValuesMenu
			// 
			this.SettingValuesMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.NewSettingValue});
			this.SettingValuesMenu.Name = "SettingValuesMenu";
			this.SettingValuesMenu.Size = new System.Drawing.Size(136, 26);
			// 
			// MainLayout
			// 
			this.MainLayout.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.MainLayout.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Inset;
			this.MainLayout.ColumnCount = 1;
			this.MainLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.MainLayout.Controls.Add(this.ButtonLayout, 0, 2);
			this.MainLayout.Controls.Add(this.HeaderPanel, 0, 0);
			this.MainLayout.Controls.Add(this.SettingsGrid, 0, 1);
			this.MainLayout.Dock = System.Windows.Forms.DockStyle.Fill;
			this.MainLayout.Location = new System.Drawing.Point(0, 0);
			this.MainLayout.Name = "MainLayout";
			this.MainLayout.RowCount = 3;
			this.MainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 69F));
			this.MainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.MainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
			this.MainLayout.Size = new System.Drawing.Size(807, 421);
			this.MainLayout.TabIndex = 5;
			// 
			// ButtonLayout
			// 
			this.ButtonLayout.ColumnCount = 3;
			this.ButtonLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 73.93645F));
			this.ButtonLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 13.03178F));
			this.ButtonLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 13.03178F));
			this.ButtonLayout.Controls.Add(this.btnOk, 2, 0);
			this.ButtonLayout.Controls.Add(this.btnCancel, 1, 0);
			this.ButtonLayout.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ButtonLayout.Location = new System.Drawing.Point(5, 387);
			this.ButtonLayout.Name = "ButtonLayout";
			this.ButtonLayout.RowCount = 1;
			this.ButtonLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.ButtonLayout.Size = new System.Drawing.Size(797, 29);
			this.ButtonLayout.TabIndex = 4;
			// 
			// btnOk
			// 
			this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOk.Dock = System.Windows.Forms.DockStyle.Fill;
			this.btnOk.Location = new System.Drawing.Point(695, 3);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(99, 23);
			this.btnOk.TabIndex = 3;
			this.btnOk.Text = "Ok";
			this.btnOk.UseVisualStyleBackColor = true;
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.btnCancel.Location = new System.Drawing.Point(592, 3);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(97, 23);
			this.btnCancel.TabIndex = 4;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			// 
			// HeaderPanel
			// 
			this.HeaderPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.HeaderPanel.BackColor = System.Drawing.Color.White;
			this.HeaderPanel.ColumnCount = 2;
			this.HeaderPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.HeaderPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 57F));
			this.HeaderPanel.Controls.Add(this.ConfigSettingsLayout, 0, 0);
			this.HeaderPanel.Controls.Add(this.HeaderPicture, 1, 0);
			this.HeaderPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.HeaderPanel.Location = new System.Drawing.Point(2, 2);
			this.HeaderPanel.Margin = new System.Windows.Forms.Padding(0);
			this.HeaderPanel.Name = "HeaderPanel";
			this.HeaderPanel.RowCount = 1;
			this.HeaderPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.HeaderPanel.Size = new System.Drawing.Size(803, 69);
			this.HeaderPanel.TabIndex = 4;
			// 
			// ConfigSettingsLayout
			// 
			this.ConfigSettingsLayout.ColumnCount = 2;
			this.ConfigSettingsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 15F));
			this.ConfigSettingsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 470F));
			this.ConfigSettingsLayout.Controls.Add(this.lblSettingsHeader, 0, 0);
			this.ConfigSettingsLayout.Controls.Add(this.lblSettingsSubHeader, 1, 1);
			this.ConfigSettingsLayout.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ConfigSettingsLayout.Location = new System.Drawing.Point(3, 3);
			this.ConfigSettingsLayout.Name = "ConfigSettingsLayout";
			this.ConfigSettingsLayout.RowCount = 2;
			this.ConfigSettingsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.ConfigSettingsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.ConfigSettingsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.ConfigSettingsLayout.Size = new System.Drawing.Size(740, 63);
			this.ConfigSettingsLayout.TabIndex = 5;
			// 
			// lblSettingsHeader
			// 
			this.lblSettingsHeader.AutoSize = true;
			this.ConfigSettingsLayout.SetColumnSpan(this.lblSettingsHeader, 2);
			this.lblSettingsHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblSettingsHeader.Location = new System.Drawing.Point(3, 0);
			this.lblSettingsHeader.Name = "lblSettingsHeader";
			this.lblSettingsHeader.Size = new System.Drawing.Size(308, 13);
			this.lblSettingsHeader.TabIndex = 0;
			this.lblSettingsHeader.Text = "Default Configuration For Background Worker Service";
			// 
			// lblSettingsSubHeader
			// 
			this.lblSettingsSubHeader.AutoSize = true;
			this.lblSettingsSubHeader.Location = new System.Drawing.Point(18, 20);
			this.lblSettingsSubHeader.Name = "lblSettingsSubHeader";
			this.lblSettingsSubHeader.Size = new System.Drawing.Size(718, 26);
			this.lblSettingsSubHeader.TabIndex = 1;
			this.lblSettingsSubHeader.Text = resources.GetString("lblSettingsSubHeader.Text");
			// 
			// HeaderPicture
			// 
			this.HeaderPicture.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.HeaderPicture.Image = ((System.Drawing.Image)(resources.GetObject("HeaderPicture.Image")));
			this.HeaderPicture.Location = new System.Drawing.Point(750, 13);
			this.HeaderPicture.Margin = new System.Windows.Forms.Padding(0);
			this.HeaderPicture.Name = "HeaderPicture";
			this.HeaderPicture.Size = new System.Drawing.Size(48, 42);
			this.HeaderPicture.TabIndex = 2;
			this.HeaderPicture.TabStop = false;
			// 
			// SettingsGrid
			// 
			this.SettingsGrid.Dock = System.Windows.Forms.DockStyle.Fill;
			this.SettingsGrid.Location = new System.Drawing.Point(5, 76);
			this.SettingsGrid.Name = "SettingsGrid";
			this.SettingsGrid.Size = new System.Drawing.Size(797, 303);
			this.SettingsGrid.TabIndex = 5;
			// 
			// listImages
			// 
			this.listImages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("listImages.ImageStream")));
			this.listImages.TransparentColor = System.Drawing.Color.Fuchsia;
			this.listImages.Images.SetKeyName(0, "Setting");
			// 
			// SettingValueMenu
			// 
			this.SettingValueMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.RemoveSettingValue});
			this.SettingValueMenu.Name = "SettingValueMenu";
			this.SettingValueMenu.Size = new System.Drawing.Size(125, 26);
			// 
			// RemoveSettingValue
			// 
			this.RemoveSettingValue.Name = "RemoveSettingValue";
			this.RemoveSettingValue.Size = new System.Drawing.Size(124, 22);
			this.RemoveSettingValue.Text = "Remove";
			// 
			// SettingsMenu
			// 
			this.SettingsMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.NewBooleanSetting,
            this.NewInt32Setting,
            this.NewStringSetting});
			this.SettingsMenu.Name = "SettingsMenu";
			this.SettingsMenu.Size = new System.Drawing.Size(185, 70);
			// 
			// NewBooleanSetting
			// 
			this.NewBooleanSetting.Name = "NewBooleanSetting";
			this.NewBooleanSetting.Size = new System.Drawing.Size(184, 22);
			this.NewBooleanSetting.Text = "New Boolean Setting";
			// 
			// NewInt32Setting
			// 
			this.NewInt32Setting.Name = "NewInt32Setting";
			this.NewInt32Setting.Size = new System.Drawing.Size(184, 22);
			this.NewInt32Setting.Text = "New Int32 Setting";
			// 
			// NewStringSetting
			// 
			this.NewStringSetting.Name = "NewStringSetting";
			this.NewStringSetting.Size = new System.Drawing.Size(184, 22);
			this.NewStringSetting.Text = "New String Setting";
			// 
			// SettingMenu
			// 
			this.SettingMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.RemoveSetting});
			this.SettingMenu.Name = "SettingMenu";
			this.SettingMenu.Size = new System.Drawing.Size(162, 26);
			// 
			// RemoveSetting
			// 
			this.RemoveSetting.Name = "RemoveSetting";
			this.RemoveSetting.Size = new System.Drawing.Size(161, 22);
			this.RemoveSetting.Text = "Remove Setting";
			// 
			// Configuration
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(807, 421);
			this.ControlBox = false;
			this.Controls.Add(this.MainLayout);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "Configuration";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Background Worker Service Configuration";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Configuration_FormClosed);
			this.Load += new System.EventHandler(this.Configuration_Load);
			this.SettingValuesMenu.ResumeLayout(false);
			this.MainLayout.ResumeLayout(false);
			this.ButtonLayout.ResumeLayout(false);
			this.HeaderPanel.ResumeLayout(false);
			this.ConfigSettingsLayout.ResumeLayout(false);
			this.ConfigSettingsLayout.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.HeaderPicture)).EndInit();
			this.SettingValueMenu.ResumeLayout(false);
			this.SettingsMenu.ResumeLayout(false);
			this.SettingMenu.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ToolStripMenuItem NewSettingValue;
		private System.Windows.Forms.ContextMenuStrip SettingValuesMenu;
		private System.Windows.Forms.TableLayoutPanel MainLayout;
		private System.Windows.Forms.TableLayoutPanel ButtonLayout;
		private System.Windows.Forms.Button btnOk;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.TableLayoutPanel HeaderPanel;
		private System.Windows.Forms.PictureBox HeaderPicture;
		private System.Windows.Forms.PropertyGrid SettingsGrid;
		private System.Windows.Forms.ImageList listImages;
		private System.Windows.Forms.ContextMenuStrip SettingValueMenu;
		private System.Windows.Forms.ToolStripMenuItem RemoveSettingValue;
		private System.Windows.Forms.ContextMenuStrip SettingsMenu;
		private System.Windows.Forms.ToolStripMenuItem NewBooleanSetting;
		private System.Windows.Forms.ToolStripMenuItem NewInt32Setting;
		private System.Windows.Forms.ToolStripMenuItem NewStringSetting;
		private System.Windows.Forms.ContextMenuStrip SettingMenu;
		private System.Windows.Forms.ToolStripMenuItem RemoveSetting;
		private System.Windows.Forms.TableLayoutPanel ConfigSettingsLayout;
		private System.Windows.Forms.Label lblSettingsHeader;
		private System.Windows.Forms.Label lblSettingsSubHeader;
	}
}
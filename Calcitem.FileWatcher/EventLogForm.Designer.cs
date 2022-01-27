/*
 * Copyright (c) 2006 Calcitem Studio
 */

namespace Calcitem.FileSystemWatch
{
    partial class EventLogForm
    {
        ///<summary>
        ///Required designer variables.
        ///</summary>
        private System.ComponentModel.IContainer components = null;

        ///<summary>
        ///Clean up all resources in use.
        ///</summary>
        ///<param name="disposing">true if the managed resource should be released; otherwise, false. </param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Forms Designer Generated Code

        ///<summary>
        ///Designer supports required methods -don't
        ///Use a code editor to modify the content of this method.
        ///</summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EventLogForm));
            this.MyLog = new System.Diagnostics.EventLog();
            this.logListView = new System.Windows.Forms.ListView();
            this.dateCol = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.timeCol = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.eventCol = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.typeCol = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.logToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadOptionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.autoRefashToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.manualRefashToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.OptionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.maxLogSizeStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MaximumKilobytesTextBox = new System.Windows.Forms.ToolStripTextBox();
            this.overFlowActionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.DoNotOverwriteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.OverwriteAsNeededToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.OverwriteOlderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.refashLogButton = new System.Windows.Forms.ToolStripButton();
            this.saveButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.includeTextBox = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.markButton = new System.Windows.Forms.ToolStripButton();
            this.markTextBox = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripLabel3 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel4 = new System.Windows.Forms.ToolStripLabel();
            this.msgCountTextBox = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripLabel5 = new System.Windows.Forms.ToolStripLabel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripSplitButton1 = new System.Windows.Forms.ToolStripSplitButton();
            this.autoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.manualToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.MyLog)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // myLog
            // 
            this.MyLog.EnableRaisingEvents = true;
            this.MyLog.Log = "File System";
            this.MyLog.SynchronizingObject = this;
            this.MyLog.EntryWritten += new System.Diagnostics.EntryWrittenEventHandler(this.OnEntryWritten);
            // 
            // logListView
            // 
            this.logListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.logListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.dateCol,
            this.timeCol,
            this.eventCol,
            this.typeCol});
            this.logListView.FullRowSelect = true;
            this.logListView.GridLines = true;
            this.logListView.HideSelection = false;
            this.logListView.Location = new System.Drawing.Point(0, 69);
            this.logListView.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.logListView.Name = "logListView";
            this.logListView.Size = new System.Drawing.Size(1055, 664);
            this.logListView.TabIndex = 0;
            this.logListView.UseCompatibleStateImageBehavior = false;
            this.logListView.View = System.Windows.Forms.View.Details;
            this.logListView.SelectedIndexChanged += new System.EventHandler(this.logListView_SelectedIndexChanged);
            this.logListView.Click += new System.EventHandler(this.logListView_Click);
            // 
            // dateCol
            // 
            this.dateCol.Text = "Date";
            this.dateCol.Width = 78;
            // 
            // timeCol
            // 
            this.timeCol.Text = "Time";
            this.timeCol.Width = 81;
            // 
            // eventCol
            // 
            this.eventCol.Text = "Event";
            this.eventCol.Width = 542;
            // 
            // typeCol
            // 
            this.typeCol.Text = "Type";
            this.typeCol.Width = 40;
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.logToolStripMenuItem,
            this.OptionToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(8, 3, 0, 3);
            this.menuStrip1.Size = new System.Drawing.Size(1056, 30);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // logToolStripMenuItem
            // 
            this.logToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadOptionToolStripMenuItem,
            this.clearLogToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.logToolStripMenuItem.Name = "logToolStripMenuItem";
            this.logToolStripMenuItem.Size = new System.Drawing.Size(48, 24);
            this.logToolStripMenuItem.Text = "&Log";
            // 
            // loadOptionToolStripMenuItem
            // 
            this.loadOptionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.autoRefashToolStripMenuItem,
            this.manualRefashToolStripMenuItem});
            this.loadOptionToolStripMenuItem.Name = "loadOptionToolStripMenuItem";
            this.loadOptionToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this.loadOptionToolStripMenuItem.Text = "Import ";
            // 
            // autoRefashToolStripMenuItem
            // 
            this.autoRefashToolStripMenuItem.Name = "autoRefashToolStripMenuItem";
            this.autoRefashToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.autoRefashToolStripMenuItem.Size = new System.Drawing.Size(171, 26);
            this.autoRefashToolStripMenuItem.Text = "&Automatically";
            this.autoRefashToolStripMenuItem.Click += new System.EventHandler(this.autoRefreshToolStripMenuItem_Click);
            // 
            // manualRefashToolStripMenuItem
            // 
            this.manualRefashToolStripMenuItem.Checked = true;
            this.manualRefashToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.manualRefashToolStripMenuItem.Name = "manualRefashToolStripMenuItem";
            this.manualRefashToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F6;
            this.manualRefashToolStripMenuItem.Size = new System.Drawing.Size(171, 26);
            this.manualRefashToolStripMenuItem.Text = "&Manually";
            this.manualRefashToolStripMenuItem.Click += new System.EventHandler(this.manualRefreshToolStripMenuItem_Click);
            // 
            // clearLogToolStripMenuItem
            // 
            this.clearLogToolStripMenuItem.Image = global::Calcitem.FileSystemWatch.Properties.Resources.DeleteHS;
            this.clearLogToolStripMenuItem.Name = "clearLogToolStripMenuItem";
            this.clearLogToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this.clearLogToolStripMenuItem.Text = "&Clear log";
            this.clearLogToolStripMenuItem.Click += new System.EventHandler(this.clearLogToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(221, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this.exitToolStripMenuItem.Text = "&Exit";
            // 
            // OptionToolStripMenuItem
            // 
            this.OptionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.maxLogSizeStripMenuItem,
            this.overFlowActionToolStripMenuItem});
            this.OptionToolStripMenuItem.Name = "OptionToolStripMenuItem";
            this.OptionToolStripMenuItem.Size = new System.Drawing.Size(69, 24);
            this.OptionToolStripMenuItem.Text = "&Option";
            // 
            // maxLogSizeStripMenuItem
            // 
            this.maxLogSizeStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MaximumKilobytesTextBox});
            this.maxLogSizeStripMenuItem.Name = "maxLogSizeStripMenuItem";
            this.maxLogSizeStripMenuItem.Size = new System.Drawing.Size(268, 26);
            this.maxLogSizeStripMenuItem.Text = "&Maximum log file size (kb)";
            // 
            // MaximumKilobytesTextBox
            // 
            this.MaximumKilobytesTextBox.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MaximumKilobytesTextBox.Name = "MaximumKilobytesTextBox";
            this.MaximumKilobytesTextBox.Size = new System.Drawing.Size(100, 27);
            this.MaximumKilobytesTextBox.Text = "512";
            this.MaximumKilobytesTextBox.Leave += new System.EventHandler(this.MaximumKilobytesTextBox_Leave);
            this.MaximumKilobytesTextBox.TextChanged += new System.EventHandler(this.MaximumKilobytesTextBox_TextChanged);
            this.MaximumKilobytesTextBox.VisibleChanged += new System.EventHandler(this.MaximumKilobytesTextBox_VisibleChanged);
            // 
            // overFlowActionToolStripMenuItem
            // 
            this.overFlowActionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.DoNotOverwriteToolStripMenuItem,
            this.OverwriteAsNeededToolStripMenuItem,
            this.OverwriteOlderToolStripMenuItem});
            this.overFlowActionToolStripMenuItem.Enabled = false;
            this.overFlowActionToolStripMenuItem.Name = "overFlowActionToolStripMenuItem";
            this.overFlowActionToolStripMenuItem.Size = new System.Drawing.Size(268, 26);
            this.overFlowActionToolStripMenuItem.Text = "When the maximum log size is reached";
            // 
            // DoNotOverwriteToolStripMenuItem
            // 
            this.DoNotOverwriteToolStripMenuItem.Enabled = false;
            this.DoNotOverwriteToolStripMenuItem.Name = "DoNotOverwriteToolStripMenuItem";
            this.DoNotOverwriteToolStripMenuItem.Size = new System.Drawing.Size(332, 26);
            this.DoNotOverwriteToolStripMenuItem.Text = "Keep existing log entries and discard new ones";
            // 
            // OverwriteAsNeededToolStripMenuItem
            // 
            this.OverwriteAsNeededToolStripMenuItem.Checked = true;
            this.OverwriteAsNeededToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.OverwriteAsNeededToolStripMenuItem.Enabled = false;
            this.OverwriteAsNeededToolStripMenuItem.Name = "OverwriteAsNeededToolStripMenuItem";
            this.OverwriteAsNeededToolStripMenuItem.Size = new System.Drawing.Size(332, 26);
            this.OverwriteAsNeededToolStripMenuItem.Text = "Overwrite old log entries with new log entries";
            // 
            // OverwriteOlderToolStripMenuItem
            // 
            this.OverwriteOlderToolStripMenuItem.Enabled = false;
            this.OverwriteOlderToolStripMenuItem.Name = "OverwriteOlderToolStripMenuItem";
            this.OverwriteOlderToolStripMenuItem.Size = new System.Drawing.Size(332, 26);
            this.OverwriteOlderToolStripMenuItem.Text = "Overwrite only events older than the age of an existing log entry";
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripProgressBar1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 742);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 19, 0);
            this.statusStrip1.Size = new System.Drawing.Size(1056, 22);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(0, 16);
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(133, 19);
            this.toolStripProgressBar1.Visible = false;
            // 
            // refashLogButton
            // 
            this.refashLogButton.Image = ((System.Drawing.Image)(resources.GetObject("refashLogButton.Image")));
            this.refashLogButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.refashLogButton.Name = "refashLogButton";
            this.refashLogButton.Size = new System.Drawing.Size(78, 24);
            this.refashLogButton.Text = "Import";
            this.refashLogButton.ToolTipText = "Import file system logs";
            this.refashLogButton.Click += new System.EventHandler(this.refreshLogButton_Click);
            // 
            // saveButton
            // 
            this.saveButton.Image = ((System.Drawing.Image)(resources.GetObject("saveButton.Image")));
            this.saveButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(64, 24);
            this.saveButton.Text = "Save";
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(6, 27);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 27);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(45, 24);
            this.toolStripLabel1.Text = "Filter:";
            // 
            // includeTextBox
            // 
            this.includeTextBox.BackColor = System.Drawing.Color.LightYellow;
            this.includeTextBox.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.includeTextBox.Name = "includeTextBox";
            this.includeTextBox.Size = new System.Drawing.Size(132, 27);
            this.includeTextBox.ToolTipText = "Contains text";
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(25, 24);
            this.toolStripLabel2.Text = "    ";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 27);
            // 
            // markButton
            // 
            this.markButton.Image = ((System.Drawing.Image)(resources.GetObject("markButton.Image")));
            this.markButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.markButton.Name = "markButton";
            this.markButton.Size = new System.Drawing.Size(69, 24);
            this.markButton.Text = "Mark:";
            this.markButton.Click += new System.EventHandler(this.markButton_Click);
            // 
            // markTextBox
            // 
            this.markTextBox.BackColor = System.Drawing.Color.LightYellow;
            this.markTextBox.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.markTextBox.Name = "markTextBox";
            this.markTextBox.Size = new System.Drawing.Size(132, 27);
            // 
            // toolStripLabel3
            // 
            this.toolStripLabel3.Name = "toolStripLabel3";
            this.toolStripLabel3.Size = new System.Drawing.Size(25, 24);
            this.toolStripLabel3.Text = "    ";
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 27);
            // 
            // toolStripLabel4
            // 
            this.toolStripLabel4.Name = "toolStripLabel4";
            this.toolStripLabel4.Size = new System.Drawing.Size(112, 24);
            this.toolStripLabel4.Text = "Display the first";
            // 
            // msgCountTextBox
            // 
            this.msgCountTextBox.BackColor = System.Drawing.Color.LightYellow;
            this.msgCountTextBox.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.msgCountTextBox.Name = "msgCountTextBox";
            this.msgCountTextBox.Size = new System.Drawing.Size(39, 27);
            this.msgCountTextBox.Text = "40";
            this.msgCountTextBox.TextBoxTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // toolStripLabel5
            // 
            this.toolStripLabel5.Name = "toolStripLabel5";
            this.toolStripLabel5.Size = new System.Drawing.Size(37, 24);
            this.toolStripLabel5.Text = "logs";
            // 
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.refashLogButton,
            this.saveButton,
            this.toolStripSeparator5,
            this.toolStripSplitButton1,
            this.toolStripSeparator2,
            this.toolStripLabel1,
            this.includeTextBox,
            this.toolStripLabel2,
            this.toolStripSeparator3,
            this.markButton,
            this.markTextBox,
            this.toolStripLabel3,
            this.toolStripSeparator4,
            this.toolStripLabel4,
            this.msgCountTextBox,
            this.toolStripLabel5});
            this.toolStrip1.Location = new System.Drawing.Point(0, 30);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1056, 27);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripSplitButton1
            // 
            this.toolStripSplitButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.autoToolStripMenuItem,
            this.manualToolStripMenuItem});
            this.toolStripSplitButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripSplitButton1.Image")));
            this.toolStripSplitButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripSplitButton1.Name = "toolStripSplitButton1";
            this.toolStripSplitButton1.Size = new System.Drawing.Size(128, 24);
            this.toolStripSplitButton1.Text = "Import Type";
            // 
            // autoToolStripMenuItem
            // 
            this.autoToolStripMenuItem.Name = "autoToolStripMenuItem";
            this.autoToolStripMenuItem.Size = new System.Drawing.Size(262, 26);
            this.autoToolStripMenuItem.Text = "Import Log Automatically";
            this.autoToolStripMenuItem.Click += new System.EventHandler(this.autoToolStripMenuItem_Click);
            // 
            // manualToolStripMenuItem
            // 
            this.manualToolStripMenuItem.Checked = true;
            this.manualToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.manualToolStripMenuItem.Name = "manualToolStripMenuItem";
            this.manualToolStripMenuItem.Size = new System.Drawing.Size(262, 26);
            this.manualToolStripMenuItem.Text = "Import Log Manually";
            this.manualToolStripMenuItem.Click += new System.EventHandler(this.manualToolStripMenuItem_Click);
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.DefaultExt = "log";
            this.saveFileDialog1.OverwritePrompt = false;
            this.saveFileDialog1.FileOk += new System.ComponentModel.CancelEventHandler(this.saveFileDialog1_FileOk);
            // 
            // EventLogForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1056, 764);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.logListView);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "EventLogForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "File System Event Viewer";
            this.Load += new System.EventHandler(this.EventLogForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.MyLog)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView logListView;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ColumnHeader dateCol;
        private System.Windows.Forms.ColumnHeader timeCol;
        private System.Windows.Forms.ColumnHeader eventCol;
        private System.Windows.Forms.ColumnHeader typeCol;
        private System.Windows.Forms.ToolStripMenuItem logToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem OptionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem maxLogSizeStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox MaximumKilobytesTextBox;
        private System.Windows.Forms.ToolStripMenuItem overFlowActionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem DoNotOverwriteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem OverwriteAsNeededToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem OverwriteOlderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadOptionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem autoRefashToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem manualRefashToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearLogToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton refashLogButton;
        private System.Windows.Forms.ToolStripButton saveButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripSplitButton toolStripSplitButton1;
        private System.Windows.Forms.ToolStripMenuItem autoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem manualToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripTextBox includeTextBox;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton markButton;
        private System.Windows.Forms.ToolStripTextBox markTextBox;
        private System.Windows.Forms.ToolStripLabel toolStripLabel3;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripLabel toolStripLabel4;
        private System.Windows.Forms.ToolStripTextBox msgCountTextBox;
        private System.Windows.Forms.ToolStripLabel toolStripLabel5;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
    }
}
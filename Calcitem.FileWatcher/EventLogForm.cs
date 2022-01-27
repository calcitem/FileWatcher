/*
 * Copyright (c) 2006 Calcitem Studio
 */

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Calcitem.FileSystemWatch
{
    public partial class EventLogForm : Form
    {
        //private BackgroundWorker backgroundWorker1;
        public EventLog MyLog;

        public EventLogForm() => InitializeComponent();

        public override void Refresh()
        {
            toolStripStatusLabel1.Text = "";
            toolStripProgressBar1.Visible = true;
            logListView.Items.Clear();
            //EventLog myLog = new EventLog();
            MyLog.Log = "File system";
            int msgCount = Gfct.ConvertStringToUInt(msgCountTextBox.Text);
            int index = 0;
            //EventLogEntryCollection entry = myLog.Entries;
            for (int i = MyLog.Entries.Count - 1; i >= 0; --i)
            //foreach (EventLogEntry entry in myLog.Entries)
            {
                string message = MyLog.Entries[i].Message;
                if (message.IndexOf(includeTextBox.Text, StringComparison.Ordinal) != -1)
                {
                    string date = MyLog.Entries[i].TimeGenerated.Date.Year + "-" +
                                  MyLog.Entries[i].TimeGenerated.Date.Month +
                                  "-" + MyLog.Entries[i].TimeGenerated.Date.Day;
                    logListView.Items.Add(date);
                    logListView.Items[index].SubItems.Add(MyLog.Entries[i].TimeGenerated.TimeOfDay.ToString());
                    logListView.Items[index].SubItems.Add(message);
                    logListView.Items[index].SubItems.Add(MyLog.Entries[i].CategoryNumber.ToString());
                    if (markTextBox.Text != "" &&
                        logListView.Items[index].SubItems[2].Text.IndexOf(markTextBox.Text, StringComparison.Ordinal) !=
                        -1)
                    {
                        logListView.Items[index].ForeColor = Color.Blue;
                    }

                    ++index;
                }

                toolStripProgressBar1.Value = index * 100 / msgCount;
                if (index == msgCount)
                {
                    break;
                }
            }

            if (logListView.Items.Count == 0)
            {
                MessageBox.Show(@"The specified item was not found.", @"Find");
            }

            toolStripProgressBar1.Visible = false;
        }

        private void refreshLogButton_Click(object sender, EventArgs e) => Refresh();

        private void logListView_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void manualRefreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (manualRefashToolStripMenuItem.Checked)
            {
                return;
            }

            manualToolStripMenuItem.Checked = true;
            manualRefashToolStripMenuItem.Checked = true;
            autoToolStripMenuItem.Checked = false;
            autoRefashToolStripMenuItem.Checked = false;
            refashLogButton.Enabled = true;
            //myLog.EntryWritten -= new EntryWrittenEventHandler(OnEntryWritten);
        }

        private void autoRefreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (autoRefashToolStripMenuItem.Checked)
            {
                return;
            }

            autoToolStripMenuItem.Checked = true;
            autoRefashToolStripMenuItem.Checked = true;
            manualToolStripMenuItem.Checked = false;
            manualRefashToolStripMenuItem.Checked = false;
            refashLogButton.Enabled = false;
        }

        private void EventLogForm_Load(object sender, EventArgs e)
        {
        }

        private void AddItem(string date, string time, string message, string categoryNumber)
        {
            logListView.Items.Insert(0, date);
            logListView.Items[0].SubItems.Add(time);
            logListView.Items[0].SubItems.Add(message);
            logListView.Items[0].SubItems.Add(categoryNumber);
            if (logListView.Items.Count > Gfct.ConvertStringToUInt(msgCountTextBox.Text))
            {
                logListView.Items[logListView.Items.Count - 1].Remove();
            }
        }

        public void OnEntryWritten(object source, EntryWrittenEventArgs e)
        {
            //this.backgroundWorker1.RunWorkerAsync();
            if (!autoRefashToolStripMenuItem.Checked)
            {
                return;
            }

            string message = e.Entry.Message;
            if (message.IndexOf(includeTextBox.Text, StringComparison.Ordinal) == -1)
            {
                return;
            }

            string date = e.Entry.TimeGenerated.Date.Year + "-" + e.Entry.TimeGenerated.Date.Month + "-" +
                          e.Entry.TimeGenerated.Date.Day;
            string time = e.Entry.TimeGenerated.TimeOfDay.ToString();
            string categoryNumber = e.Entry.CategoryNumber.ToString();
            AddItem(date, time, message, categoryNumber);
            if (message.IndexOf(markTextBox.Text, StringComparison.Ordinal) != -1 && markTextBox.Text != "")
            {
                logListView.Items[0].ForeColor = Color.Blue;
            }
        }

        private void MaximumKilobytesTextBox_Leave(object sender, EventArgs e)
        {
        }

        private void MaximumKilobytesTextBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                int mb = Gfct.ConvertStringToUInt(MaximumKilobytesTextBox.Text);
                mb -= mb % 64;
                if (mb < 64)
                {
                    mb = 64;
                }

                MyLog.MaximumKilobytes = mb;
            }
            catch (Exception)
            {
                MessageBox.Show(@"The log size must be between 64 KB and 4 GB and must be in 64K increments.", @"Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MaximumKilobytesTextBox_VisibleChanged(object sender, EventArgs e)
        {
        }

        private void clearLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            const string item = "File system";

            Gval.WriteLog = false;
            try
            {
                EventLog.Delete(item);
                //EventLog.DeleteEventSource(source);
            }
            catch (Exception)
            {
                MessageBox.Show(
                    @"Unable to delete log. \nThe log may not exist, or may be being accessed by an external program.",
                    @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void markButton_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < logListView.Items.Count; ++i)
            {
                logListView.Items[i].ForeColor = Color.Black;
            }

            if (markTextBox.Text == "")
            {
                return;
            }

            for (int i = 0; i < logListView.Items.Count; ++i)
            {
                if (logListView.Items[i].SubItems[2].Text.IndexOf(markTextBox.Text, StringComparison.Ordinal) != -1)
                {
                    logListView.Items[i].ForeColor = Color.Blue;
                }
            }
        }

        private void logListView_Click(object sender, EventArgs e) =>
            toolStripStatusLabel1.Text = logListView.SelectedItems[0].SubItems[2].Text;

        private void manualToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (manualToolStripMenuItem.Checked)
            {
                return;
            }

            manualToolStripMenuItem.Checked = true;
            manualRefashToolStripMenuItem.Checked = true;
            autoToolStripMenuItem.Checked = false;
            autoRefashToolStripMenuItem.Checked = false;
            refashLogButton.Enabled = true;
        }

        private void autoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!manualToolStripMenuItem.Checked)
            {
                return;
            }

            manualToolStripMenuItem.Checked = false;
            manualRefashToolStripMenuItem.Checked = false;
            autoToolStripMenuItem.Checked = true;
            autoRefashToolStripMenuItem.Checked = true;
            refashLogButton.Enabled = false;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            saveFileDialog1.DefaultExt = "log";
            saveFileDialog1.Filter = @"Text files (*.log)|*.log";
            DialogResult result = saveFileDialog1.ShowDialog();

            if (result != DialogResult.OK)
            {
                return;
            }

            StreamWriter sw = File.CreateText(saveFileDialog1.FileName);
            for (int i = 0; i < logListView.Items.Count; ++i)
            {
                sw.WriteLine(logListView.Items[i].SubItems[0].Text + "  " + logListView.Items[i].SubItems[1].Text +
                             "    " + logListView.Items[i].SubItems[2].Text);
            }

            sw.Close();
        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
        }
    }
}

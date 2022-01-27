/*
 * Copyright (c) 2006 Calcitem Studio
 */

using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using NJFLib.Controls;
using Raccoom.Windows.Forms;

namespace Calcitem.FileSystemWatch
{
    /// <summary>
    ///     Summary description for Form1.
    /// </summary>
    public class MainForm : Form
    {
        public MainForm()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
            components = null; //????????????????????????
        }


        private void ShowSelectedDirs()
        {
            //this.eventListView.Items.Clear();
            int index = 0;
            foreach (string s in _treeViewRecursiveChecked.SelectedDirectories)
            {
                _eventListView.Items.Add(s);
                _eventListView.Items[index].SubItems.Add("*.*");
                _eventListView.Items[index].SubItems.Add("");
                _eventListView.Items[index].SubItems.Add("");
                _eventListView.Items[index].SubItems.Add("");
                ++index;
            }
        }

        private void FillDataProviderCombo()
        {
            _cmbDataProvider.Items.Clear();
            //
            _cmbDataProvider.Items.Add(new TreeViewFolderBrowserDataProvider());
            TreeViewFolderBrowserDataProviderShell32 shell32Provider = new TreeViewFolderBrowserDataProviderShell32();
            shell32Provider.EnableContextMenu = true;
            shell32Provider.ShowAllShellObjects = true;
            _cmbDataProvider.Items.Add(shell32Provider);
            _cmbDataProvider.SelectedIndex = 1;
        }

        private void Form1_Load(object sender, EventArgs e) => CheckForIllegalCrossThreadCalls = false; //


        private void startButton_Click(object sender, EventArgs e)
        {
            if (_eventListView.CheckedItems.Count == 0)
            {
                DialogResult dr = MessageBox.Show(
                    "No files are selected.\nDo you want to select all files and start monitoring?",
                    "Prompt", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr == DialogResult.Yes)
                {
                    for (int i = 0; i < _eventListView.Items.Count; ++i)
                    {
                        _eventListView.Items[i].Checked = true;
                    }
                }
                else
                {
                    return;
                }
            }


            _monTimer.Enabled = true;
            MonType type = 0;
            if (_monChangedToolStripMenuItem.Checked)
            {
                type |= MonType.Changed;
            }

            if (_monCreatedToolStripMenuItem.Checked)
            {
                type |= MonType.Created;
            }

            if (_monDeletedToolStripMenuItem.Checked)
            {
                type |= MonType.Deleted;
            }

            if (_monRenamedToolStripMenuItem.Checked)
            {
                type |= MonType.Renamed;
            }

            Gval.ClearMonPath();

            bool shouldBeMon = false;
            for (int i = 0; i < _eventListView.CheckedItems.Count; ++i)
            {
                if (_eventListView.CheckedItems[i].SubItems[0].ForeColor != Color.Brown)
                {
                    if (Gval.HisMonPath.Any(t => t == _eventListView.CheckedItems[i].SubItems[0].Text))
                    {
                        shouldBeMon = true;
                    }

                    if (shouldBeMon == false)
                    {
                        _eventListView.CheckedItems[i].SubItems[1].Text = _filterComboBox.Text;
                        if (Watcher.Run(_eventListView.CheckedItems[i].SubItems[0].Text,
                                _eventListView.CheckedItems[i].SubItems[1].Text, type, _monSubDirMenuItem.Checked) == 0)
                        {
                            _eventListView.CheckedItems[i].SubItems[0].ForeColor = Color.Brown;
                            // Gval.monPath[i] = eventListView.CheckedItems[i].SubItems[0].Text;
                            for (int k = 0; k < Gval.HisMonPath.Length; ++k)
                            {
                                if (Gval.HisMonPath[k] == null)
                                {
                                    Gval.HisMonPath[k] = _eventListView.CheckedItems[i].SubItems[0].Text;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            _eventListView.CheckedItems[i].SubItems[4].Text = "Cannot monitor this item.";
                        }
                    }
                    else
                    {
                        _eventListView.CheckedItems[i].SubItems[0].ForeColor = Color.Brown;
                    }

                    shouldBeMon = false;
                }
            }

            for (int i = 0; i < _eventListView.Items.Count; ++i)
            {
                if (_eventListView.Items[i].ForeColor == Color.Brown)
                {
                    Gval.MonPath[i] = _eventListView.Items[i].Text;
                }
            }
        }


        private void ViewLogStripButton_Click(object sender, EventArgs e)
        {
            try
            {
                EventLogForm eventForm = new EventLogForm();
                eventForm.Show();
            }
            catch (Exception)
            {
                MessageBox.Show("The log is empty.", "Log", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void eventListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_eventListView.SelectedItems.Count > 0)
            {
                _statusBar1.Text = _eventListView.SelectedItems[0].Text + "    " +
                                   _eventListView.SelectedItems[0].SubItems[4].Text;
            }
        }

        //private void filterComboBox_TextChanged(object sender, EventArgs e)
        //{
        //    int c = eventListView.CheckedItems.Count;
        //    for (int i = 0; i < c; ++i)
        //    {

        //        //if (eventListView.CheckedItems[i].ForeColor != Color.Black)
        //        //{
        //            eventListView.CheckedItems[i].SubItems[1].Text = filterComboBox.Text;
        //        //}
        //    }
        //}

        private void MonCreatedToolStripMenuItem_Click(object sender, EventArgs e) =>
            _monCreatedToolStripMenuItem.Checked = !_monCreatedToolStripMenuItem.Checked;

        private void MonChangedToolStripMenuItem_Click(object sender, EventArgs e) =>
            _monChangedToolStripMenuItem.Checked = !_monChangedToolStripMenuItem.Checked;

        private void MonDeletedToolStripMenuItem_Click(object sender, EventArgs e) =>
            _monDeletedToolStripMenuItem.Checked = !_monDeletedToolStripMenuItem.Checked;

        private void MonRenamedToolStripMenuItem_Click(object sender, EventArgs e) =>
            _monRenamedToolStripMenuItem.Checked = !_monRenamedToolStripMenuItem.Checked;

        private void StopMonitor()
        {
            for (int i = 0; i < _eventListView.CheckedItems.Count; ++i)
            {
                //Calcitem.FileSystemWatch.Watcher.Stop(eventListView.CheckedItems[i].SubItems[0].Text, eventListView.CheckedItems[i].SubItems[1].Text);
                _eventListView.CheckedItems[i].SubItems[0].ForeColor = Color.Black;
                for (int k = 0; k < Gval.MonPath.Length; ++k)
                {
                    if (Gval.MonPath[k] == _eventListView.CheckedItems[i].SubItems[0].Text)
                    {
                        Gval.MonPath[k] = null;
                    }
                }
            }
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            if (_eventListView.CheckedItems.Count == 0)
            {
                DialogResult dr = MessageBox.Show(
                    "No items selected.\nDo you want to select all items and stop monitoring?",
                    "Prompt", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr == DialogResult.Yes)
                {
                    for (int i = 0; i < _eventListView.Items.Count; ++i)
                    {
                        _eventListView.Items[i].Checked = true;
                    }
                }
                else
                {
                    return;
                }
            }

            StopMonitor();
        }

        private void startLogButton_Click(object sender, EventArgs e)
        {
            const string source = "Calcitem Studio";
            const string item = "File System";


            if (!EventLog.SourceExists(source))
            {
                EventLog.CreateEventSource(source, item);
            }

            _startLogButton.Enabled = false;
            _stopLogButton.Enabled = true;

            Gval.WriteLog = true;
        }

        private void stopLogButton_Click(object sender, EventArgs e)
        {
            _stopLogButton.Enabled = false;
            _startLogButton.Enabled = true;
            Gval.WriteLog = false;
        }

        private void monSubDirMenuItem_Click(object sender, EventArgs e) =>
            _monSubDirMenuItem.Checked = !_monSubDirMenuItem.Checked;
        //monSubDirMenuItem.Enabled = false;

        private void highToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_highToolStripMenuItem.Checked)
            {
                return;
            }

            _standardToolStripMenuItem.Checked = false;
            _highToolStripMenuItem.Checked = true;
            _lowToolStripMenuItem.Checked = false;
            _monTimer.Interval = 1000;
        }

        private void standardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_standardToolStripMenuItem.Checked)
            {
                return;
            }

            _standardToolStripMenuItem.Checked = true;
            _highToolStripMenuItem.Checked = false;
            _lowToolStripMenuItem.Checked = false;
            _monTimer.Interval = 5000;
        }

        private void lowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_lowToolStripMenuItem.Checked)
            {
                return;
            }

            _standardToolStripMenuItem.Checked = false;
            _highToolStripMenuItem.Checked = false;
            _lowToolStripMenuItem.Checked = true;
            _monTimer.Interval = 10000;
        }

        private void monTimer_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < _eventListView.Items.Count; ++i)
            {
                if (_eventListView.Items[i].SubItems[0].ForeColor == Color.Brown)
                {
                    if ((_eventListView.Items[i].SubItems[0].Text == Gval.LatestLogEvent.Path ||
                         _eventListView.Items[i].SubItems[0].Text == Gval.LatestLogEvent.Path + "\\") &&
                        _eventListView.Items[i].SubItems[4].Text != Gval.LatestLogEvent.LatestEvent)
                    {
                        _eventListView.Items[i].SubItems[2].Text = Gval.LatestLogEvent.Date;
                        _eventListView.Items[i].SubItems[3].Text = Gval.LatestLogEvent.Time;
                        string latestEvent =
                            Gval.LatestLogEvent.LatestEvent.Replace(Gval.LatestLogEvent.Path + "\\", "");
                        _eventListView.Items[i].SubItems[4].Text = latestEvent;
                        _eventListView.Items[i].ToolTipText =
                            _eventListView.Items[i].SubItems[1].Text + "    " + latestEvent;
                    }
                }
            }
        }

        private void DeleteLog()
        {
            DialogResult dr = MessageBox.Show("Do you really want to delete the log?", "Confirm deletion",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dr != DialogResult.Yes)
            {
                return;
            }

            //const string source = "Calcitem Studio";
            const string item = "File System";
            _stopLogButton.Enabled = false;
            _startLogButton.Enabled = true;
            Gval.WriteLog = false;
            try
            {
                EventLog.Delete(item);
                //EventLog.DeleteEventSource(source);
            }
            catch (Exception)
            {
                MessageBox.Show(
                    "The log could not be deleted.\nThe log may not exist, or may be being accessed by an external program.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void clearLogButton_Click(object sender, EventArgs e) => DeleteLog();

        private void eventListView_DoubleClick(object sender, EventArgs e)
        {
            _eventListView.SelectedItems[0].Checked = !_eventListView.SelectedItems[0].Checked;
            CalcitemBrowser browser = new CalcitemBrowser();
            browser.Show();
            browser.Navigate(_eventListView.SelectedItems[0].Text);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e) => Close();

        private void contentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CalcitemBrowser browser = new CalcitemBrowser();
            browser.Show();
            browser.Navigate(Environment.CurrentDirectory + "\\Readme.htm");
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormAbout formAbout = new FormAbout();
            formAbout.Show();
        }

        private void filterComboBox_Click(object sender, EventArgs e)
        {
        }

        private void viewLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EventLogForm eventForm = new EventLogForm();
            eventForm.Show();
            eventForm.Refresh();
        }

        private void splitter1_Click(object sender, EventArgs e)
        {
        }

        private void splitter1_SplitterMoved(object sender, SplitterEventArgs e)
        {
        }

        #region fields

        private TreeViewFolderBrowser _treeViewRecursiveChecked;
        private StatusBar _statusBar1;
        private ComboBox _cmbDataProvider;
        private CollapsibleSplitter _splitter1;
        private MenuStrip _menuStrip1;
        private ToolStripMenuItem _fileToolStripMenuItem;
        private ToolStripMenuItem _viewToolStripMenuItem;
        private ToolStripMenuItem _helpToolStripMenuItem;
        private ToolStripMenuItem _contentToolStripMenuItem;
        private ToolStripMenuItem _aboutToolStripMenuItem;
        private ListView _eventListView;
        private ColumnHeader _dirCol;
        private ColumnHeader _fileCol;
        private ColumnHeader _dataCol;
        private ColumnHeader _timeCol;
        private ColumnHeader _eventCol;
        private ToolStrip _toolStrip1;
        private ToolStripButton _startButton;
        private ToolStripLabel _toolStripLabel1;
        private ToolStripButton _stopButton;
        private ToolStripLabel _toolStripLabel3;
        private ToolStripSeparator _toolStripSeparator1;
        private ToolStripLabel _toolStripLabel2;
        private ToolStripButton _startLogButton;
        private ToolStripButton _stopLogButton;
        private ToolStripButton _clearLogButton;
        private ToolStripComboBox _filterComboBox;
        private ToolStripSeparator _toolStripSeparator2;
        private ToolStripLabel _toolStripLabel4;
        private ToolStripDropDownButton _monTypeButton;
        private ToolStripMenuItem _monChangedToolStripMenuItem;
        private ToolStripMenuItem _monCreatedToolStripMenuItem;
        private ToolStripMenuItem _monDeletedToolStripMenuItem;
        private ToolStripMenuItem _monRenamedToolStripMenuItem;
        private ToolStripButton _viewLogStripButton;
        private ToolStripMenuItem _optionMenuItem1;
        private ToolStripMenuItem _monSubDirMenuItem;
        private ToolStripMenuItem _freToolStripMenuItem;
        private ToolStripMenuItem _highToolStripMenuItem;
        private ToolStripMenuItem _standardToolStripMenuItem;
        private ToolStripMenuItem _lowToolStripMenuItem;
        private Timer _monTimer;
        private ToolStripMenuItem _exitToolStripMenuItem;
        private ToolStripMenuItem _viewLogToolStripMenuItem;
        private IContainer components;

        #endregion

        #region events

        private void _cmbDataProvider_SelectionChangeCommitted(object sender, EventArgs e) =>
            _treeViewRecursiveChecked.DataSource = _cmbDataProvider.SelectedItem as ITreeViewFolderBrowserDataProvider;

        private void treeViewRecursiveChecked_SelectedDirectoriesChanged(object sender,
            SelectedDirectoriesChangedEventArgs e)
        {
            _statusBar1.Text = e.Path; // +" is now " + e.CheckState.ToString();
            if (e.CheckState == CheckState.Checked)
            {
                _eventListView.Items.Add(e.Path);
                _eventListView.Items[_eventListView.Items.Count - 1].SubItems.Add(_filterComboBox.Text);
                _eventListView.Items[_eventListView.Items.Count - 1].SubItems.Add("");
                _eventListView.Items[_eventListView.Items.Count - 1].SubItems.Add("");
                _eventListView.Items[_eventListView.Items.Count - 1].SubItems.Add("");
            }
            else
            {
                for (int i = 0; i < _eventListView.Items.Count; ++i)
                {
                    if (_eventListView.Items[i].Text == e.Path)
                    {
                        for (int k = 0; k < Gval.MonPath.Length; ++k)
                        {
                            if (Gval.MonPath[k] == _eventListView.Items[i].SubItems[0].Text)
                            {
                                Gval.MonPath[k] = null;
                            }
                        }

                        _eventListView.Items[i].Remove();
                        break;
                    }
                }
            }
        }

        private void treeViewRecursiveChecked_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node is TreeNodePath node)
            {
                _statusBar1.Text = node.Path;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            if (!DesignMode)
            {
                FileStream stream = new FileStream("list", FileMode.OpenOrCreate);
                try
                {
                    BinaryFormatter binary = new BinaryFormatter();
                    _treeViewRecursiveChecked.SelectedDirectories = binary.Deserialize(stream) as StringCollection;
                }
                catch
                {
                }

                stream.Close();

                FillDataProviderCombo();


                _treeViewRecursiveChecked.DataSource = _cmbDataProvider.Items[1] as ITreeViewFolderBrowserDataProvider;


                _treeViewRecursiveChecked.RootFolder = Environment.SpecialFolder.MyComputer;

                _treeViewRecursiveChecked.Populate(Environment.SpecialFolder.ApplicationData);

                Text = Application.CompanyName + " " + Application.ProductName + " " +
                       Application.ProductVersion.Substring(0, 3);

                ShowSelectedDirs();
            }

            base.OnLoad(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            FileStream stream = new FileStream("list", FileMode.Create);
            BinaryFormatter binary = new BinaryFormatter();
            binary.Serialize(stream, _treeViewRecursiveChecked.SelectedDirectories);
            stream.Close();
            //
            base.OnClosed(e);
        }

        #endregion

        #region Windows Form Designer generated code

        /// <summary>
        ///     Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }

            base.Dispose(disposing);
        }


        /// <summary>
        ///     Required method for Designer support - do not modify
        ///     the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources =
                new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this._statusBar1 = new System.Windows.Forms.StatusBar();
            this._cmbDataProvider = new System.Windows.Forms.ComboBox();
            this._menuStrip1 = new System.Windows.Forms.MenuStrip();
            this._fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._viewLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._optionMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this._monSubDirMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._freToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._highToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._standardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._lowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._contentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._eventListView = new System.Windows.Forms.ListView();
            this._dirCol = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this._fileCol = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this._dataCol = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this._timeCol = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this._eventCol = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this._toolStrip1 = new System.Windows.Forms.ToolStrip();
            this._toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this._startButton = new System.Windows.Forms.ToolStripButton();
            this._stopButton = new System.Windows.Forms.ToolStripButton();
            this._toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this._monTypeButton = new System.Windows.Forms.ToolStripDropDownButton();
            this._monChangedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._monCreatedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._monDeletedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._monRenamedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._toolStripLabel4 = new System.Windows.Forms.ToolStripLabel();
            this._filterComboBox = new System.Windows.Forms.ToolStripComboBox();
            this._toolStripLabel3 = new System.Windows.Forms.ToolStripLabel();
            this._toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this._toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this._viewLogStripButton = new System.Windows.Forms.ToolStripButton();
            this._startLogButton = new System.Windows.Forms.ToolStripButton();
            this._stopLogButton = new System.Windows.Forms.ToolStripButton();
            this._clearLogButton = new System.Windows.Forms.ToolStripButton();
            this._monTimer = new System.Windows.Forms.Timer(this.components);
            this._treeViewRecursiveChecked = new Raccoom.Windows.Forms.TreeViewFolderBrowser();
            this._splitter1 = new NJFLib.Controls.CollapsibleSplitter();
            this._menuStrip1.SuspendLayout();
            this._toolStrip1.SuspendLayout();
            this.SuspendLayout();
            //
            // statusBar1
            //
            this._statusBar1.Location = new System.Drawing.Point(0, 513);
            this._statusBar1.Name = "_statusBar1";
            this._statusBar1.Size = new System.Drawing.Size(792, 22);
            this._statusBar1.TabIndex = 7;
            //
            // _cmbDataProvider
            //
            this._cmbDataProvider.Anchor =
                ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top |
                                                      System.Windows.Forms.AnchorStyles.Right)));
            this._cmbDataProvider.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cmbDataProvider.Location = new System.Drawing.Point(370, 291);
            this._cmbDataProvider.Name = "_cmbDataProvider";
            this._cmbDataProvider.Size = new System.Drawing.Size(221, 24);
            this._cmbDataProvider.TabIndex = 16;
            this._cmbDataProvider.Visible = false;
            this._cmbDataProvider.SelectionChangeCommitted +=
                new System.EventHandler(this._cmbDataProvider_SelectionChangeCommitted);
            //
            // menuStrip1
            //
            this._menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this._menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[]
            {
                this._fileToolStripMenuItem, this._viewToolStripMenuItem, this._optionMenuItem1,
                this._helpToolStripMenuItem
            });
            this._menuStrip1.Location = new System.Drawing.Point(0, 0);
            this._menuStrip1.Name = "_menuStrip1";
            this._menuStrip1.Size = new System.Drawing.Size(792, 28);
            this._menuStrip1.TabIndex = 23;
            this._menuStrip1.Text = "menuStrip1";
            //
            // FileToolStripMenuItem
            //
            this._fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[]
            {
                this._exitToolStripMenuItem
            });
            this._fileToolStripMenuItem.Name = "_fileToolStripMenuItem";
            this._fileToolStripMenuItem.Size = new System.Drawing.Size(46, 24);
            this._fileToolStripMenuItem.Text = "&File";
            //
            // exitToolStripMenuItem
            //
            this._exitToolStripMenuItem.Name = "_exitToolStripMenuItem";
            this._exitToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this._exitToolStripMenuItem.Text = "E&xit";
            this._exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            //
            // ViewToolStripMenuItem
            //
            this._viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[]
            {
                this._viewLogToolStripMenuItem
            });
            this._viewToolStripMenuItem.Name = "_viewToolStripMenuItem";
            this._viewToolStripMenuItem.Size = new System.Drawing.Size(55, 24);
            this._viewToolStripMenuItem.Text = "&View";
            //
            // viewLogToolStripMenuItem
            //
            this._viewLogToolStripMenuItem.Name = "_viewLogToolStripMenuItem";
            this._viewLogToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this._viewLogToolStripMenuItem.Text = "&Log";
            this._viewLogToolStripMenuItem.Click += new System.EventHandler(this.viewLogToolStripMenuItem_Click);
            //
            // optionMenuItem1
            //
            this._optionMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[]
            {
                this._monSubDirMenuItem, this._freToolStripMenuItem
            });
            this._optionMenuItem1.Name = "_optionMenuItem1";
            this._optionMenuItem1.Size = new System.Drawing.Size(69, 24);
            this._optionMenuItem1.Text = "&Option";
            //
            // monSubDirMenuItem
            //
            this._monSubDirMenuItem.Name = "_monSubDirMenuItem";
            this._monSubDirMenuItem.Size = new System.Drawing.Size(224, 26);
            this._monSubDirMenuItem.Text = "Watch &subfolders";
            this._monSubDirMenuItem.Click += new System.EventHandler(this.monSubDirMenuItem_Click);
            //
            // freToolStripMenuItem
            //
            this._freToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[]
            {
                this._highToolStripMenuItem, this._standardToolStripMenuItem, this._lowToolStripMenuItem
            });
            this._freToolStripMenuItem.Image = global::Calcitem.FileSystemWatch.Properties.Resources.ExpirationHS;
            this._freToolStripMenuItem.Name = "_freToolStripMenuItem";
            this._freToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this._freToolStripMenuItem.Text = "&Update speed";
            //
            // highToolStripMenuItem
            //
            this._highToolStripMenuItem.Name = "_highToolStripMenuItem";
            this._highToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this._highToolStripMenuItem.Text = "&High";
            this._highToolStripMenuItem.Click += new System.EventHandler(this.highToolStripMenuItem_Click);
            //
            // standardToolStripMenuItem
            //
            this._standardToolStripMenuItem.Checked = true;
            this._standardToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this._standardToolStripMenuItem.Name = "_standardToolStripMenuItem";
            this._standardToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this._standardToolStripMenuItem.Text = "&Normal";
            this._standardToolStripMenuItem.Click += new System.EventHandler(this.standardToolStripMenuItem_Click);
            //
            // lowToolStripMenuItem
            //
            this._lowToolStripMenuItem.Name = "_lowToolStripMenuItem";
            this._lowToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this._lowToolStripMenuItem.Text = "&Low";
            this._lowToolStripMenuItem.Click += new System.EventHandler(this.lowToolStripMenuItem_Click);
            //
            // HelpToolStripMenuItem
            //
            this._helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[]
            {
                this._contentToolStripMenuItem, this._aboutToolStripMenuItem
            });
            this._helpToolStripMenuItem.Name = "_helpToolStripMenuItem";
            this._helpToolStripMenuItem.Size = new System.Drawing.Size(55, 24);
            this._helpToolStripMenuItem.Text = "&Help";
            //
            // contentToolStripMenuItem
            //
            this._contentToolStripMenuItem.Image =
                global::Calcitem.FileSystemWatch.Properties.Resources.ArrangeSideBySideHS;
            this._contentToolStripMenuItem.Name = "_contentToolStripMenuItem";
            this._contentToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this._contentToolStripMenuItem.Text = "&Folder";
            this._contentToolStripMenuItem.Click += new System.EventHandler(this.contentToolStripMenuItem_Click);
            //
            // aboutToolStripMenuItem
            //
            this._aboutToolStripMenuItem.Name = "_aboutToolStripMenuItem";
            this._aboutToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this._aboutToolStripMenuItem.Text = "&About";
            this._aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            //
            // eventListView
            //
            this._eventListView.Anchor =
                ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top |
                                                        System.Windows.Forms.AnchorStyles.Bottom)
                                                       | System.Windows.Forms.AnchorStyles.Left)
                                                      | System.Windows.Forms.AnchorStyles.Right)));
            this._eventListView.CheckBoxes = true;
            this._eventListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[]
            {
                this._dirCol, this._fileCol, this._dataCol, this._timeCol, this._eventCol
            });
            this._eventListView.FullRowSelect = true;
            this._eventListView.GridLines = true;
            this._eventListView.HideSelection = false;
            this._eventListView.LabelEdit = true;
            this._eventListView.Location = new System.Drawing.Point(254, 56);
            this._eventListView.Name = "_eventListView";
            this._eventListView.Size = new System.Drawing.Size(538, 457);
            this._eventListView.TabIndex = 26;
            this._eventListView.UseCompatibleStateImageBehavior = false;
            this._eventListView.View = System.Windows.Forms.View.Details;
            this._eventListView.SelectedIndexChanged +=
                new System.EventHandler(this.eventListView_SelectedIndexChanged);
            this._eventListView.DoubleClick += new System.EventHandler(this.eventListView_DoubleClick);
            //
            // dirCol
            //
            this._dirCol.Text = "Folder";
            this._dirCol.Width = 108;
            //
            // fileCol
            //
            this._fileCol.Text = "File";
            this._fileCol.Width = 67;
            //
            // dataCol
            //
            this._dataCol.Text = "Date";
            //
            // timeCol
            //
            this._timeCol.Text = "Time";
            //
            // eventCol
            //
            this._eventCol.Text = "Event";
            this._eventCol.Width = 204;
            //
            // toolStrip1
            //
            this._toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this._toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[]
            {
                this._toolStripLabel1, this._startButton, this._stopButton, this._toolStripSeparator2,
                this._monTypeButton, this._toolStripLabel4, this._filterComboBox, this._toolStripLabel3,
                this._toolStripSeparator1, this._toolStripLabel2, this._viewLogStripButton, this._startLogButton,
                this._stopLogButton, this._clearLogButton
            });
            this._toolStrip1.Location = new System.Drawing.Point(8, 28);
            this._toolStrip1.Name = "_toolStrip1";
            this._toolStrip1.Size = new System.Drawing.Size(784, 28);
            this._toolStrip1.TabIndex = 27;
            this._toolStrip1.Text = "toolStrip1";
            //
            // toolStripLabel1
            //
            this._toolStripLabel1.ForeColor = System.Drawing.Color.MidnightBlue;
            this._toolStripLabel1.Name = "_toolStripLabel1";
            this._toolStripLabel1.Size = new System.Drawing.Size(50, 25);
            this._toolStripLabel1.Text = "Watch";
            //
            // startButton
            //
            this._startButton.Image = ((System.Drawing.Image)(resources.GetObject("startButton.Image")));
            this._startButton.ImageTransparentColor = System.Drawing.Color.WhiteSmoke;
            this._startButton.Name = "_startButton";
            this._startButton.Size = new System.Drawing.Size(64, 25);
            this._startButton.Text = "Start";
            this._startButton.Click += new System.EventHandler(this.startButton_Click);
            //
            // stopButton
            //
            this._stopButton.Image = ((System.Drawing.Image)(resources.GetObject("stopButton.Image")));
            this._stopButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._stopButton.Name = "_stopButton";
            this._stopButton.Size = new System.Drawing.Size(64, 25);
            this._stopButton.Text = "Stop";
            this._stopButton.Click += new System.EventHandler(this.stopButton_Click);
            //
            // toolStripSeparator2
            //
            this._toolStripSeparator2.Name = "_toolStripSeparator2";
            this._toolStripSeparator2.Size = new System.Drawing.Size(6, 28);
            //
            // MonTypeButton
            //
            this._monTypeButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[]
            {
                this._monChangedToolStripMenuItem, this._monCreatedToolStripMenuItem,
                this._monDeletedToolStripMenuItem, this._monRenamedToolStripMenuItem
            });
            this._monTypeButton.Image = ((System.Drawing.Image)(resources.GetObject("MonTypeButton.Image")));
            this._monTypeButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._monTypeButton.Name = "_monTypeButton";
            this._monTypeButton.Size = new System.Drawing.Size(86, 25);
            this._monTypeButton.Text = "Action";
            //
            // MonChangedToolStripMenuItem
            //
            this._monChangedToolStripMenuItem.Checked = true;
            this._monChangedToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this._monChangedToolStripMenuItem.Name = "_monChangedToolStripMenuItem";
            this._monChangedToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this._monChangedToolStripMenuItem.Text = "&Modify";
            this._monChangedToolStripMenuItem.Click += new System.EventHandler(this.MonChangedToolStripMenuItem_Click);
            //
            // MonCreatedToolStripMenuItem
            //
            this._monCreatedToolStripMenuItem.Checked = true;
            this._monCreatedToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this._monCreatedToolStripMenuItem.Name = "_monCreatedToolStripMenuItem";
            this._monCreatedToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this._monCreatedToolStripMenuItem.Text = "&Create";
            this._monCreatedToolStripMenuItem.Click += new System.EventHandler(this.MonCreatedToolStripMenuItem_Click);
            //
            // MonDeletedToolStripMenuItem
            //
            this._monDeletedToolStripMenuItem.Checked = true;
            this._monDeletedToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this._monDeletedToolStripMenuItem.Name = "_monDeletedToolStripMenuItem";
            this._monDeletedToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this._monDeletedToolStripMenuItem.Text = "&Delete";
            this._monDeletedToolStripMenuItem.Click += new System.EventHandler(this.MonDeletedToolStripMenuItem_Click);
            //
            // MonRenamedToolStripMenuItem
            //
            this._monRenamedToolStripMenuItem.Checked = true;
            this._monRenamedToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this._monRenamedToolStripMenuItem.Name = "_monRenamedToolStripMenuItem";
            this._monRenamedToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this._monRenamedToolStripMenuItem.Text = "&Rename";
            this._monRenamedToolStripMenuItem.Click += new System.EventHandler(this.MonRenamedToolStripMenuItem_Click);
            //
            // toolStripLabel4
            //
            this._toolStripLabel4.Name = "_toolStripLabel4";
            this._toolStripLabel4.Size = new System.Drawing.Size(48, 25);
            this._toolStripLabel4.Text = "  Type";
            //
            // filterComboBox
            //
            this._filterComboBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this._filterComboBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this._filterComboBox.BackColor = System.Drawing.Color.LightCyan;
            this._filterComboBox.DropDownWidth = 85;
            this._filterComboBox.Items.AddRange(new object[]
            {
                "*.*", "*.exe", "*.dll", "*.ini", "*.sys", "*.txt", "*.doc", "*.log"
            });
            this._filterComboBox.Name = "_filterComboBox";
            this._filterComboBox.Size = new System.Drawing.Size(75, 28);
            this._filterComboBox.Text = "*.*";
            this._filterComboBox.ToolTipText = "File Type";
            this._filterComboBox.Click += new System.EventHandler(this.filterComboBox_Click);
            //
            // toolStripLabel3
            //
            this._toolStripLabel3.Name = "_toolStripLabel3";
            this._toolStripLabel3.Size = new System.Drawing.Size(25, 25);
            this._toolStripLabel3.Text = "    ";
            //
            // toolStripSeparator1
            //
            this._toolStripSeparator1.Name = "_toolStripSeparator1";
            this._toolStripSeparator1.Size = new System.Drawing.Size(6, 28);
            //
            // toolStripLabel2
            //
            this._toolStripLabel2.ForeColor = System.Drawing.Color.MidnightBlue;
            this._toolStripLabel2.Name = "_toolStripLabel2";
            this._toolStripLabel2.Size = new System.Drawing.Size(34, 25);
            this._toolStripLabel2.Text = "Log";
            //
            // ViewLogStripButton
            //
            this._viewLogStripButton.Image = ((System.Drawing.Image)(resources.GetObject("ViewLogStripButton.Image")));
            this._viewLogStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._viewLogStripButton.Name = "_viewLogStripButton";
            this._viewLogStripButton.Size = new System.Drawing.Size(65, 25);
            this._viewLogStripButton.Text = "View";
            this._viewLogStripButton.Click += new System.EventHandler(this.ViewLogStripButton_Click);
            //
            // startLogButton
            //
            this._startLogButton.Image = ((System.Drawing.Image)(resources.GetObject("startLogButton.Image")));
            this._startLogButton.ImageTransparentColor = System.Drawing.Color.WhiteSmoke;
            this._startLogButton.Name = "_startLogButton";
            this._startLogButton.Size = new System.Drawing.Size(78, 25);
            this._startLogButton.Text = "Enable";
            this._startLogButton.Click += new System.EventHandler(this.startLogButton_Click);
            //
            // stopLogButton
            //
            this._stopLogButton.Enabled = false;
            this._stopLogButton.Image = ((System.Drawing.Image)(resources.GetObject("stopLogButton.Image")));
            this._stopLogButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._stopLogButton.Name = "_stopLogButton";
            this._stopLogButton.Size = new System.Drawing.Size(83, 25);
            this._stopLogButton.Text = "Disable";
            this._stopLogButton.Click += new System.EventHandler(this.stopLogButton_Click);
            //
            // clearLogButton
            //
            this._clearLogButton.Image = ((System.Drawing.Image)(resources.GetObject("clearLogButton.Image")));
            this._clearLogButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._clearLogButton.Name = "_clearLogButton";
            this._clearLogButton.Size = new System.Drawing.Size(67, 25);
            this._clearLogButton.Text = "Clear";
            this._clearLogButton.Click += new System.EventHandler(this.clearLogButton_Click);
            //
            // monTimer
            //
            this._monTimer.Interval = 5000;
            this._monTimer.Tick += new System.EventHandler(this.monTimer_Tick);
            //
            // treeViewRecursiveChecked
            //
            this._treeViewRecursiveChecked.Anchor =
                ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top |
                                                       System.Windows.Forms.AnchorStyles.Bottom)
                                                      | System.Windows.Forms.AnchorStyles.Left)));
            this._treeViewRecursiveChecked.BackColor = System.Drawing.Color.White;
            this._treeViewRecursiveChecked.DataSource = null;
            this._treeViewRecursiveChecked.ForeColor = System.Drawing.Color.Black;
            this._treeViewRecursiveChecked.HideSelection = false;
            this._treeViewRecursiveChecked.LineColor = System.Drawing.Color.MidnightBlue;
            this._treeViewRecursiveChecked.Location = new System.Drawing.Point(0, 56);
            this._treeViewRecursiveChecked.Name = "_treeViewRecursiveChecked";
            this._treeViewRecursiveChecked.SelectedDirectories =
                ((System.Collections.Specialized.StringCollection)(resources.GetObject(
                    "treeViewRecursiveChecked.SelectedDirectories")));
            this._treeViewRecursiveChecked.Size = new System.Drawing.Size(248, 457);
            this._treeViewRecursiveChecked.TabIndex = 0;
            this._treeViewRecursiveChecked.SelectedDirectoriesChanged +=
                new Raccoom.Windows.Forms.SelectedDirectoriesChangedDelegate(this
                    .treeViewRecursiveChecked_SelectedDirectoriesChanged);
            this._treeViewRecursiveChecked.AfterSelect +=
                new System.Windows.Forms.TreeViewEventHandler(this.treeViewRecursiveChecked_AfterSelect);
            //
            // splitter1
            //
            this._splitter1.AnimationDelay = 20;
            this._splitter1.AnimationStep = 20;
            this._splitter1.BorderStyle3D = System.Windows.Forms.Border3DStyle.Flat;
            this._splitter1.ControlToHide = this._treeViewRecursiveChecked;
            this._splitter1.ExpandParentForm = false;
            this._splitter1.Location = new System.Drawing.Point(0, 28);
            this._splitter1.Name = "_splitter1";
            this._splitter1.TabIndex = 18;
            this._splitter1.TabStop = false;
            this._splitter1.UseAnimations = false;
            this._splitter1.VisualStyle = NJFLib.Controls.VisualStyles.Mozilla;
            this._splitter1.SplitterMoved +=
                new System.Windows.Forms.SplitterEventHandler(this.splitter1_SplitterMoved);
            this._splitter1.Click += new System.EventHandler(this.splitter1_Click);
            //
            // Form1
            //
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 15);
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(792, 535);
            this.Controls.Add(this._toolStrip1);
            this.Controls.Add(this._eventListView);
            this.Controls.Add(this._splitter1);
            this.Controls.Add(this._cmbDataProvider);
            this.Controls.Add(this._treeViewRecursiveChecked);
            this.Controls.Add(this._statusBar1);
            this.Controls.Add(this._menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this._menuStrip1;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Calcitem FileSystem Monitor";
            this.Load += new System.EventHandler(this.Form1_Load);
            this._menuStrip1.ResumeLayout(false);
            this._menuStrip1.PerformLayout();
            this._toolStrip1.ResumeLayout(false);
            this._toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion
    }
}

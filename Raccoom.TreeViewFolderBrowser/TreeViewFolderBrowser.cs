// Copyright ?2004 by Christoph Richner. All rights are reserved.
// 
// If you like this code then feel free to go ahead and use it.
// The only thing I ask is that you don't remove or alter my copyright notice.
//
// Your use of this software is entirely at your own risk. I make no claims or
// warrantees about the reliability or fitness of this code for any particular purpose.
// If you make changes or additions to this code please mark your code as being yours.
// 
// website http://raccoom.sytes.net, email microweb@bluewin.ch, msn chrisdarebell@msn.com

using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Raccoom.Windows.Forms.Design;

namespace Raccoom.Windows.Forms
{
    /// <summary>
    ///     TreeViewFolderBrowser works a bite like FolderBrowserDialog but was designed to let the user choose many
    ///     directories by <c>Chechboxes</c>.
    ///     <seealso cref="DriveTypes" /><seealso cref="CheckboxBehaviorMode" /><seealso cref="TreeNodePath" />
    /// </summary>
    /// <remarks>
    ///     Because this class delegates the drive, folder and ImageList specific task's to a
    ///     <see cref="ITreeViewFolderBrowserDataProvider" /> instance, this class needs a wired
    ///     <see cref="ITreeViewFolderBrowserDataProvider" /> instance before you can call any method wich fill's the tree
    ///     view.
    /// </remarks>
    [Designer(typeof(TreeViewFolderBrowserDesigner))]
    [ToolboxBitmap(typeof(TreeView))]
    [DefaultProperty("CheckboxBehaviorMode")]
    [DefaultEvent("SelectedDirectoriesChanged")]
    public sealed class TreeViewFolderBrowser : TreeView
    {
        #region constructors

        /// <summary>
        ///     Required designer variable.
        /// </summary>
        public TreeViewFolderBrowser()
        {
            InitializeComponent();
            // initalize a new helper instance for this tree view.
            _helper = new TreeViewFolderBrowserHelper(this);
            //
            ContextMenu = new ContextMenu();
            ContextMenu.Popup += OnContextMenu_Popup;
            //
            _driveTypes = DriveTypes.NoRootDirectory | DriveTypes.RemovableDisk | DriveTypes.LocalDisk |
                          DriveTypes.NetworkDrive | DriveTypes.CompactDisc | DriveTypes.RamDisk;
            RootFolder = Environment.SpecialFolder.MyComputer;
            CheckboxBehaviorMode = CheckboxBehaviorMode.SingleChecked;
            // init bold font
            _boldFont = new Font(Font, FontStyle.Bold);
        }

        #endregion

        #region Windows Form Designer generated code

        /// <summary>
        ///     Required method for Designer support - do not modify
        ///     the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            // 
            // TreeViewFolderBrowser
            // 
            this.HideSelection = false;
        }

        #endregion

        #region fields

        /// <summary>
        ///     Fired if a directory was selected or deselected.
        /// </summary>
        public event SelectedDirectoriesChangedDelegate SelectedDirectoriesChanged;

        /// <summary>
        ///     Fired if a data provider has changed
        /// </summary>
        public EventHandler DataSourceChanged;

        /// <summary>
        ///     Fired if a CheckboxBehaviorMode has changed
        /// </summary>
        public EventHandler CheckboxBehaviorModeChanged;

        /// <summary>
        ///     Fired if a drive types has changed
        /// </summary>
        public EventHandler DriveTypesChanged;

        /// <summary>holds the path list</summary>
        private StringCollection _folderList;

        /// <summary>flag used to suppress CheckItem Event</summary>
        private int _supressCheck;

        /// <summary>font used to mark nodes which contains checked sub nodes</summary>
        private readonly Font _boldFont;

        /// <summary>current working mode</summary>
        private CheckboxBehaviorMode _checkboxBehavior;

        /// <summary>designer</summary>
        private Environment.SpecialFolder _specialFolderRootFolder;

        /// <summary>Specify which drive types are displayed.</summary>
        private DriveTypes _driveTypes;

        /// <summary>data provider which is responsible to manage this instance</summary>
        private ITreeViewFolderBrowserDataProvider _dataProvider;

        /// <summary>data provider helper instance</summary>
        private readonly TreeViewFolderBrowserHelper _helper;

        #endregion

        #region public interface

        /// <summary>
        ///     Gets or sets <see cref="ITreeViewFolderBrowserDataProvider" /> which is responsible to fill this
        ///     <c>TreeViewFolderBrowser</c> instance.
        /// </summary>
        [Browsable(true)]
        [Category("FolderBrowser")]
        [Description("DataSource specifies the DataProvider which is responsible to fill this instance")]
        [DefaultValue(Environment.SpecialFolder.MyComputer)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public ITreeViewFolderBrowserDataProvider DataSource
        {
            get => _dataProvider;
            set
            {
                bool changed = !Equals(_dataProvider, value);
                _dataProvider = value;
                //
                ImageList = _dataProvider?.ImageList;
                //
                if (changed)
                {
                    OnDataSourceChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        ///     Gets or sets the root folder where the browsing starts from.
        /// </summary>
        [Browsable(true)]
        [Category("FolderBrowser")]
        [Description(
            "Only the specified folder and any subfolders that are beneath it will appear in the dialog box and be selectable.")]
        [DefaultValue(Environment.SpecialFolder.MyComputer)]
        public Environment.SpecialFolder RootFolder
        {
            get => _specialFolderRootFolder;
            set => _specialFolderRootFolder = value;
        }

        /// <summary>
        ///     List contains the path for all checked items.
        /// </summary>
        [Browsable(false)]
        public StringCollection SelectedDirectories
        {
            get => _folderList ?? (_folderList = new StringCollection());
            set => _folderList = value;
        }

        /// <summary>
        ///     Specify how the tree view handles checkboxes and associated events.
        /// </summary>
        [Browsable(true)]
        [Category("FolderBrowser")]
        [Description("Specify how the tree view handles checkboxes and associated events")]
        [DefaultValue(CheckboxBehaviorMode.SingleChecked)]
        public CheckboxBehaviorMode CheckboxBehaviorMode
        {
            get => _checkboxBehavior;
            set
            {
                _checkboxBehavior = value;
                //
                OnCheckboxBehaviorModeChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        ///     Specify which drive types are displayed.
        /// </summary>
        [Browsable(true)]
        [Category("FolderBrowser")]
        [Description("Specify which drive types are displayed")]
        [DefaultValue(DriveTypes.All)]
        public DriveTypes DriveTypes
        {
            get => _driveTypes;
            set
            {
                _driveTypes = value;
                //
                OnDriveTypesChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        ///     Clears the TreeView and popluates the root level.
        /// </summary>
        /// <param name="specialFolder">The <c> SpecialFolder</c> which should be selected after population. </param>
        public void Populate(Environment.SpecialFolder specialFolder) =>
            Populate(Environment.GetFolderPath(specialFolder));

        /// <summary>
        ///     Clears the TreeView and popluates the root level.
        /// </summary>
        /// <param name="selectedFolderPath">The path of the folder that should be selected after population.</param>
        public void Populate(string selectedFolderPath)
        {
            // clear out the old values
            BeginUpdate();
            Nodes.Clear();
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                CheckBoxes = _checkboxBehavior != CheckboxBehaviorMode.None;
                _dataProvider.RequestRoot(_helper);
            }
            finally
            {
                EndUpdate();
                Cursor.Current = Cursors.Default;
            }

            // open selected folder
            if (!string.IsNullOrEmpty(selectedFolderPath))
            {
                ShowFolder(selectedFolderPath);
            }
        }

        /// <summary>
        ///     Focus the specified folder and scroll it in to view.
        /// </summary>
        /// <param name="directoryPath">The path which should be focused</param>
        public void ShowFolder(string directoryPath)
        {
            if (string.IsNullOrEmpty(directoryPath) || directoryPath == string.Empty)
            {
                return;
            }

            // start search at root node
            TreeNodeCollection nodeCol = _dataProvider.RequestDriveCollection(_helper);
            //
            if (!Directory.Exists(directoryPath) || nodeCol == null)
            {
                return;
            }

            //
            DirectoryInfo dirInfo = new DirectoryInfo(directoryPath);
            // get path tokens
            ArrayList dirs = new ArrayList { dirInfo.FullName };
            //
            while (dirInfo.Parent != null)
            {
                dirs.Add(dirInfo.Parent.FullName);
                //
                dirInfo = dirInfo.Parent;
            }

            // try to expand all path tokens
            Cursor.Current = Cursors.WaitCursor;
            BeginUpdate();
            // load on demand was not fired till now
            if (nodeCol.Count == 1 && ((TreeNodePath)nodeCol[0]).Path == null)
            {
                nodeCol[0].Parent.Expand();
            }

            try
            {
                //			
                for (int i = dirs.Count - 1; i >= 0; i--)
                {
                    foreach (TreeNodePath n in nodeCol)
                    {
                        if (string.Compare(n.Path.ToLower(), dirs[i].ToString().ToLower(), StringComparison.Ordinal) !=
                            0)
                        {
                            continue;
                        }

                        nodeCol = n.Nodes;
                        if (i == 0)
                        {
                            n.EnsureVisible();
                            SelectedNode = n;
                        }
                        else
                        {
                            n.Expand();
                        }

                        break;
                    }
                }
            }
            finally
            {
                EndUpdate();
                Cursor.Current = Cursors.Default;
            }
        }

        #endregion

        #region internal interface

        private void OnContextMenu_Popup(object sender, EventArgs e) => OnContextMenuPopup();

        private void OnContextMenuPopup()
        {
            if (_dataProvider == null)
            {
                return;
            }

            //
            ContextMenu.MenuItems.Clear();
            //			
            if (!(_helper.TreeView.GetNodeAt(_helper.TreeView.PointToClient(Cursor.Position)) is TreeNodePath node))
            {
                return;
            }

            //
            _dataProvider.QueryContextMenuItems(_helper, node);
        }

        /// <summary>
        ///     True to supress OnBeforeCheck Execution, otherwise false to allow it.
        /// </summary>
        /// <param name="supressEvent"></param>
        internal void SupressCheckEvent(bool supressEvent) => _supressCheck += supressEvent ? +1 : -1;

        /// <summary>
        ///     Indicates if OnBeforeCheck is permitted to call code
        /// </summary>
        private bool IsCheckEventSupressed => _supressCheck != 0;

        /// <summary>
        ///     Populates the Directory structure for a given path.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="e"></param>
        private void GetSubDirs(TreeNodePath parent, TreeViewCancelEventArgs e)
        {
            if (parent.Path == null)
            {
                return;
            }

            // everything ok, here we go
            BeginUpdate();
            try
            {
                parent.RemoveDummyNode();
                // if we have not scanned this folder before
                if (parent.Nodes.Count == 0)
                {
                    _dataProvider.RequestSubDirs(_helper, parent, e);
                }
            }
            finally
            {
                EndUpdate();
            }
        }

        /// <summary>
        ///     Toggle the check flag for tree nodes, works recursive
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="check"></param>
        private static void CheckNodesRec(TreeNode parent, bool check)
        {
            foreach (TreeNode n in parent.Nodes)
            {
                n.Checked = check;
                //
                CheckNodesRec(n, check);
            }
        }

        /// <summary>
        ///     Add or removes the nodes recursive to or from the folderList_.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="add"></param>
        private void ExchangeFoldersRec(TreeNodePath parent, bool add)
        {
            foreach (TreeNodePath n in parent.Nodes)
            {
                if (n.Path != null)
                {
                    ExchangePath(n.Path, add);
                    MarkNode(parent);
                }

                //
                ExchangeFoldersRec(n, add);
            }
        }

        /// <summary>
        ///     Add or removes path from the folderList_.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="add"></param>
        private void ExchangePath(string path, bool add)
        {
            if (add)
            {
                if (_folderList.Contains(path))
                {
                    return;
                }

                _folderList.Add(path);
                // notfiy add
                OnSelectedDirectoriesChanged(new SelectedDirectoriesChangedEventArgs(path, CheckState.Checked));
            }
            else
            {
                if (!_folderList.Contains(path))
                {
                    return;
                }

                _folderList.Remove(path);
                // notfiy remove
                OnSelectedDirectoriesChanged(new SelectedDirectoriesChangedEventArgs(path, CheckState.Unchecked));
            }
        }

        /// <summary>
        ///     Set the text bold if there is a child node checked.
        /// </summary>
        /// <param name="node"></param>
        internal void MarkNode(TreeNodePath node)
        {
            if (_checkboxBehavior == CheckboxBehaviorMode.None)
            {
                return;
            }

            //
            if (node == null)
            {
                return;
            }

            // no path selected, no node could be marked
            if (_folderList.Count == 0)
            {
                try
                {
                    if (node.NodeFont != null && node.NodeFont.Bold)
                    {
                        node.NodeFont = Font;
                    }
                }
                catch
                {
                    // ignored
                }

                return;
            }

            // there are a few paths, so we have to check each against our node path
            string path = node.Path;
            //
            bool isBold = false;
            foreach (string s in _folderList)
            {
                // if path is equal, return
                if (s.Equals(path))
                {
                    continue;
                }

                // if path is substring, mark node bold, otherwise normal font is used
                if (s.IndexOf(path, StringComparison.Ordinal) == -1)
                {
                    continue;
                }

                isBold = true;
                break;
            }

            //
            node.NodeFont = isBold ? _boldFont : Font;
        }

        /// <summary>
        ///     Set the text bold for each parent node if there is a child node checked.
        /// </summary>
        /// <param name="parent"></param>
        private void MarkNodesRec(TreeNodePath parent)
        {
            while (true)
            {
                if (_checkboxBehavior == CheckboxBehaviorMode.None)
                {
                    return;
                }

                //
                if (parent == null)
                {
                    return;
                }

                //
                MarkNode(parent);
                if (parent.Parent != null)
                {
                    parent = parent.Parent as TreeNodePath;
                    continue;
                }

                break;
            }
        }

        #endregion

        #region events

        /// <summary>
        ///     Raises the CheckboxBehaviorModeChanged event.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        private void OnCheckboxBehaviorModeChanged(EventArgs e) => CheckboxBehaviorModeChanged?.Invoke(this, e);

        /// <summary>
        ///     Raises the DriveTypesChanged event.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        private void OnDriveTypesChanged(EventArgs e) => DriveTypesChanged?.Invoke(this, e);

        /// <summary>
        ///     Raises the DataSourceChanged event.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        private void OnDataSourceChanged(EventArgs e) => DataSourceChanged?.Invoke(this, e);

        /// <summary>
        ///     Used for drives like floppy, cd - rom ect. where it can be that no valid medium is inserted.
        ///     in this case the click on the + will remove the +, after double click there's a new + to give the user
        ///     the chance to browse this device after inserting a valid medium.
        /// </summary>
        protected override void OnDoubleClick(EventArgs e)
        {
            //
            if (!(SelectedNode is TreeNodePath node))
            {
                return;
            }

            //
            if (node.Nodes.Count > 0 || node.Path.Length > 3)
            {
                return;
            }

            //
            node.AddDummyNode();
            //
            base.OnDoubleClick(e);
        }

        /// <summary>
        ///     Fired before check action occurs, manages the folderList_.
        /// </summary>
        protected override void OnBeforeCheck(TreeViewCancelEventArgs e)
        {
            // check suppress flag
            if (IsCheckEventSupressed)
            {
                base.OnBeforeCheck(e);
                return;
            }

            // get current action		
            bool check = !e.Node.Checked;
            // is it allowed to check item ?
            if ((CheckboxBehaviorMode == CheckboxBehaviorMode.RecursiveChecked) & !check & (e.Node.Parent != null) &&
                e.Node.Parent.Checked)
            {
                e.Cancel = true;
                base.OnBeforeCheck(e);
                return;
            }

            // set supress flag
            SupressCheckEvent(true);
            // stop drawing tree content
            BeginUpdate();
            // set cursor
            Cursor.Current = Cursors.WaitCursor;
            //
            try
            {
                // add or remove path
                ExchangePath(((TreeNodePath)e.Node).Path, check);
                // handle recursive behaviour
                if (CheckboxBehaviorMode == CheckboxBehaviorMode.RecursiveChecked)
                {
                    // remove all childs from folderList_
                    ExchangeFoldersRec(e.Node as TreeNodePath, false);
                    // check child nodes to reflect parent check state
                    CheckNodesRec(e.Node, check);
                }

                // update marked nodes fonts
                MarkNodesRec(e.Node.Parent as TreeNodePath);
            }
            catch (Exception ex)
            {
                // ups, exception ?
                Console.WriteLine(ex.Message);
            }
            finally
            {
                // reset supress flag
                SupressCheckEvent(false);
                // let the tree redraw his content
                EndUpdate();
                // reset the cursor
                Cursor.Current = Cursors.Default;
            }

            //
            base.OnBeforeCheck(e);
        }

        /// <summary>
        ///     Fired before node expands, used to fill next level in directory structure.
        /// </summary>
        protected override void OnBeforeExpand(TreeViewCancelEventArgs e)
        {
            TreeNodePath node = e.Node as TreeNodePath;
            //
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                GetSubDirs(node, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, MethodBase.GetCurrentMethod()?.Name);
                e.Cancel = true;
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }

            //
            base.OnBeforeExpand(e);
        }

        /// <summary>
        ///     Raises the SelectedDirectoriesChanged event.<seealso cref="SelectedDirectoriesChangedDelegate" />
        /// </summary>
        private void OnSelectedDirectoriesChanged(SelectedDirectoriesChangedEventArgs e) =>
            SelectedDirectoriesChanged?.Invoke(this, e);

        #endregion
    }

    /// <summary>
    ///     <c>ITreeViewFolderBrowserDataProvider</c> is used by a <see cref="TreeViewFolderBrowser" /> instance and is
    ///     responsible for
    ///     <list>
    ///         <item>retrieve the computer drives and directories</item>
    ///         <item>Imagelist which is used to assign images to the nodes created by this instance.</item>
    ///         <item>ContextMenu</item>
    ///     </list>
    ///     <see cref="TreeViewFolderBrowser" /> calls the interface method's and provide a
    ///     <see cref="TreeViewFolderBrowserHelper" /> instance which let you create node's and give you access to the
    ///     underlying <see cref="TreeViewFolderBrowser" /> instance.
    /// </summary>
    public interface ITreeViewFolderBrowserDataProvider : IDisposable
    {
        /// <summary>
        ///     Gets the ImageList that contains the Image objects used by the tree nodes.
        /// </summary>
        ImageList ImageList { get; }

        /// <summary>
        ///     Fired before the context menu popup.
        /// </summary>
        /// <param name="helper">The helper instance which provides method's and properties related to create and get nodes.</param>
        /// <param name="node">The node on which the context menu was requested.</param>
        void QueryContextMenuItems(TreeViewFolderBrowserHelper helper, TreeNodePath node);

        /// <summary>
        ///     Fill the root level.
        /// </summary>
        /// <param name="helper">The helper instance which provides method's and properties related to create and get nodes.</param>
        void RequestRoot(TreeViewFolderBrowserHelper helper);

        /// <summary>
        ///     Fill the Directory structure for a given path.
        /// </summary>
        /// <param name="helper">The helper instance which provides method's and properties related to create and get nodes.</param>
        /// <param name="parent">The expanding node.</param>
        /// <param name="e"></param>
        void RequestSubDirs(TreeViewFolderBrowserHelper helper, TreeNodePath parent, TreeViewCancelEventArgs e);

        /// <summary>
        ///     Gets the tree node collection which holds the drive node's. The requested collection is than used to search a
        ///     specific node.
        /// </summary>
        TreeNodeCollection RequestDriveCollection(TreeViewFolderBrowserHelper helper);
    }

    /// <summary>
    ///     <c>TreeViewFolderBrowserHelper</c> is like a bridge between <see cref="ITreeViewFolderBrowserDataProvider" /> and
    ///     <see cref="TreeViewFolderBrowser" />
    ///     and provides access to the needed informations to fill the tree view.
    /// </summary>
    public class TreeViewFolderBrowserHelper
    {
        #region fields

        /// <summary>the managed tree view instance</summary>
        private readonly TreeViewFolderBrowser _treeView;

        #endregion

        #region constructors

        /// <summary>
        ///     Initialize a new instance of TreeViewFolderBrowserHelper for the specified TreeViewFolderBrowser instance.
        /// </summary>
        /// <param name="treeView"></param>
        internal TreeViewFolderBrowserHelper(TreeViewFolderBrowser treeView) => _treeView = treeView;

        #endregion

        #region public interface

        /// <summary>
        ///     Gets the underlying <see cref="TreeViewFolderBrowser" /> instance.
        /// </summary>
        public TreeViewFolderBrowser TreeView => _treeView;

        /// <summary>
        ///     Creates a tree node and add it to the <c>TreeNodeCollection</c>.
        /// </summary>
        /// <param name="text">The text displayed in the label of the tree node.</param>
        /// <param name="path">The path the node represents.</param>
        /// <param name="addDummyNode">True to add + sign, otherwise no + sign appears.</param>
        /// <param name="forceChecked">
        ///     True to check node in each case, otherwise false to allow normal check against selected
        ///     paths.
        /// </param>
        /// <param name="isSpecialFolder">
        ///     Specifies if this node is a special folder. Special folders do not request data from the
        ///     attached data provider.
        /// </param>
        /// <returns></returns>
        public virtual TreeNodePath CreateTreeNode(string text, string path, bool addDummyNode, bool forceChecked,
            bool isSpecialFolder)
        {
            TreeNodePath newNode = new TreeNodePath(text, isSpecialFolder)
            {
                // path
                Path = path
            };
            //						
            try
            {
                _treeView.SupressCheckEvent(true);
                //
                newNode.Checked = forceChecked || _treeView.SelectedDirectories.Contains(path);
                _treeView.MarkNode(newNode);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message, _treeView.Name);
            }
            finally
            {
                _treeView.SupressCheckEvent(false);
            }

            //
            if (addDummyNode)
            // add dummy node, otherwise there is no + sign
            {
                newNode.AddDummyNode();
            }

            //
            return newNode;
        }

        #endregion
    }

    /// <summary>
    ///     A simple designer class for the <see cref="TreeViewFolderBrowser" /> control to remove
    ///     unwanted properties at design time.
    /// </summary>
    public class TreeViewFolderBrowserDesigner : ControlDesigner
    {
        /// <summary>
        ///     Allows a designer to change or remove items from the set of properties that it exposes through a TypeDescriptor.
        /// </summary>
        /// <param name="properties">The properties for the class of the component.</param>
        protected override void PreFilterProperties(IDictionary properties)
        {
            properties.Remove("CheckBoxes");
            properties.Remove("ImageList");
            properties.Remove("SelectedImageIndex");
            properties.Remove("ImageIndex");
            properties.Remove("ContextMenu");
        }
    }

    /// <summary>
    ///     Extends the <c>TreeNode</c> type with a path property. This node type is used by
    ///     <see cref="TreeViewFolderBrowser" />
    /// </summary>
    public class TreeNodePath : TreeNode
    {
        #region constructor

        /// <summary>
        ///     Initializes a new instance of the TreeNodePath class.
        /// </summary>
        /// <param name="text">The label Text of the new tree node. </param>
        /// <param name="isSpecialFolder"></param>
        public TreeNodePath(string text, bool isSpecialFolder) : base(text) => _isSpecialFolder = isSpecialFolder;

        #endregion

        #region fields

        /// <summary>Specifiy that this node instance represent a special folder.</summary>
        private readonly bool _isSpecialFolder;

        /// <summary>
        ///     File or directory path information
        /// </summary>
        private string _path;

        #endregion fiels

        #region public interface

        /// <summary>
        ///     Adds a dummy node to the parent node
        /// </summary>
        public virtual void AddDummyNode() => Nodes.Add(new TreeNodePath("@@Dummy@@", false));

        /// <summary>
        ///     Removes the dummy node from the parent node.
        /// </summary>
        public virtual void RemoveDummyNode()
        {
            if ((Nodes.Count == 1) & (Nodes[0].Text == "@@Dummy@@"))
            {
                Nodes[0].Remove();
            }
        }

        /// <summary>
        ///     Gets or sets this node as a special folder node.
        /// </summary>
        /// <remarks>
        ///     SpecialFolder's are folder's which are defined by <see cref="Environment.SpecialFolder" /> enum.
        /// </remarks>
        public bool IsSpecialFolder => _isSpecialFolder;

        /// <summary>
        ///     Gets or sets the file or directory path information
        /// </summary>
        public string Path
        {
            get => _path;
            set => _path = value;
        }

        #endregion
    }

    /// <summary>
    ///     Occurs when a directory checkstate has changed
    /// </summary>
    /// <remarks>
    ///     The SelectedDirectoriesChangedDelegate event passes an SelectedDirectoriesChangedEventArgs object to its event
    ///     handler.
    /// </remarks>
    public delegate void SelectedDirectoriesChangedDelegate(object sender, SelectedDirectoriesChangedEventArgs e);

    /// <summary>
    ///     Provides data for the SelectedDirectoriesChangedDelegate event of a TreeViewFolderBrowser control.
    /// </summary>
    public class SelectedDirectoriesChangedEventArgs
    {
        #region constructors

        /// <summary>Initalize a new instance of SelectedDirectoriesChangedEventArgs</summary>
        public SelectedDirectoriesChangedEventArgs(string path, CheckState checkState)
        {
            _path = path;
            _checkState = checkState;
        }

        #endregion

        /// <summary>Gets the path which was modified</summary>
        public string Path => _path;

        /// <summary>Gets the check state for the path</summary>
        public CheckState CheckState => _checkState;

        #region fields

        /// <summary>File path</summary>
        private readonly string _path;

        /// <summary>Checkstate</summary>
        private readonly CheckState _checkState;

        #endregion
    }

    /// <summary>
    ///     Indicating whether check boxes are displayed next to the tree nodes in the tree view control and how the tree view
    ///     handle related events.
    /// </summary>
    public enum CheckboxBehaviorMode
    {
        /// <summary>
        ///     No check boxes are displayed next to the tree nodes in the tree view control.
        /// </summary>
        None,

        /// <summary>
        ///     Check boxes are displayed next to the tree nodes in the tree view control. The user can check directories.
        /// </summary>
        SingleChecked,

        /// <summary>
        ///     Check boxes are displayed next to the tree nodes in the tree view control. The user can check directories, the
        ///     subdirectories are checked recursive.
        /// </summary>
        RecursiveChecked
    }

    /// <summary>
    ///     Defines the DriveTypes used for Win32_LogicalDisk<seealso cref="TreeViewFolderBrowser" />.This enumeration can be
    ///     treated as a bit field, that is, a set of flags.
    /// </summary>
    [Flags]
    [Editor(typeof(FlagsEditor), typeof(UITypeEditor))]
    public enum DriveTypes
    {
        /// <summary>All drive types</summary>
        All = NoRootDirectory | RemovableDisk | LocalDisk | NetworkDrive | CompactDisc | RamDisk,

        /// <summary>
        ///     NoRootDirectory
        /// </summary>
        NoRootDirectory = 0x0001,

        /// <summary>
        ///     Drive has removable media. This includes all floppy drives and many other varieties of storage devices.
        /// </summary>
        RemovableDisk = 0x0002,

        /// <summary>
        ///     Drive has fixed (nonremovable) media. This includes all hard drives, including hard drives that are removable.
        /// </summary>
        LocalDisk = 0x0004,

        /// <summary>
        ///     Network drives. This includes drives shared anywhere on a network.
        /// </summary>
        NetworkDrive = 0x0008,

        /// <summary>
        ///     Drive is a CD-ROM. No distinction is made between read-only and read/write CD-ROM drives.
        /// </summary>
        CompactDisc = 0x0020,

        /// <summary>
        ///     Drive is a block of Random Access Memory (RAM) on the local computer that behaves like a disk drive.
        /// </summary>
        RamDisk = 0x0040
    }

    /// <summary>
    ///     Defines the DriveTypes used for Win32_LogicalDisk<seealso cref="TreeViewFolderBrowser" />.This enumeration can a
    ///     <b>not</b> be treated as a bit field
    /// </summary>
    public enum Win32LogicalDiskDriveTypes
    {
    }
}

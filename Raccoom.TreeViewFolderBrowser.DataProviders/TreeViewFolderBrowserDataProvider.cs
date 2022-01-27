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
using System.ComponentModel;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Security.Permissions;
using System.Windows.Forms;
using Raccoom.Win32;
using ROOT.CIMV2.Win32;

namespace Raccoom.Windows.Forms
{
    /// <summary>
    ///     <c>TreeViewFolderBrowserDataProvider</c> is the standard data provider for <see cref="TreeViewFolderBrowser" />
    ///     which is based on <see cref="ROOT.CIMV2.Win32.LogicalDisk" />, System.IO and
    ///     <see cref="Raccoom.Win32.SystemImageList" />
    ///     <seealso cref="ITreeViewFolderBrowserDataProvider" />
    /// </summary>
    [ToolboxBitmap(typeof(SqlDataAdapter))]
    public class TreeViewFolderBrowserDataProvider : ITreeViewFolderBrowserDataProvider
    {
        #region IDisposable Members

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose() => _systemImageList?.Dispose();

        #endregion

        public override string ToString() => "Standard Provider";

        #region fields

        /// <summary>Shell32 ImageList</summary>
        private SystemImageList _systemImageList;

        /// <summary>last CheckboxMode used to fill the tree view, saved to know about changes</summary>
        private CheckboxBehaviorMode _checkboxMode;

        #endregion

        #region ITreeViewFolderBrowserDataProvider Members

        [Browsable(false)] public virtual ImageList ImageList => null;

        public virtual void QueryContextMenuItems(TreeViewFolderBrowserHelper helper, TreeNodePath node)
        {
        }

        public virtual TreeNodeCollection RequestDriveCollection(TreeViewFolderBrowserHelper helper)
        {
            switch (helper.TreeView.RootFolder)
            {
                case Environment.SpecialFolder.Desktop:
                    return helper.TreeView.Nodes[0].Nodes[1].Nodes;
                case Environment.SpecialFolder.ApplicationData:
                case Environment.SpecialFolder.CommonApplicationData:
                case Environment.SpecialFolder.LocalApplicationData:
                case Environment.SpecialFolder.Cookies:
                case Environment.SpecialFolder.Favorites:
                case Environment.SpecialFolder.History:
                case Environment.SpecialFolder.InternetCache:
                case Environment.SpecialFolder.Programs:
                case Environment.SpecialFolder.MyComputer:
                case Environment.SpecialFolder.MyMusic:
                case Environment.SpecialFolder.MyPictures:
                case Environment.SpecialFolder.Recent:
                case Environment.SpecialFolder.SendTo:
                case Environment.SpecialFolder.StartMenu:
                case Environment.SpecialFolder.Startup:
                case Environment.SpecialFolder.System:
                case Environment.SpecialFolder.Templates:
                case Environment.SpecialFolder.DesktopDirectory:
                case Environment.SpecialFolder.Personal:
                case Environment.SpecialFolder.ProgramFiles:
                case Environment.SpecialFolder.CommonProgramFiles:
                case Environment.SpecialFolder.AdminTools:
                case Environment.SpecialFolder.CDBurning:
                case Environment.SpecialFolder.CommonAdminTools:
                case Environment.SpecialFolder.CommonDocuments:
                case Environment.SpecialFolder.CommonMusic:
                case Environment.SpecialFolder.CommonOemLinks:
                case Environment.SpecialFolder.CommonPictures:
                case Environment.SpecialFolder.CommonStartMenu:
                case Environment.SpecialFolder.CommonPrograms:
                case Environment.SpecialFolder.CommonStartup:
                case Environment.SpecialFolder.CommonDesktopDirectory:
                case Environment.SpecialFolder.CommonTemplates:
                case Environment.SpecialFolder.CommonVideos:
                case Environment.SpecialFolder.Fonts:
                case Environment.SpecialFolder.MyVideos:
                case Environment.SpecialFolder.NetworkShortcuts:
                case Environment.SpecialFolder.PrinterShortcuts:
                case Environment.SpecialFolder.UserProfile:
                case Environment.SpecialFolder.CommonProgramFilesX86:
                case Environment.SpecialFolder.ProgramFilesX86:
                case Environment.SpecialFolder.Resources:
                case Environment.SpecialFolder.LocalizedResources:
                case Environment.SpecialFolder.SystemX86:
                case Environment.SpecialFolder.Windows:
                default:
                    return helper.TreeView.Nodes;
            }
        }

        public virtual void RequestSubDirs(TreeViewFolderBrowserHelper helper, TreeNodePath parent,
            TreeViewCancelEventArgs e)
        {
            if (parent.Path == null)
            {
                return;
            }

            //
            DirectoryInfo directory = new DirectoryInfo(parent.Path);
            // check persmission
            new FileIOPermission(FileIOPermissionAccess.PathDiscovery, directory.FullName).Demand();
            //					
            foreach (DirectoryInfo dir in directory.GetDirectories())
            {
                if ((dir.Attributes & FileAttributes.System) == FileAttributes.System)
                {
                    continue;
                }

                if ((dir.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                {
                    continue;
                }

                TreeNodePath newNode = CreateTreeNode(helper, dir.Name, dir.FullName, false,
                    helper.TreeView.CheckboxBehaviorMode != CheckboxBehaviorMode.None && parent.Checked, false);
                parent.Nodes.Add(newNode);
                //
                try
                {
                    if (dir.GetDirectories().GetLength(0) > 0)
                    {
                        newNode.AddDummyNode();
                    }
                }
                catch
                {
                    // ignored
                }
            }
        }

        public virtual void RequestRoot(TreeViewFolderBrowserHelper helper)
        {
            AttachSystemImageList(helper);
            //
            bool populateDrives = true;
            //
            TreeNodeCollection rootNodeCollection;
            TreeNodeCollection driveRootNodeCollection;
            // setup up root node collection
            switch (helper.TreeView.RootFolder)
            {
                case Environment.SpecialFolder.Desktop:
                    // create root node <Desktop>
                    TreeNodePath desktopNode = CreateTreeNode(helper, Environment.SpecialFolder.Desktop.ToString(),
                        string.Empty,
                        false, false, true);
                    helper.TreeView.Nodes.Add(desktopNode);
                    rootNodeCollection = helper.TreeView.Nodes[0].Nodes;
                    // create child node <Personal>
                    string personalDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                    rootNodeCollection.Add(CreateTreeNode(helper, Path.GetFileName(personalDirectory),
                        personalDirectory, true, false, false));
                    // create child node <MyComputer>
                    TreeNodePath myComputerNode = CreateTreeNode(helper,
                        Environment.SpecialFolder.MyComputer.ToString(),
                        string.Empty, false, false, true);
                    rootNodeCollection.Add(myComputerNode);
                    driveRootNodeCollection = myComputerNode.Nodes;
                    break;
                case Environment.SpecialFolder.MyComputer:
                    rootNodeCollection = helper.TreeView.Nodes;
                    driveRootNodeCollection = rootNodeCollection;
                    break;
                case Environment.SpecialFolder.ApplicationData:
                case Environment.SpecialFolder.CommonApplicationData:
                case Environment.SpecialFolder.LocalApplicationData:
                case Environment.SpecialFolder.Cookies:
                case Environment.SpecialFolder.Favorites:
                case Environment.SpecialFolder.History:
                case Environment.SpecialFolder.InternetCache:
                case Environment.SpecialFolder.Programs:
                case Environment.SpecialFolder.MyMusic:
                case Environment.SpecialFolder.MyPictures:
                case Environment.SpecialFolder.Recent:
                case Environment.SpecialFolder.SendTo:
                case Environment.SpecialFolder.StartMenu:
                case Environment.SpecialFolder.Startup:
                case Environment.SpecialFolder.System:
                case Environment.SpecialFolder.Templates:
                case Environment.SpecialFolder.DesktopDirectory:
                case Environment.SpecialFolder.Personal:
                case Environment.SpecialFolder.ProgramFiles:
                case Environment.SpecialFolder.CommonProgramFiles:
                case Environment.SpecialFolder.AdminTools:
                case Environment.SpecialFolder.CDBurning:
                case Environment.SpecialFolder.CommonAdminTools:
                case Environment.SpecialFolder.CommonDocuments:
                case Environment.SpecialFolder.CommonMusic:
                case Environment.SpecialFolder.CommonOemLinks:
                case Environment.SpecialFolder.CommonPictures:
                case Environment.SpecialFolder.CommonStartMenu:
                case Environment.SpecialFolder.CommonPrograms:
                case Environment.SpecialFolder.CommonStartup:
                case Environment.SpecialFolder.CommonDesktopDirectory:
                case Environment.SpecialFolder.CommonTemplates:
                case Environment.SpecialFolder.CommonVideos:
                case Environment.SpecialFolder.Fonts:
                case Environment.SpecialFolder.MyVideos:
                case Environment.SpecialFolder.NetworkShortcuts:
                case Environment.SpecialFolder.PrinterShortcuts:
                case Environment.SpecialFolder.UserProfile:
                case Environment.SpecialFolder.CommonProgramFilesX86:
                case Environment.SpecialFolder.ProgramFilesX86:
                case Environment.SpecialFolder.Resources:
                case Environment.SpecialFolder.LocalizedResources:
                case Environment.SpecialFolder.SystemX86:
                case Environment.SpecialFolder.Windows:
                default:
                    rootNodeCollection = helper.TreeView.Nodes;
                    driveRootNodeCollection = rootNodeCollection;
                    // create root node with specified SpecialFolder
                    rootNodeCollection.Add(CreateTreeNode(helper,
                        Path.GetFileName(Environment.GetFolderPath(helper.TreeView.RootFolder)),
                        Environment.GetFolderPath(helper.TreeView.RootFolder), true, false, false));
                    populateDrives = false;
                    break;
            }

            if (!populateDrives)
            {
                return;
            }

            // populate local machine drives
            foreach (LogicalDisk logicalDisk in LogicalDisk.GetInstances(null, GetWmiQueryStatement(helper.TreeView)))
            {
                string path = logicalDisk.Name + "\\";
                string name = logicalDisk.Description;
                //
                name += name != string.Empty ? " (" + path + ")" : path;
                // add node to root collection
                driveRootNodeCollection.Add(CreateTreeNode(helper, name, path, true, false, false));
            }
        }

        #endregion

        #region internal interface

        protected virtual void AttachSystemImageList(TreeViewFolderBrowserHelper helper)
        {
            if (_checkboxMode != helper.TreeView.CheckboxBehaviorMode)
            // checkboxes recreate the control internal
            {
                if (_systemImageList != null)
                {
                    SystemImageListHelper.SetTreeViewImageList(helper.TreeView, _systemImageList, false);
                }
            }

            _checkboxMode = helper.TreeView.CheckboxBehaviorMode;
        }

        /// <summary>
        ///     Creates a new node and assigns a icon
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="text"></param>
        /// <param name="path"></param>
        /// <param name="addDummyNode"></param>
        /// <param name="forceChecked"></param>
        /// <param name="isSpecialFolder"></param>
        /// <returns></returns>
        protected virtual TreeNodePath CreateTreeNode(TreeViewFolderBrowserHelper helper, string text, string path,
            bool addDummyNode, bool forceChecked, bool isSpecialFolder)
        {
            TreeNodePath node = helper.CreateTreeNode(text, path, addDummyNode, forceChecked, isSpecialFolder);
            try
            {
                SetIcon(helper.TreeView, node);
            }
            catch
            {
                node.ImageIndex = -1;
                node.SelectedImageIndex = -1;
            }

            return node;
        }

        /// <summary>
        ///     Extract the icon for the file type (Extension)
        /// </summary>
        protected virtual void SetIcon(TreeViewFolderBrowser treeView, TreeNodePath node)
        {
            // create on demand
            if (_systemImageList == null)
            {
                // Shell32 ImageList
                _systemImageList = new SystemImageList(SystemImageListSize.SmallIcons);
                SystemImageListHelper.SetTreeViewImageList(treeView, _systemImageList, false);
            }

            node.ImageIndex = _systemImageList.IconIndex(node.Path, true);
            node.SelectedImageIndex = node.ImageIndex;
        }

        /// <summary>
        ///     Gets the WMI query string based on the current drive types.
        /// </summary>
        /// <returns></returns>
        protected virtual string GetWmiQueryStatement(TreeViewFolderBrowser treeView)
        {
            if ((treeView.DriveTypes & DriveTypes.All) == DriveTypes.All)
            {
                return string.Empty;
            }

            //
            string where = string.Empty;
            //
            Array array = Enum.GetValues(typeof(DriveTypes));
            //
            foreach (DriveTypes type in array)
            {
                if ((treeView.DriveTypes & type) != type)
                {
                    continue;
                }

                if (where == string.Empty)
                {
                    @where += "drivetype = " + Enum.Format(typeof(Win32LogicalDiskDriveTypes),
                        Enum.Parse(typeof(Win32LogicalDiskDriveTypes), type.ToString(), true), "d");
                }
                else
                {
                    @where += " OR drivetype = " + Enum.Format(typeof(Win32LogicalDiskDriveTypes),
                        Enum.Parse(typeof(Win32LogicalDiskDriveTypes), type.ToString(), true), "d");
                }
            }

            //
            return where;
        }

        #endregion
    }
}

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
using System.Linq;
using System.Windows.Forms;
using Raccoom.Win32;
using ROOT.CIMV2.Win32;
using Shell32;

namespace Raccoom.Windows.Forms
{
    /// <summary>
    ///     <c>TreeViewFolderBrowserDataProvider</c> is the shell32 interop data provider for
    ///     <see cref="TreeViewFolderBrowser" /> which is based on <see cref="ROOT.CIMV2.Win32.LogicalDisk" />, <c>Shell32</c>
    ///     Interop and <see cref="Raccoom.Win32.SystemImageList" />
    ///     <seealso cref="ITreeViewFolderBrowserDataProvider" />
    /// </summary>
    /// <remarks>
    ///     Shell32 does not support the .NET System.Security.Permissions system. There is no code access permission, only
    ///     FileSystem ACL.
    /// </remarks>
    [DefaultProperty("ShowAllShellObjects")]
    [ToolboxBitmap(typeof(SqlDataAdapter))]
    public class TreeViewFolderBrowserDataProviderShell32 : TreeViewFolderBrowserDataProvider
    {
        public override string ToString() => "Shell32 Provider";

        #region fields

        /// <summary>Shell32 Com Object</summary>
        private readonly Shell32Namespaces _shell = new Shell32Namespaces();

        /// <summary>drive tree node (mycomputer) root collection</summary>
        private TreeNodeCollection _rootCollection;

        #endregion

        #region constructors

        #endregion

        #region public interface

        /// <summary>
        ///     Enables or disables the context menu which show's the folder item's shell verbs.
        /// </summary>
        [Browsable(true)]
        [Category("Behaviour")]
        [Description("Specifies if the context menu is enabled.")]
        [DefaultValue(false)]
        public bool EnableContextMenu { get; set; }

        /// <summary>show only filesystem</summary>
        [Browsable(true)]
        [Category("Behaviour")]
        [Description("Display file system and virtual shell folders.")]
        [DefaultValue(false)]
        public bool ShowAllShellObjects { get; set; } = false;

        #endregion

        #region ITreeViewFolderBrowserDataProvider Members

        public override void QueryContextMenuItems(TreeViewFolderBrowserHelper helper, TreeNodePath node)
        {
            if (!EnableContextMenu)
            {
                return;
            }

            //
            if (!(node.Tag is FolderItem fi))
            {
                return;
            }

            //
            foreach (MenuItemShellVerb item in from FolderItemVerb verb in fi.Verbs()
                                               where verb.Name.Length != 0
                                               select new MenuItemShellVerb(verb))
            {
                helper.TreeView.ContextMenu.MenuItems.Add(item);
            }
        }

        public override void RequestRoot(TreeViewFolderBrowserHelper helper)
        {
            AttachSystemImageList(helper);
            //
            // setup up root node collection
            switch (helper.TreeView.RootFolder)
            {
                case Environment.SpecialFolder.Desktop:
                    Folder2 desktopFolder = (Folder2)_shell.GetDesktop();
                    // create root node <Desktop>
                    TreeNodePath desktopNode = CreateTreeNode(helper, desktopFolder.Title, desktopFolder.Self.Path,
                        false, false,
                        true);
                    helper.TreeView.Nodes.Add(desktopNode);
                    desktopNode.Tag = desktopFolder;
                    //
                    _ = (Folder2)_shell.Shell.NameSpace(ShellSpecialFolderConstants.ssfDRIVES);
                    foreach (FolderItem fi in desktopFolder.Items())
                    {
                        if (!fi.IsFolder)
                        {
                            continue;
                        }

                        //
                        TreeNodePath node = CreateTreeNode(helper, fi.Name, fi.Path, true, false, true);
                        node.Tag = fi;
                        desktopNode.Nodes.Add(node);
                        //
                        if (_shell.Shell.NameSpace(ShellSpecialFolderConstants.ssfDRIVES).Title == fi.Name)
                        {
                            _rootCollection = node.Nodes;
                        }
                    }

                    break;
                case Environment.SpecialFolder.MyComputer:
                    FillMyComputer(((Folder2)_shell.Shell.NameSpace(ShellSpecialFolderConstants.ssfDRIVES)).Self,
                        helper.TreeView.Nodes, helper);
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
                    // create root node with specified SpecialFolder
                    Folder2 root = (Folder3)_shell.Shell.NameSpace(helper.TreeView.RootFolder);
                    TreeNodePath rootNode = CreateTreeNode(helper, root.Title, root.Self.Path, true, false, true);
                    rootNode.Tag = root.Self;
                    helper.TreeView.Nodes.Add(rootNode);
                    _rootCollection = rootNode.Nodes;
                    break;
            }
        }

        public override void RequestSubDirs(TreeViewFolderBrowserHelper helper, TreeNodePath parent,
            TreeViewCancelEventArgs e)
        {
            if (!parent.IsSpecialFolder)
            {
                return;
            }

            //
            FolderItem folderItem = (FolderItem)parent.Tag;
            //
            if (_shell.Shell.NameSpace(ShellSpecialFolderConstants.ssfDRIVES).Title == folderItem.Name)
            {
                FillMyComputer(folderItem, parent.Nodes, helper);
            }
            else
            {
                foreach (FolderItem fi in ((Folder)folderItem.GetFolder).Items())
                {
                    if (!ShowAllShellObjects && !fi.IsFileSystem || !fi.IsFolder)
                    {
                        continue;
                    }

                    //						
                    TreeNodePath node = CreateTreeNode(helper, fi.Name, fi.Path, IsFolderWithChilds(fi), false, true);
                    node.Tag = fi;
                    parent.Nodes.Add(node);
                }
            }
        }

        public override TreeNodeCollection RequestDriveCollection(TreeViewFolderBrowserHelper helper) =>
            _rootCollection;

        #endregion

        #region internal interface

        protected virtual void FillMyComputer(FolderItem folderItem, TreeNodeCollection parentCollection,
            TreeViewFolderBrowserHelper helper)
        {
            _rootCollection = parentCollection;
            LogicalDisk.LogicaldiskCollection logicalDisks = null;
            // get wmi logical disk's if we have to 			
            if (helper.TreeView.DriveTypes != DriveTypes.All)
            {
                logicalDisks = LogicalDisk.GetInstances(null, GetWmiQueryStatement(helper.TreeView));
            }

            //
            foreach (FolderItem fi in ((Folder)folderItem.GetFolder).Items().Cast<FolderItem>()
                     .Where(fi => ShowAllShellObjects || fi.IsFileSystem))
            {
                // check drive type 
                if (fi.IsFileSystem && logicalDisks != null)
                {
                    bool skipDrive = logicalDisks.Cast<LogicalDisk>().All(lg => lg.Name + "\\" != fi.Path);
                    if (skipDrive)
                    {
                        continue;
                    }
                }

                // create new node
                TreeNodePath node = CreateTreeNode(helper, fi.Name, fi.Path, IsFolderWithChilds(fi), false, true);
                node.Tag = fi;
                parentCollection.Add(node);
            }
        }

        /// <summary>
        ///     Do we have to add a dummy node (+ sign)
        /// </summary>
        protected virtual bool IsFolderWithChilds(FolderItem fi) =>
            ShowAllShellObjects || fi.IsFileSystem && fi.IsFolder && !fi.IsBrowsable;

        #endregion
    }

    /// <summary>
    ///     Extends the <c>MenuItem</c> class with a Shell32.FolderItemVerb.
    /// </summary>
    public class MenuItemShellVerb : MenuItem
    {
        #region fields

        private readonly FolderItemVerb _verb;

        #endregion

        #region constructors

        public MenuItemShellVerb(FolderItemVerb verb)
        {
            _verb = verb;
            //
            Text = verb.Name;
        }

        #endregion

        #region internal interface

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            //
            try
            {
                _verb.DoIt();
            }
            catch
            {
                // ignored
            }
        }

        #endregion
    }
}

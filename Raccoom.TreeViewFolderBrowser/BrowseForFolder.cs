using System;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace TreeViewExplorer.Source
{	
	/// <summary>
	/// Standard BrowseForFolder Dialog
	/// </summary>
	public class BrowseForFolder : FolderNameEditor 
	{ 
		FolderNameEditor.FolderBrowser bDialog;
    	
		public BrowseForFolder()
		{ 
			bDialog = new FolderNameEditor.FolderBrowser(); 
		}
    	
		public string browseDialog(string sTitle)
		{
			bDialog.Description = sTitle;
			bDialog.StartLocation = FolderNameEditor.FolderBrowserFolder.MyComputer;
			bDialog.Style = FolderNameEditor.FolderBrowserStyles.RestrictToDomain;
			bDialog.ShowDialog();			
			return bDialog.DirectoryPath;
		}
		~BrowseForFolder()
		{
			bDialog.Dispose(); 
		}
	}

}

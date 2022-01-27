/*
 * Copyright (c) 2006 Calcitem Studio
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace Calcitem.FileSystemWatch
{
    public partial class CalcitemBrowser : Form
    {
        public CalcitemBrowser() => InitializeComponent();

        public void Navigate(string address)
        {
            if (address.EndsWith(":"))
            {
                address += "\\";
            }

            if (string.IsNullOrEmpty(address))
            {
                return;
            }

            if (address.Equals("about:blank"))
            {
                return;
            }

            if (address.IndexOf(':') == -1)
            {
                address = @"http://" + address;
            }

            try
            {
                fileBrowser.Navigate(new Uri(address));
            }
            catch (UriFormatException)
            {
            }
        }

        private static string FormatUrl(string url)
        {
            string newUrl = url.Replace("file:///", "");
            if (newUrl.StartsWith("/"))
            {
                newUrl = newUrl.Substring(1);
            }

            if (newUrl.IndexOf("http://", StringComparison.Ordinal) == -1 &&
                newUrl.IndexOf("https://", StringComparison.Ordinal) == -1 &&
                newUrl.IndexOf("ftp://", StringComparison.Ordinal) == -1)
            {
                newUrl = newUrl.Replace("/", "\\");
            }

            return newUrl;
        }

        private static IEnumerable<string> UrlToStringArray(string url)
        {
            string newUrl = FormatUrl(url);
            if (!newUrl.EndsWith("\\") && !newUrl.EndsWith("/"))
            {
                newUrl += "\\";
            }

            char splite = '/';
            if (url.IndexOf("http://", StringComparison.Ordinal) == -1 &&
                url.IndexOf("https://", StringComparison.Ordinal) == -1 &&
                url.IndexOf("ftp://", StringComparison.Ordinal) == -1)
            {
                splite = '\\';
            }

            int count = newUrl.Count(t => t == splite);

            string[] strArr = new string[count];
            int k = 0;


            while (true)
            {
                int i = newUrl.IndexOf(splite);
                if (i == -1)
                {
                    break;
                }

                string substr = newUrl.Substring(0, i + 1);

                strArr.SetValue(substr, k);
                newUrl = newUrl.Remove(0, i + 1);
                k++;
            }

            return strArr;
        }

        private void go_Click(object sender, EventArgs e) => Navigate(dirComboBox.Text);

        private void dirToolBar_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
        {
            string add = "";
            int k = dirToolBar.Buttons.IndexOf(e.Button);
            for (int i = 0; i <= k; ++i)
            {
                add += dirToolBar.Buttons[i].Text;
            }

            dirComboBox.Text = FormatUrl(add);
            Navigate(dirComboBox.Text);
        }

        private void fileBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            string strurl = fileBrowser.Url.ToString();
            if (strurl.IndexOf("http://", StringComparison.Ordinal) == -1 &&
                strurl.IndexOf("https://", StringComparison.Ordinal) == -1 &&
                strurl.IndexOf("ftp://", StringComparison.Ordinal) == -1)
            {
                dirComboBox.Text = fileBrowser.Url.LocalPath;
            }
            else
            {
                dirComboBox.Text = strurl;
            }

            //string[] dirArr = fileBrowser.Url.Segments;
            IEnumerable<string> dirArr = UrlToStringArray(strurl);
            //System.Text.Encoding.Convert(dirArr[2], dirArr[2]);

            dirToolBar.Buttons.Clear();

            foreach (string i in dirArr)
            {
                dirToolBar.Buttons.Add(i);
            }

            dirToolBar.ButtonClick += dirToolBar_ButtonClick;
        }

        private void dirComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Navigate(dirComboBox.Text);
            }
        }


        private void dirComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void goBackButton_Click(object sender, EventArgs e) => fileBrowser.GoBack();

        private void fileBrowser_CanGoBackChanged(object sender, EventArgs e) =>
            goBackButton.Enabled = fileBrowser.CanGoBack;

        private void forwardButton_Click(object sender, EventArgs e) => fileBrowser.GoForward();

        private void fileBrowser_CanGoForwardChanged(object sender, EventArgs e) =>
            forwardButton.Enabled = fileBrowser.CanGoForward;

        private void fileBrowser_NewWindow(object sender, CancelEventArgs e)
        {
            //MessageBox.Show("You must save first.");
            if (fileBrowser.StatusText == "" || fileBrowser.StatusText == "Done")
            {
                return;
            }

            dirComboBox.Text = fileBrowser.StatusText;
            e.Cancel = true;
            Navigate(dirComboBox.Text);
        }

        private void stopButton_Click(object sender, EventArgs e) => fileBrowser.Stop();

        private void refreshButton_Click(object sender, EventArgs e)
        {
            if (!fileBrowser.Url.Equals("about:blank"))
            {
                fileBrowser.Refresh();
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e) => fileBrowser.GoSearch();

        private void fileBrowser_StatusTextChanged(object sender, EventArgs e) =>
            browserStatusLabel.Text = fileBrowser.StatusText;

        private void RefreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!fileBrowser.Url.Equals("about:blank"))
            {
                fileBrowser.Refresh();
            }
        }

        private void StopToolStripMenuItem_Click(object sender, EventArgs e) => fileBrowser.Stop();

        private void exitToolStripMenuItem_Click(object sender, EventArgs e) => Close();
    }
}

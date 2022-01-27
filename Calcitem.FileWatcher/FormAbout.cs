/*
 * Copyright (c) 2006 Calcitem Studio
 */

using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Calcitem.FileSystemWatch
{
    /// <summary>
    ///     A Form that fades into view at creation, and fades out of view at destruction.
    /// </summary>
    public class FormAbout : Form
    {
        // *************************************************************************
        // Constructors.
        // *************************************************************************

        /// <summary>
        ///     Default constructor.
        /// </summary>
        public FormAbout()
        {
            InitializeComponent();
            //
            if (!DesignMode)
            {
            }
        } // End FadingForm()

        private void OkButton_Click(object sender, EventArgs e) => Close();


        #region internal

        // *******************************************************************
        // Timer event handlers.
        // *******************************************************************

        /// <summary>
        ///     Timer tick event handler. Used to drive the fading activity.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void m_fadeInOutTimer_Tick(object sender, EventArgs e)
        {
            // How should we fade?
            if (_mFadeInFlag == false)
            {
                Opacity -= _mFadeInOutTimer.Interval / 1000.0;

                // Should we continue to fade?
                if (Opacity > 0)
                {
                    _mFadeInOutTimer.Enabled = true;
                }
                else
                {
                    _mFadeInOutTimer.Enabled = false;
                    Close();
                } // End else we should close the form.
            } // End if we should fade in.
            else
            {
                Opacity += _mFadeInOutTimer.Interval / 1000.0;
                _mFadeInOutTimer.Enabled = Opacity < 1.0;
                _mFadeInFlag = Opacity < 1.0;
            } // End else we should fade out.
        } // End m_fadeInOutTimer_Tick()

        // *******************************************************************
        // Private methods.
        // *******************************************************************

        #endregion

        #region members

        // *******************************************************************
        // Attributes.
        // *******************************************************************

        /// <summary>
        ///     Flag to control whether the form fades in or out of view.
        /// </summary>
        private bool _mFadeInFlag;

        /// <summary>
        ///     Timer to drive the fading process.
        /// </summary>
        private Timer _mFadeInOutTimer;

        private ContextMenu _contextMenu1;
        private ImageList _imageList1;
        private MenuItem _mnuEmail;
        private MenuItem _mnuWebsite;
        private Label _aboutLabel;
        private Button _okButton;

        /// <summary>
        ///     Required designer variable.
        /// </summary>
        private IContainer components;

        #endregion

        #region events

        // *******************************************************************

        /// <summary>
        ///     Used to initiate the fade in process.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // Should we start fading?
            if (DesignMode)
            {
                return;
            }

            _mFadeInFlag = true;
            Opacity = 0;

            _mFadeInOutTimer.Enabled = true;
        } // End OnLoad()

        // *******************************************************************

        /// <summary>
        ///     Used to control the fade out process.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            // If the user canceled then don't fade anything.
            if (e.Cancel)
            {
                return;
            }

            // Should we fade instead of closing?
            if (!(Opacity > 0))
            {
                return;
            }

            _mFadeInFlag = false;
            _mFadeInOutTimer.Enabled = true;
            e.Cancel = true;
        } // End OnClosing()

        #endregion

        #region Windows Form Designer generated code

        // *************************************************************************
        // Overrides.
        // *************************************************************************

        /// <summary>
        ///     Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                    components.Dispose();
            } // End if

            base.Dispose(disposing);
        } // End Dispose()

        /// <summary>
        ///     Required method for Designer support - do not modify
        ///     the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources =
                new System.ComponentModel.ComponentResourceManager(typeof(FormAbout));
            this._mFadeInOutTimer = new System.Windows.Forms.Timer(this.components);
            this._contextMenu1 = new System.Windows.Forms.ContextMenu();
            this._mnuEmail = new System.Windows.Forms.MenuItem();
            this._mnuWebsite = new System.Windows.Forms.MenuItem();
            this._imageList1 = new System.Windows.Forms.ImageList(this.components);
            this._aboutLabel = new System.Windows.Forms.Label();
            this._okButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // m_fadeInOutTimer
            // 
            this._mFadeInOutTimer.Tick += new System.EventHandler(this.m_fadeInOutTimer_Tick);
            // 
            // contextMenu1
            // 
            this._contextMenu1.MenuItems.AddRange(
                new System.Windows.Forms.MenuItem[] { this._mnuEmail, this._mnuWebsite });
            // 
            // mnuEmail
            // 
            this._mnuEmail.Index = 0;
            this._mnuEmail.Text = "&Email";
            // 
            // mnuWebsite
            // 
            this._mnuWebsite.Index = 1;
            this._mnuWebsite.Text = "&Website";
            // 
            // imageList1
            // 
            this._imageList1.ImageStream =
                ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this._imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this._imageList1.Images.SetKeyName(0, "");
            this._imageList1.Images.SetKeyName(1, "");
            this._imageList1.Images.SetKeyName(2, "");
            // 
            // aboutlabel
            // 
            this._aboutLabel.AutoSize = true;
            this._aboutLabel.Location = new System.Drawing.Point(24, 19);
            this._aboutLabel.Name = "_aboutlabel";
            this._aboutLabel.Size = new System.Drawing.Size(191, 48);
            this._aboutLabel.TabIndex = 0;
            this._aboutLabel.Text = "Calcitem  File System Monitor\r\n\r\nCopyright 2006 Calcitem Studio";
            // 
            // OKbutton
            // 
            this._okButton.Location = new System.Drawing.Point(203, 118);
            this._okButton.Name = "_oKbutton";
            this._okButton.Size = new System.Drawing.Size(72, 26);
            this._okButton.TabIndex = 1;
            this._okButton.Text = "&OK";
            this._okButton.UseVisualStyleBackColor = true;
            this._okButton.Click += new System.EventHandler(this.OkButton_Click);
            // 
            // FormAbout
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 15);
            this.ClientSize = new System.Drawing.Size(314, 155);
            this.Controls.Add(this._okButton);
            this.Controls.Add(this._aboutLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormAbout";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "About";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion
    } // End class FadingForm
} // End namespace demo

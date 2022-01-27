using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Raccoom.Win32
{
    #region Public Enumerations

    /// <summary>
    ///     Available system image list sizes
    /// </summary>
    public enum SystemImageListSize
    {
        /// <summary>
        ///     System Small Icon Size (typically 16x16)
        /// </summary>
        SmallIcons = 0x1
    }

    /// <summary>
    ///     Flags specifying the state of the icon to draw from the Shell
    /// </summary>
    [Flags]
    public enum ShellIconStateConstants
    {
        /// <summary>
        ///     Get icon in normal state
        /// </summary>
        ShellIconStateNormal = 0
    }

    #endregion

    #region SystemImageList class

    /// <summary>
    ///     Summary description for SysImageList.
    /// </summary>
    public class SystemImageList : IDisposable
    {
        #region Properties

        /// <summary>
        ///     Gets the hImageList handle
        /// </summary>
        public IntPtr Handle => _hIml;

        #endregion

        #region Private Enumerations

        [Flags]
        private enum ShGetFileInfoConstants
        {
            ShgfiSysiconindex = 0x4000, // get system icon index 
            ShgfiSmallicon = 0x1, // get small icon // pszPath is a pidl 
            ShgfiUsefileattributes = 0x10 // use passed dwFileAttribute 
        }

        #endregion

        #region Constants

        private const int MaxPath = 260;

        private const int FileAttributeNormal = 0x80;

        #endregion

        #region Fields

        private IntPtr _hIml = IntPtr.Zero;
        private IImageList _iImageList;
        private readonly SystemImageListSize _size;
        private bool _disposed;

        #endregion

        #region Private ImageList structures

        [StructLayout(LayoutKind.Sequential)]
        private struct Rect
        {
            private readonly int left;
            private readonly int top;
            private readonly int right;
            private readonly int bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct Point
        {
            private readonly int x;
            private readonly int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct Imagelistdrawparams
        {
            private readonly int cbSize;
            private readonly IntPtr himl;
            private readonly int i;
            private readonly IntPtr hdcDst;
            private readonly int x;
            private readonly int y;
            private readonly int cx;
            private readonly int cy;
            private readonly int xBitmap; // x offest from the upperleft of bitmap
            private readonly int yBitmap; // y offset from the upperleft of bitmap
            private readonly int rgbBk;
            private readonly int rgbFg;
            private readonly int fStyle;
            private readonly int dwRop;
            private readonly int fState;
            private readonly int Frame;
            private readonly int crEffect;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct Imageinfo
        {
            private readonly IntPtr hbmImage;
            private readonly IntPtr hbmMask;
            private readonly int Unused1;
            private readonly int Unused2;
            private readonly Rect rcImage;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct Shfileinfo
        {
            private readonly IntPtr hIcon;
            public readonly int iIcon;
            private readonly int dwAttributes;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MaxPath)]
            private readonly string szDisplayName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            private readonly string szTypeName;
        }

        #endregion

        #region Constructors, Dispose, Destructor

        /// <summary>
        ///     Creates a SystemImageList with the specified size
        /// </summary>
        /// <param name="size">Size of System ImageList</param>
        public SystemImageList(SystemImageListSize size)
        {
            _size = size;
            Create();
        }

        /// <summary>
        ///     Clears up any resources associated with the SystemImageList
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Clears up any resources associated with the SystemImageList
        ///     when disposing is true.
        /// </summary>
        /// <param name="disposing">Whether the object is being disposed</param>
        public virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_iImageList != null)
                    {
                        Marshal.ReleaseComObject(_iImageList);
                    }

                    _iImageList = null;
                }
            }

            _disposed = true;
        }

        /// <summary>
        ///     Finalise for SysImageList
        /// </summary>
        ~SystemImageList() => Dispose(false);

        #endregion

        #region Implementation

        [DllImport("shell32")]
        private static extern IntPtr SHGetFileInfo(
            string pszPath,
            int dwFileAttributes,
            ref Shfileinfo psfi,
            uint cbFileInfo,
            uint uFlags);

        /// <summary>
        ///     SHGetImageList is not exported correctly in XP.  See KB316931
        ///     http://support.microsoft.com/default.aspx?scid=kb;EN-US;Q316931
        ///     Apparently (and hopefully) ordinal 727 isn't going to change.
        /// </summary>
        [DllImport("shell32.dll", EntryPoint = "#727")]
        private static extern int SHGetImageList(
            int iImageList,
            ref Guid riid,
            ref IImageList ppv
        );

        [DllImport("shell32.dll", EntryPoint = "#727")]
        private static extern int SHGetImageListHandle(
            int iImageList,
            ref Guid riid,
            ref IntPtr handle
        );

        /// <summary>
        ///     Determines if the system is running Windows XP
        ///     or above
        /// </summary>
        /// <returns>True if system is running XP or above, False otherwise</returns>
        private static bool IsXpOrAbove()
        {
            bool ret = false;
            if (Environment.OSVersion.Version.Major > 5)
            {
                ret = true;
            }
            else if (Environment.OSVersion.Version.Major == 5 &&
                     Environment.OSVersion.Version.Minor >= 1)
            {
                ret = true;
            }

            return ret;
            //return false;
        }

        /// <summary>
        ///     Creates the SystemImageList
        /// </summary>
        private void Create()
        {
            // forget last image list if any:
            _hIml = IntPtr.Zero;

            if (IsXpOrAbove())
            {
                // Get the System IImageList object from the Shell:
                Guid iidImageList = new Guid("46EB5926-582E-4017-9FDF-E8998DAA0950");
                SHGetImageList(
                    (int)_size,
                    ref iidImageList,
                    ref _iImageList
                );
                // the image list handle is the IUnknown pointer, but 
                // using Marshal.GetIUnknownForObject doesn't return
                // the right value.  It really doesn't hurt to make
                // a second call to get the handle:
                SHGetImageListHandle((int)_size, ref iidImageList, ref _hIml);
            }
            else
            {
                // Prepare flags:
                ShGetFileInfoConstants dwFlags = ShGetFileInfoConstants.ShgfiUsefileattributes |
                                                 ShGetFileInfoConstants.ShgfiSysiconindex;
                if (_size == SystemImageListSize.SmallIcons)
                {
                    dwFlags |= ShGetFileInfoConstants.ShgfiSmallicon;
                }

                // Get image list
                Shfileinfo shfi = new Shfileinfo();
                uint shfiSize = (uint)Marshal.SizeOf(shfi.GetType());

                // Call SHGetFileInfo to get the image list handle
                // using an arbitrary file:
                _hIml = SHGetFileInfo(
                    ".txt",
                    FileAttributeNormal,
                    ref shfi,
                    shfiSize,
                    (uint)dwFlags);
                Debug.Assert(_hIml != IntPtr.Zero, "Failed to create Image List");
            }
        }

        #region Private ImageList COM Interop (XP)

        [ComImportAttribute]
        [GuidAttribute("46EB5926-582E-4017-9FDF-E8998DAA0950")]
        [InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
        //helpstring("Image List"),
        private interface IImageList
        {
            [PreserveSig]
            int Add(
                IntPtr hbmImage,
                IntPtr hbmMask,
                ref int pi);

            [PreserveSig]
            int ReplaceIcon(
                int i,
                IntPtr hicon,
                ref int pi);

            [PreserveSig]
            int SetOverlayImage(
                int iImage,
                int iOverlay);

            [PreserveSig]
            int Replace(
                int i,
                IntPtr hbmImage,
                IntPtr hbmMask);

            [PreserveSig]
            int AddMasked(
                IntPtr hbmImage,
                int crMask,
                ref int pi);

            [PreserveSig]
            int Draw(
                ref Imagelistdrawparams pimldp);

            [PreserveSig]
            int Remove(
                int i);

            [PreserveSig]
            int GetIcon(
                int i,
                int flags,
                ref IntPtr picon);

            [PreserveSig]
            int GetImageInfo(
                int i,
                ref Imageinfo pImageInfo);

            [PreserveSig]
            int Copy(
                int iDst,
                IImageList punkSrc,
                int iSrc,
                int uFlags);

            [PreserveSig]
            int Merge(
                int i1,
                IImageList punk2,
                int i2,
                int dx,
                int dy,
                ref Guid riid,
                ref IntPtr ppv);

            [PreserveSig]
            int Clone(
                ref Guid riid,
                ref IntPtr ppv);

            [PreserveSig]
            int GetImageRect(
                int i,
                ref Rect prc);

            [PreserveSig]
            int GetIconSize(
                ref int cx,
                ref int cy);

            [PreserveSig]
            int SetIconSize(
                int cx,
                int cy);

            [PreserveSig]
            int GetImageCount(
                ref int pi);

            [PreserveSig]
            int SetImageCount(
                int uNewCount);

            [PreserveSig]
            int SetBkColor(
                int clrBk,
                ref int pclr);

            [PreserveSig]
            int GetBkColor(
                ref int pclr);

            [PreserveSig]
            int BeginDrag(
                int iTrack,
                int dxHotspot,
                int dyHotspot);

            [PreserveSig]
            int EndDrag();

            [PreserveSig]
            int DragEnter(
                IntPtr hwndLock,
                int x,
                int y);

            [PreserveSig]
            int DragLeave(
                IntPtr hwndLock);

            [PreserveSig]
            int DragMove(
                int x,
                int y);

            [PreserveSig]
            int SetDragCursorImage(
                ref IImageList punk,
                int iDrag,
                int dxHotspot,
                int dyHotspot);

            [PreserveSig]
            int DragShowNolock(
                int fShow);

            [PreserveSig]
            int GetDragImage(
                ref Point ppt,
                ref Point pptHotspot,
                ref Guid riid,
                ref IntPtr ppv);

            [PreserveSig]
            int GetItemFlags(
                int i,
                ref int dwFlags);

            [PreserveSig]
            int GetOverlayImage(
                int iOverlay,
                ref int piIndex);
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        ///     Returns the index of the icon for the specified file
        /// </summary>
        /// <param name="fileName">Filename to get icon for</param>
        /// <param name="forceLoadFromDisk">
        ///     If True, then hit the disk to get the icon,
        ///     otherwise only hit the disk if no cached icon is available.
        /// </param>
        /// <returns>Index of the icon</returns>
        public int IconIndex(
            string fileName,
            bool forceLoadFromDisk) =>
            IconIndex(
                fileName,
                forceLoadFromDisk,
                ShellIconStateConstants.ShellIconStateNormal);

        /// <summary>
        ///     Returns the index of the icon for the specified file
        /// </summary>
        /// <param name="fileName">Filename to get icon for</param>
        /// <param name="forceLoadFromDisk">
        ///     If True, then hit the disk to get the icon,
        ///     otherwise only hit the disk if no cached icon is available.
        /// </param>
        /// <param name="iconState">
        ///     Flags specifying the state of the icon
        ///     returned.
        /// </param>
        /// <returns>Index of the icon</returns>
        public int IconIndex(
            string fileName,
            bool forceLoadFromDisk,
            ShellIconStateConstants iconState
        )
        {
            ShGetFileInfoConstants dwFlags = ShGetFileInfoConstants.ShgfiSysiconindex;
            int dwAttr;
            if (_size == SystemImageListSize.SmallIcons)
            {
                dwFlags |= ShGetFileInfoConstants.ShgfiSmallicon;
            }

            // We can choose whether to access the disk or not. If you don't
            // hit the disk, you may get the wrong icon if the icon is
            // not cached. Also only works for files.
            if (!forceLoadFromDisk)
            {
                dwFlags |= ShGetFileInfoConstants.ShgfiUsefileattributes;
                dwAttr = FileAttributeNormal;
            }
            else
            {
                dwAttr = 0;
            }

            // sFileSpec can be any file. You can specify a
            // file that does not exist and still get the
            // icon, for example sFileSpec = "C:\PANTS.DOC"
            Shfileinfo shfi = new Shfileinfo();
            uint shfiSize = (uint)Marshal.SizeOf(shfi.GetType());
            IntPtr retVal = SHGetFileInfo(
                fileName, dwAttr, ref shfi, shfiSize,
                (uint)dwFlags | (uint)iconState);

            if (!retVal.Equals(IntPtr.Zero))
            {
                return shfi.iIcon;
            }

            Debug.Assert(!retVal.Equals(IntPtr.Zero), "Failed to get icon index");
            return 0;
        }

        #endregion
    }

    #endregion

    #region SystemImageListHelper class

    /// <summary>
    ///     Helper Methods for Connecting SystemImageList to Common Controls
    /// </summary>
    public class SystemImageListHelper
    {
        #region Implementation

        [DllImport("user32", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(
            IntPtr hWnd,
            int wMsg,
            IntPtr wParam,
            IntPtr lParam);

        #endregion

        #region Methods

        /// <summary>
        ///     Associates a SysImageList with a TreeView control
        /// </summary>
        /// <param name="treeView">TreeView control to associated ImageList with</param>
        /// <param name="sysImageList">System Image List to associate</param>
        /// <param name="forStateImages">Whether to add ImageList as StateImageList</param>
        public static void SetTreeViewImageList(
            TreeView treeView,
            SystemImageList sysImageList,
            bool forStateImages
        )
        {
            IntPtr wParam = (IntPtr)TvsilNormal;
            if (forStateImages)
            {
                wParam = (IntPtr)TvsilState;
            }

            SendMessage(
                treeView.Handle,
                TvmSetimagelist,
                wParam,
                sysImageList.Handle);
        }

        #endregion

        #region Constants

        private const int TvFirst = 0x1100;
        private const int TvmSetimagelist = TvFirst + 9;

        private const int TvsilNormal = 0;
        private const int TvsilState = 2;

        #endregion
    }

    #endregion
}

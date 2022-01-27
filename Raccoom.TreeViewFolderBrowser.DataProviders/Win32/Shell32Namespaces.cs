using Shell32;

namespace Raccoom.Win32
{
    /// <summary>
    ///     Summary description for Shell32Namespaces.
    /// </summary>
    public class Shell32Namespaces
    {
        #region fields

        private Shell _shell;

        #endregion

        #region internal interface

        internal Shell Shell =>
            // create on demand
            _shell ?? (_shell = new ShellClass());

        #endregion

        #region public interface

        public Folder GetDesktop() => Shell.NameSpace(ShellSpecialFolderConstants.ssfDESKTOP);

        #endregion
    }
}

using IWshRuntimeLibrary;
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace ShortcutMaker
{
    public class ShortcutMaker
    {
        public static void CreateShortcutByClsid(string shortcutPath, string targetPath, string iconFilePath)
        {
            Type type = Type.GetTypeFromCLSID(new Guid("72C24DD5-D70A-438B-8A42-98424B88AFB8"));

            // NOT WORK! shell maybe null
            // dynamic shell = Activator.CreateInstance(type);
            // var lnk = shell.CreateShortcut(destination);
            // lnk.TargetPath = filePath;
            // lnk.IconLocation = iconFilePath;
            // lnk.Save();

            object shell = null;
            object lnk = null;
            try
            {
                shell = Activator.CreateInstance(type);
                lnk = type.InvokeMember("CreateShortcut", BindingFlags.InvokeMethod, null, shell, new object[] { shortcutPath });
                type.InvokeMember("TargetPath", BindingFlags.SetProperty, null, lnk, new object[] { targetPath });
                type.InvokeMember("IconLocation", BindingFlags.SetProperty, null, lnk, new object[] { iconFilePath });
                type.InvokeMember("Save", BindingFlags.InvokeMethod, null, lnk, null);
            }
            finally
            {
                Marshal.FinalReleaseComObject(lnk);
                Marshal.FinalReleaseComObject(shell);
            }
        }

        public static void CreateShortcut(string filePath, string destination, string iconFilePath)
        {
            Type type = Type.GetTypeFromCLSID(new Guid("72C24DD5-D70A-438B-8A42-98424B88AFB8"));
            dynamic shell = Activator.CreateInstance(type);
            try
            {
                var lnk = shell.CreateShortcut(destination);
                try
                {
                    lnk.TargetPath = filePath;
                    lnk.IconLocation = iconFilePath;
                    lnk.Save();
                }
                finally
                {
                    Marshal.FinalReleaseComObject(lnk);
                }
            }
            finally
            {
                Marshal.FinalReleaseComObject(shell);
            }
        }

        public static void CreateShortcutByWsh(string shortcutPath, string targetPath, string iconPath, string description = null)
        {
            WshShell shell = new WshShell();
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);

            shortcut.Description = description;
            shortcut.IconLocation = iconPath;
            shortcut.TargetPath = targetPath;
            //shortcut.Hotkey = hotKey;
            shortcut.Save();
        }

        public static void CreateShortcutByCom(string shortcutPath, string targetPath, string iconPath, string description = null)
        {
            IShellLink link = new ShellLink() as IShellLink;
            if (link == null)
            {
                return;
            }

            link.SetDescription(description);
            link.SetPath(targetPath);
            link.SetIconLocation(iconPath, 5);
            // Convert from Keys string value to IShellLink 16-bit format
            // IShellLink: 0xMMVK
            //   MM = Modifier (Alt, Control, Shift)
            //   VK = Virtual key code
            //

            IPersistFile file = (IPersistFile)link;
            file.Save(shortcutPath, false);
            Marshal.ReleaseComObject(file);
            Marshal.ReleaseComObject(link);
        }

        [ComImport]
        [Guid("00021401-0000-0000-C000-000000000046")]
        internal class ShellLink
        {
        }

        /// <summary>The IShellLink interface allows Shell links to be created, modified, and resolved</summary>
        [ComImport]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("000214F9-0000-0000-C000-000000000046")]
        internal interface IShellLink
        {
            /// <summary>Retrieves the path and file name of a Shell link object</summary>
            void GetPath([Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile, int cchMaxPath, out IntPtr pfd, int fFlags);

            /// <summary>Retrieves the list of item identifiers for a Shell link object</summary>
            void GetIDList(out IntPtr ppidl);

            /// <summary>Sets the pointer to an item identifier list (PIDL) for a Shell link object.</summary>
            void SetIDList(IntPtr pidl);

            /// <summary>Retrieves the description string for a Shell link object</summary>
            void GetDescription([Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszName, int cchMaxName);

            /// <summary>Sets the description for a Shell link object. The description can be any application-defined string</summary>
            void SetDescription([MarshalAs(UnmanagedType.LPWStr)] string pszName);

            /// <summary>Retrieves the name of the working directory for a Shell link object</summary>
            void GetWorkingDirectory([Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszDir, int cchMaxPath);

            /// <summary>Sets the name of the working directory for a Shell link object</summary>
            void SetWorkingDirectory([MarshalAs(UnmanagedType.LPWStr)] string pszDir);

            /// <summary>Retrieves the command-line arguments associated with a Shell link object</summary>
            void GetArguments([Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszArgs, int cchMaxPath);

            /// <summary>Sets the command-line arguments for a Shell link object</summary>
            void SetArguments([MarshalAs(UnmanagedType.LPWStr)] string pszArgs);

            /// <summary>Retrieves the hot key for a Shell link object</summary>
            void GetHotkey(out short pwHotkey);

            /// <summary>Sets a hot key for a Shell link object</summary>
            void SetHotkey(short wHotkey);

            /// <summary>Retrieves the show command for a Shell link object</summary>
            void GetShowCmd(out int piShowCmd);

            /// <summary>Sets the show command for a Shell link object. The show command sets the initial show state of the window.</summary>
            void SetShowCmd(int iShowCmd);

            /// <summary>Retrieves the location (path and index) of the icon for a Shell link object</summary>
            void GetIconLocation([Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszIconPath, int cchIconPath, out int piIcon);

            /// <summary>Sets the location (path and index) of the icon for a Shell link object</summary>
            void SetIconLocation([MarshalAs(UnmanagedType.LPWStr)] string pszIconPath, int iIcon);

            /// <summary>Sets the relative path to the Shell link object</summary>
            void SetRelativePath([MarshalAs(UnmanagedType.LPWStr)] string pszPathRel, int dwReserved);

            /// <summary>Attempts to find the target of a Shell link, even if it has been moved or renamed</summary>
            void Resolve(IntPtr hwnd, int fFlags);

            /// <summary>Sets the path and file name of a Shell link object</summary>
            void SetPath([MarshalAs(UnmanagedType.LPWStr)] string pszFile);
        }
    }
}
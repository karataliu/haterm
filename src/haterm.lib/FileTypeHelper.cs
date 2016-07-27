using System;
using System.Runtime.InteropServices;

namespace haterm
{
    public static class FileTypeHelper
    {
        [StructLayout(LayoutKind.Sequential)]
        internal struct SHFILEINFO
        {
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        };

        internal static class FILE_ATTRIBUTE
        {
            public const uint FILE_ATTRIBUTE_NORMAL = 0x80;
        }

        internal static class SHGFI
        {
            public const uint SHGFI_TYPENAME = 0x000000400;
            public const uint SHGFI_USEFILEATTRIBUTES = 0x000000010;
        }

        [DllImport("shell32.dll")]
        internal static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);

        public static string FileTypeInfo(string fileName)
        {
            FileTypeHelper.SHFILEINFO info = new FileTypeHelper.SHFILEINFO();
            uint dwFileAttributes = FileTypeHelper.FILE_ATTRIBUTE.FILE_ATTRIBUTE_NORMAL;
            uint uFlags = (uint)(FileTypeHelper.SHGFI.SHGFI_TYPENAME | FileTypeHelper.SHGFI.SHGFI_USEFILEATTRIBUTES);

            FileTypeHelper.SHGetFileInfo(fileName, dwFileAttributes, ref info, (uint)Marshal.SizeOf(info), uFlags);

            return info.szTypeName;
        }
    }
}

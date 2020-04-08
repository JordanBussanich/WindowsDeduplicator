using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WindowsDeduplicator
{
    public static class Symlink
    {
        [DllImport("kernel32.dll")]
        static extern bool CreateSymbolicLink(string lpSymlinkFileName, string lpTargetFileName, SymbolicLink dwFlags);

        private enum SymbolicLink
        {
            File = 0,
            Directory = 1
        }

        public static void Create(string targetPath, string linkPath)
        {
            CreateSymbolicLink(linkPath, targetPath, SymbolicLink.File);
        }
    }
}

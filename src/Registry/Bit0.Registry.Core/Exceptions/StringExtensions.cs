using System;
using System.IO;

namespace Bit0.Plugins.Core.Exceptions
{
    public static class StringExtensions
    {        public static String NormalizePath(this String path)
        {
            return path.NormalizePath(Path.DirectorySeparatorChar);
        }

        public static String NormalizePath(this String path, Char separatorChar)
        {
            return path.Replace('\\', '/').Replace('/', separatorChar);
        }
    }
}

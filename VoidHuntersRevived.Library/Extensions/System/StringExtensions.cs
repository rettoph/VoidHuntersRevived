using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace VoidHuntersRevived.Library.Extensions.System
{
    public static class StringExtensions
    {
        public static String ToFileName(this String input, Char replace = '_')
        {
            char[] invalids = Path.GetInvalidFileNameChars();
            return new string(input.Select(c => invalids.Contains(c) ? replace : c).ToArray());
        }
    }
}

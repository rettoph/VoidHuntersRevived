﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace VoidHuntersRevived.Windows.Launcher
{
    public static class LauncherConstants
    {
        public static String AppDirectory => Path.Combine(Directory.GetCurrentDirectory(), "GameData");
        public static String Executable => "VoidHuntersRevived.Client.Desktop";
        public static String Release => Path.Combine(AppDirectory, "release.json");
    }
}

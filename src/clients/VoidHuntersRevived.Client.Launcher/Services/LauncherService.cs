using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace VoidHuntersRevived.Client.Launcher.Services
{
    public static class LauncherService
    {
        private static String _path;
        private static String _excecutable;

        static LauncherService()
        {
            _excecutable = "VoidHuntersRevived.Utilities.Launcher";
            _path = Registry.GetValue("HKEY_CURRENT_USER\\Software\\rettoph\\VoidHuntersRevived", "InstallDir", "undefined").ToString();
        }

        public static Process Run(String args, Boolean useShellExecute = false, Boolean redirectStandardOutput = true, Boolean createNoWindow = true)
        {
            return Process.Start(new ProcessStartInfo()
            {
                FileName = System.IO.Path.Combine(_path, _excecutable),
                WorkingDirectory = _path,
                Arguments = args,
                UseShellExecute = useShellExecute,
                RedirectStandardOutput = redirectStandardOutput,
                CreateNoWindow = createNoWindow
            });
        }

        public static Boolean CheckUpdate(String type)
        {
            var proc = LauncherService.Run($"{type} --check");

            return Boolean.Parse(proc.StandardOutput.ReadLine());
        }

        public static Process Update(String type)
        {
            return LauncherService.Run($"{type} --update");
        }

        public static Process Launch(String type)
        {
            return LauncherService.Run($"{type} --launch", false, false, false);
        }
    }
}

using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using VoidHuntersRevived.Client.Launcher.Models;

namespace VoidHuntersRevived.Client.Launcher.Services
{
    public static class LauncherService
    {
        private static String _path;
        private static String _excecutable;

        static LauncherService()
        {
            _excecutable = "VoidHuntersRevived.Utilities.Launcher";
            _path = Registry.GetValue("HKEY_CURRENT_USER\\Software\\rettoph\\VoidHuntersRevived", "InstallDir", "undefined")?.ToString()
                ?? "C:\\Users\\Anthony\\source\\repos\\VoidHuntersRevived\\src\\utilities\\VoidHuntersRevived.Utilities.Launcher\\bin\\Debug\\net5.0";
        }

        public static Process Run(String args, Boolean redirect = true)
        {
            return Process.Start(new ProcessStartInfo()
            {
                FileName = System.IO.Path.Combine(_path, _excecutable),
                WorkingDirectory = _path,
                Arguments = args,
                UseShellExecute = false,
                RedirectStandardOutput = redirect,
                RedirectStandardError = redirect,
            });
        }

        public static Release Info(String type, Boolean remote = true)
        {
            var proc = LauncherService.Run($"{type} --action info --remote {remote}");

            proc.WaitForExit();

            var output = proc.StandardOutput.ReadToEnd();

            return JsonConvert.DeserializeObject<Release>(output);
        }

        public static Process Update(String type)
        {
            return LauncherService.Run($"{type} --action update");
        }

        public static Process Launch(String type)
        {
            return LauncherService.Run($"{type} --action launch", false);
        }
    }
}

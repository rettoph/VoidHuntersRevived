using RestSharp;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Utilities.Launcher.Models;

namespace VoidHuntersRevived.Utilities.Launcher.Services
{
    class ReleaseService
    {
        private static RestClient _releaseServer;
        private static Release _remoteLatest;

        static ReleaseService()
        {
            _releaseServer = new RestClient(Settings.Default.ReleaseServer);
            _remoteLatest = null;
        }

        public static Release GetRemote(IConsole console, String type, String version = "latest", String rid = null)
        {
            rid ??= RuntimeIdentifierService.Get();

            return _releaseServer.Execute<Release>(new RestRequest($"{Settings.Default.GetReleaseEndpoint}?type={type}&rid={rid}&version={version}", Method.GET))?.Data;
        }

        public static String GetLatest(String type)
        {
            if (_remoteLatest == default)
                _remoteLatest = _releaseServer.Execute<Release>(new RestRequest(Settings.Default.GetLatestEndpoint, Method.GET))?.Data;

            return _remoteLatest?.Version ?? ReleaseService.LocalVersions(type).Last();
        }

        public static void LaunchLocal(IConsole console, String type, String version = "latest")
        {   
            try
            {
                if (version == "latest")
                    version = Directory.GetDirectories(Path.Combine(Settings.Default.InstallDirectory, type)).OrderByDescending(v => v).First();

                String excecutable = Settings.Default.Executables[type];
                String path = Path.Combine(Settings.Default.InstallDirectory, type, version);

                console.Out.Write($"Attempting to launch '{Path.Combine(path, excecutable)}'...\n");
                Process.Start(new ProcessStartInfo()
                {
                    FileName = Path.Combine(path, excecutable),
                    WorkingDirectory = path,
                    RedirectStandardOutput = false,
                    CreateNoWindow = false
                });

                Process.GetCurrentProcess().Kill();
            }
            catch(Exception e)
            {
                console.Error.Write($"Error attempting to launch {type} - {version}. If the issue persists please try a force update.\n{e.Message}");
            }
        }

        /// <summary>
        /// Determin whether or not the system has the requested version installed.
        /// </summary>
        /// <returns></returns>
        public static Boolean HasLocal(String type, String version)
        {
            if (Directory.Exists(Path.Combine(Settings.Default.InstallDirectory, type)))
            {
                var directories = Directory.GetDirectories(Path.Combine(Settings.Default.InstallDirectory, type));


                return directories.Contains(Path.Combine(Settings.Default.InstallDirectory, type, version));
            }

            return false;
        }

        /// <summary>
        /// Return a list of all downloaded versions of a specific type.
        /// This is 100% based on file names.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static String[] LocalVersions(String type)
        {
            return Directory.GetDirectories(Path.Combine(Settings.Default.InstallDirectory, type));
        }
    }
}

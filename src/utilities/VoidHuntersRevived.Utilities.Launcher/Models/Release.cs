using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.IO;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Utilities.Launcher.Models
{
    public class Release
    {
        private Boolean _downloading;
        private Single _lastProgress;
        private String _downloadPath;

        public Int32 Id { get; set; }
        public String Version { get; set; }
        public String RID { get; set; }
        public String Type { get; set; }
        public String DownloadUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public String DownloadPath
        {
            get => _downloadPath ?? Path.Combine(Settings.Default.InstallDirectory, this.Version, this.RID, this.Type);
            set => _downloadPath = value;
        }
        public String Executable => Path.Combine(this.DownloadPath, Settings.Default.Executables[this.Type]);

        [JsonIgnore]
        public Boolean Downloaded => Directory.Exists(this.DownloadPath);

        public void Launch(IConsole console, String path = null)
        {
            if (!this.Downloaded)
                this.Download(console, path);

            try
            {
                console.Out.WriteLine($"[Update] Attempting to launch '{this.Executable}'...\n");
                Process.Start(new ProcessStartInfo()
                {
                    FileName = this.Executable,
                    WorkingDirectory = this.DownloadPath,
                    RedirectStandardOutput = false,
                    RedirectStandardError = false,
                    CreateNoWindow = false,
                    WindowStyle = ProcessWindowStyle.Normal,
                });

                // Process.GetCurrentProcess().Kill();
                Environment.Exit(0);
            }
            catch (Exception e)
            {
                console.Error.WriteLine($"[Error] Error attempting to launch {this.Version} - {this.RID} - {this.Type} at '{this.DownloadPath}'. If the issue persists please try a force update.\n{e.Message}");
            }
        }
        public void Download(IConsole console, String path = null)
        {
            Console.WriteLine(path);
            Console.WriteLine(this.DownloadPath);
            Console.WriteLine(path ?? this.DownloadPath);
            this.DownloadPath = path ?? this.DownloadPath;
            var tempDirectory = Path.Combine(Settings.Default.InstallDirectory, "temp");

            if (Directory.Exists(tempDirectory))
                Directory.Delete(tempDirectory, true);

            console.Out.WriteLine($"[Update] Downloading {this.Type} - v{this.Version} to '{this.DownloadPath}'");

            using (WebClient wc = new WebClient())
            {
                var file = Path.Combine(tempDirectory, Path.GetFileName(this.DownloadUrl));
                Directory.CreateDirectory(tempDirectory);

                wc.DownloadProgressChanged += (s, a) =>
                {
                    if (_lastProgress == a.ProgressPercentage)
                        return;

                    _lastProgress = a.ProgressPercentage;

                    console.Out.WriteLine($"[Progress] {(Single)a.ProgressPercentage / 100f}");

                    if (a.ProgressPercentage == 100)
                    {
                        if (Path.GetExtension(file) == ".zip")
                        { // Unzip...
                            console.Out.WriteLine($"[Update] Extracting {Path.GetFileName(file)}...");
                            ZipFile.ExtractToDirectory(file, tempDirectory);
                            File.Delete(file);
                        }

                        try
                        {
                            console.Out.WriteLine($"[Update] Copying to '{this.DownloadPath}'...");
                            Directory.CreateDirectory(this.DownloadPath);
                            this.CopyFilesRecursive(console, new DirectoryInfo(tempDirectory), new DirectoryInfo(this.DownloadPath));
                            Directory.Delete(tempDirectory, true);
                        }
                        catch (Exception e)
                        {
                            console.Error.WriteLine($"{e.Message}");
                            Directory.Delete(this.DownloadPath, true);
                            Directory.Delete(tempDirectory, true);
                        }
                        finally
                        {
                            _downloading = false;

                            Settings.Default.Releases.Add(this);
                            Settings.Default.SaveChanges();

                            console.Out.WriteLine("[Update] Done.");
                        }
                    }
                };

                _downloading = true;
                wc.DownloadFileAsync(
                    new System.Uri(this.DownloadUrl),
                    file
                );

                while (_downloading)
                    Thread.Sleep(100);
            }
        }

        public void Delete(IConsole console)
        {
            try
            {
                if (this.Downloaded)
                {
                    console.Out.WriteLine($"[Update] Deleting '{this.DownloadPath}'...");

                    Directory.Delete(this.DownloadPath, true);
                }
            }
            catch(Exception e)
            {
                console.Error.WriteLine($"[Error] Error Deleting '{this.DownloadPath}':\n{e.Message}");
            }
        }

        private void GetFileInfoRecursive(DirectoryInfo source, DirectoryInfo target, List<(FileInfo file, DirectoryInfo target)> files)
        {
            foreach (DirectoryInfo dir in source.GetDirectories())
                this.GetFileInfoRecursive(dir, target.CreateSubdirectory(dir.Name), files);
            foreach (FileInfo file in source.GetFiles())
                files.Add((file, target));
        }

        private void CopyFilesRecursive(IConsole console, DirectoryInfo source, DirectoryInfo target)
        {
            var files = new List<(FileInfo file, DirectoryInfo target)>();
            this.GetFileInfoRecursive(source, target, files);

            for(var i=0; i<files.Count; i++)
            {
                var info = files[i];

                console.Out.WriteLine($"[Progress] {(Single)i / (Single)files.Count}");
                console.Out.WriteLine($"[Update] Copying '{info.file.Name}' to '{info.target.FullName}'...");

                info.file.CopyTo(Path.Combine(info.target.FullName, info.file.Name), true);
            }
        }

        public Boolean Equals(Release r)
            => r.Version == this.Version && r.RID == this.RID && r.Type == this.Type;
    }
}

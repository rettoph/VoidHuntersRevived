using System;
using System.Collections.Generic;
using System.CommandLine;
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

        public Int32 Id { get; set; }
        public DateTime ReleaseDate { get; set; }
        public String Version { get; set; }
        public List<Asset> Assets { get; set; }

        public void Download(IConsole console)
        {
            var tempDirectory = Path.Combine(Settings.Default.InstallDirectory, "temp");

            if (Directory.Exists(tempDirectory))
                Directory.Delete(tempDirectory, true);

            foreach (Asset asset in this.Assets)
            {
                var targetDirectory = Path.Combine(Settings.Default.InstallDirectory, asset.Type, this.Version);

                console.Out.Write($"[Update] Downloading {asset.Type} - v{this.Version} to '{targetDirectory}'\n");

                using (WebClient wc = new WebClient())
                {
                    var file = Path.Combine(tempDirectory, Path.GetFileName(asset.downloadURL));
                    Directory.CreateDirectory(tempDirectory);

                    wc.DownloadProgressChanged += (s, a) =>
                    {
                        console.Out.Write($"[Progress] {(Single)a.ProgressPercentage / 100f}\n");

                        if (a.ProgressPercentage == 100)
                        {
                            if (Path.GetExtension(file) == ".zip")
                            { // Unzip...
                                console.Out.Write($"[Update] Extracting {Path.GetFileName(file)}...\n");
                                ZipFile.ExtractToDirectory(file, tempDirectory);
                                File.Delete(file);
                            }

                            try
                            {
                                console.Out.Write($"[Update] Copying to '{targetDirectory}'...\n");
                                Directory.CreateDirectory(targetDirectory);
                                CopyFilesRecursively(new DirectoryInfo(tempDirectory), new DirectoryInfo(targetDirectory));
                                Directory.Delete(tempDirectory, true);
                            }
                            catch (Exception e)
                            {
                                console.Error.Write($"{e.Message}\n");
                                Directory.Delete(targetDirectory, true);
                                Directory.Delete(tempDirectory, true);
                            }
                            finally
                            {
                                console.Out.Write("[Update] Done.\n");
                                _downloading = false;
                            }
                        }
                    };

                    _downloading = true;
                    wc.DownloadFileAsync(
                        new System.Uri(asset.downloadURL),
                        file
                    );

                    while (_downloading)
                        Thread.Sleep(100);
                }
            }
        }

        private static void CopyFilesRecursively(DirectoryInfo source, DirectoryInfo target)
        {
            foreach (DirectoryInfo dir in source.GetDirectories())
                CopyFilesRecursively(dir, target.CreateSubdirectory(dir.Name));
            foreach (FileInfo file in source.GetFiles())
                file.CopyTo(Path.Combine(target.FullName, file.Name), true);
        }
    }
}

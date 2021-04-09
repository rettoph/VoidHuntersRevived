using Microsoft.Extensions.PlatformAbstractions;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VoidHuntersRevived.Client.Launcher.Models;
using VoidHuntersRevived.Client.Launcher.Services;
using Path = System.IO.Path;

namespace VoidHuntersRevived.Client.Launcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private RestClient _checkClient;
        private RestRequest _checkRequest;
        private Release _release;

        public LauncherStatus LauncherStatus { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            _checkClient = new RestClient("http://retto.ph");
            _checkRequest = new RestRequest("releases/latest", Method.GET);
            this.Progress.Value = 0.5f;
            this.SetStatus(LauncherStatus.Checking);
        }

        public void SetStatus(LauncherStatus status)
        {
            this.LauncherStatus = status;

            switch(this.LauncherStatus)
            {
                case LauncherStatus.Checking:
                    this.Check();
                    break;
                case LauncherStatus.Updating:
                    this.Update();
                    break;
                case LauncherStatus.Error:
                    this.Label.Text = "There was an unknown error. Please try again.";
                    break;
                case LauncherStatus.Launching:
                    this.Launch();
                    break;
            }
        }

        private async void Check()
        {
            if(File.Exists("release.json"))
                _release = JsonConvert.DeserializeObject<Release>(File.ReadAllText("release.json"));

            this.Label.Text = "Checking for Updates...";
            this.Progress.Value = 0;

            var response = await _checkClient.ExecuteAsync<Release>(_checkRequest);

            this.Progress.Value = 0.5f;

            if(response.Data == null && _release == null)
            {
                this.SetStatus(LauncherStatus.Error);
            }
            else if(response.Data == null && _release != null)
            {
                this.SetStatus(LauncherStatus.Launching);
            }
            else if(_release?.Version == response.Data.Version)
            {
                this.SetStatus(LauncherStatus.Launching);
            }
            else
            {
                _release = response.Data;
                this.SetStatus(LauncherStatus.Updating);
            }
        }

        private async void Update()
        {
            this.Label.Text = "Downloading Updates...";

            var expectedRuntime = RuntimeIdentifierService.Get();

            Directory.CreateDirectory(_release.TempPath);

            using (WebClient wc = new WebClient())
            {
                var asset = _release.Assets.First(a => a.RID == expectedRuntime && a.Type == "desktop");
                var file = Path.Combine(_release.TempPath, Path.GetFileName(asset.downloadURL));

                wc.DownloadProgressChanged += (s, a) =>
                {
                    this.Progress.Value = (Single)a.ProgressPercentage / 100f;

                    if(a.ProgressPercentage == 100)
                    {
                        if(Path.GetExtension(file) == ".zip")
                        { // Unzip...

                            ZipFile.ExtractToDirectory(file, _release.Path, true);
                            File.Delete(file);
                        }

                        try
                        {
                            Directory.CreateDirectory(_release.Path);
                            CopyFilesRecursively(new DirectoryInfo(_release.TempPath), new DirectoryInfo(_release.Path));
                            Directory.Delete(_release.TempPath, true);
                            File.Delete("release.json");
                            File.WriteAllText("release.json", JsonConvert.SerializeObject(_release));
                        }
                        catch(Exception e)
                        {
                            this.Label.Text = e.Message;
                            Directory.Delete(_release.Path, true);
                        }
                        finally
                        {
                            this.SetStatus(LauncherStatus.Launching);
                        }
                    }
                };

                wc.DownloadFileAsync(
                    new System.Uri(asset.downloadURL),
                    file
                );
            }
        }

        private async void Launch()
        {
            this.Label.Text = "Launching...";
            this.Progress.Value = 1f;

            Process.Start(new ProcessStartInfo()
            {
                FileName = Path.Combine(_release.Path, LauncherConstants.Executable),
                WorkingDirectory = _release.Path,
            });
            Process.GetCurrentProcess().Kill();
        }

        public static void CopyFilesRecursively(DirectoryInfo source, DirectoryInfo target)
        {
            foreach (DirectoryInfo dir in source.GetDirectories())
                CopyFilesRecursively(dir, target.CreateSubdirectory(dir.Name));
            foreach (FileInfo file in source.GetFiles())
                file.CopyTo(Path.Combine(target.FullName, file.Name));
        }
    }
}

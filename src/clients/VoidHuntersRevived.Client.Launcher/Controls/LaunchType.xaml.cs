using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
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

namespace VoidHuntersRevived.Client.Launcher.Controls
{
    /// <summary>
    /// Interaction logic for LaunchType.xaml
    /// </summary>
    public partial class LaunchType : UserControl
    {
        private Boolean _shouldUpdate;

        public String Handle { get; set; }
        public new String Name
        {
            get => this.NameLabel.Content.ToString();
            set => this.NameLabel.Content = value;
        }
        public String Description
        {
            get => this.DescriptionTextBlock.Text.ToString();
            set => this.DescriptionTextBlock.Text = value;
        }

        public LaunchType(String handle, String name, String description)
        {
            InitializeComponent();

            this.Handle = handle;
            this.Name = name;
            this.Description = description;

            this.LaunchButton.IsEnabled = false;

            (new Thread(new ThreadStart(() =>
            {
                var attempt = 0;
                Release latest = default;

                while(latest == default && attempt < 5)
                {
                    attempt++;

                    this.Dispatcher.Invoke(() =>
                    {
                        this.LaunchButton.Content = $"({attempt}) Checking for Updates...";
                    });

                    latest = LauncherService.Info(this.Handle);
                }

                if(latest == default)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        this.LaunchButton.Content = $"Update Server Unavailable";
                    });
                }
                else if (!Directory.Exists(latest?.DownloadPath))
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        _shouldUpdate = true;
                        this.LaunchButton.Content = "Update & Launch";
                        this.LaunchButton.IsEnabled = true;
                    });
                }
                else
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        this.LaunchButton.Content = "Launch";
                        this.LaunchButton.IsEnabled = true;
                    });
                }
            }))).Start();

            this.LaunchButton.Click += this.HandleLaunchClicked;
        }

        private void HandleLaunchClicked(object sender, RoutedEventArgs e)
        {
            this.LaunchButton.IsEnabled = false;
            (new Thread(new ThreadStart(() =>
            {
                if (_shouldUpdate)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        this.LaunchButton.Content = "Updating...";
                    });

                    var proc = LauncherService.Update(this.Handle);

                    while (!proc.StandardOutput.EndOfStream)
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            var line = proc.StandardOutput.ReadLine();
                            if (line.Contains("[Update] "))
                            {
                                this.Description = line.Replace("[Update] ", "");
                            }
                            else if (line.Contains("[Progress] "))
                            {
                                this.ProgressBar.Value = Single.Parse(line.Replace("[Progress] ", ""));
                            }
                        });
                    }
                }

                this.Dispatcher.Invoke(() =>
                {
                    this.LaunchButton.Content = "Launching...";
                });

                LauncherService.Launch(this.Handle);
                Thread.Sleep(2000);

                this.Dispatcher.Invoke(() =>
                {
                    this.LaunchButton.IsEnabled = true;
                    this.LaunchButton.Content = "Launch";
                });
            }))).Start();

        }
    }
}

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
        private String _defaultDescription;
        private Boolean _shouldUpdate;
        private Boolean _killOnLaunch;

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

        public LaunchType(String handle, String name, String description, Boolean killOnLaunch = false)
        {
            InitializeComponent();

            _defaultDescription = description;
            _killOnLaunch = killOnLaunch;

            this.Handle = handle;
            this.Name = name;
            this.Description = description;

            this.LaunchButton.IsEnabled = false;
            this.ProgressBar.Visibility = Visibility.Hidden;

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
                        this.ProgressBar.Visibility = Visibility.Visible;
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

                    _shouldUpdate = false;
                }

                this.Dispatcher.Invoke(() =>
                {
                    this.LaunchButton.Content = "Launching...";
                });

                Thread.Sleep(100);
                LauncherService.Launch(this.Handle);

                if (this._killOnLaunch) // Auto kill after launch...
                    Environment.Exit(0);

                Thread.Sleep(2000);

                this.Dispatcher.Invoke(() =>
                {
                    this.ProgressBar.Visibility = Visibility.Hidden;
                    this.Description = _defaultDescription;

                    this.LaunchButton.IsEnabled = true;
                    this.LaunchButton.Content = "Launch";
                });
            }))).Start();

        }
    }
}

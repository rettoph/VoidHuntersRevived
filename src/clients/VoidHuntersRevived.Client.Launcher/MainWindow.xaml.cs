using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
using VoidHuntersRevived.Client.Launcher.Controls;
using VoidHuntersRevived.Client.Launcher.Services;

namespace VoidHuntersRevived.Client.Launcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var remote = LauncherService.Info("client-launcher", remote: true);
            var local = LauncherService.Info("client-launcher", remote: false);

            if(remote.Version != local.Version)
            { // 
                this.LaunchTypes.Children.Add(new LaunchType("client-launcher", "Launcher", "There is a new version of the launcher available.", true));
            }

            this.LaunchTypes.Children.Add(new LaunchType("desktop", "Desktop", "The primary local game application."));
            this.LaunchTypes.Children.Add(new LaunchType("builder", "Builder", "A simple application used to generate new ShipParts."));
            this.LaunchTypes.Children.Add(new LaunchType("server", "Server", "The main game server for self hosting."));
        }
    }
}

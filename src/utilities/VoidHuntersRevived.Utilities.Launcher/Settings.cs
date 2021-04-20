using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Utilities.Launcher.Models;

namespace VoidHuntersRevived.Utilities.Launcher
{
    public class Settings
    {
        public static Settings Default { get; private set; }
        private static String DefaultDirectory => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "rettoph/VoidHuntersRevived/Launcher");
        private static String DefaultPath => Path.Combine(Settings.DefaultDirectory, "settings.json");

        static Settings()
        {
            Settings.TryLoad();
        }

        private static void TryLoad()
        {
            if (!Directory.Exists(Settings.DefaultPath))
                Directory.CreateDirectory(Settings.DefaultDirectory);

            if (!File.Exists(Settings.DefaultPath))
            {
                Settings.Default = new Settings();
                File.WriteAllText(Settings.DefaultPath, JsonConvert.SerializeObject(Settings.Default));
            }
            else
            {
                try
                {
                    Settings.Default = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(Settings.DefaultPath));
                }
                catch (Exception e)
                {
                    File.Delete(Settings.DefaultPath);
                    Settings.TryLoad();
                }
            }
        }


        public String ReleaseServer { get; set; } = "http://retto.ph";
        public String GetReleaseEndpoint { get; set; } = "release";
        public String GetLatestEndpoint { get; set; } = "release/latest";
        public String InstallDirectory { get; set; } = Path.Combine(Directory.GetCurrentDirectory(), "data");

        public Dictionary<String, String> Executables { get; set; } = new[] {
            ("desktop", "VoidHuntersRevived.Client.Desktop"),
            ("builder", "VoidHuntersRevived.Client.Builder"),
            ("server", "VoidHuntersRevived.Server"),
        }.ToDictionary(keySelector: kvp => kvp.Item1, elementSelector: kvp => kvp.Item2);

        public Release Latest { get; set; }

        public void SaveChanged()
        {
            File.WriteAllText(Settings.DefaultPath, JsonConvert.SerializeObject(this));
        }
    }
}

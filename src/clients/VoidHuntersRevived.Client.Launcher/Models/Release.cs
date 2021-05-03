using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Client.Launcher.Models
{
    public class Release
    {
        public Int32 Id { get; set; }
        public String Version { get; set; }
        public String RID { get; set; }
        public String Type { get; set; }
        public String DownloadUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public String DownloadPath { get; set; }
        public String Executable { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Windows.Launcher.Models
{
    public class Release
    {
        public Int32 Id { get; set; }
        public DateTime ReleaseDate { get; set; }
        public String Version { get; set; }
        public List<Asset> Assets { get; set; }
        public String Path => System.IO.Path.Combine(LauncherConstants.AppDirectory, "Versions", this.Version);
        public String TempPath => System.IO.Path.Combine(LauncherConstants.AppDirectory, "temp");
    }
}

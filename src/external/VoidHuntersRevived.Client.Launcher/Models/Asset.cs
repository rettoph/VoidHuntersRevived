using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Client.Launcher.Models
{
    public class Asset
    {
        public Int32 Id { get; set; }
        public String RID { get; set; }
        public String Type { get; set; }
        public String downloadURL { get; set; }
    }
}

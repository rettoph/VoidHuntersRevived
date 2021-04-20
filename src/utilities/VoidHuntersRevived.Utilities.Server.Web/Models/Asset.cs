using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Utilities.Server.Web.Models
{
    public class Asset
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int32 Id { get; set; }
        public String RID { get; set; }
        public String Type { get; set; }
        public String DownloadURL { get; set; }

        [JsonIgnore]
        public Int32 ReleaseId { get; set; }
        [JsonIgnore]
        public Release Release { get; set; }
    }
}

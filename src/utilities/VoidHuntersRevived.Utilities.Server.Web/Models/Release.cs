using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Utilities.Server.Web.Models
{
    public class Release
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int32 Id { get; set; }
        public String Version { get; set; }
        public String RID { get; set; }
        public String Type { get; set; }
        public String DownloadUrl { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

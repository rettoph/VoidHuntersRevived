using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Server.Web.Models
{
    public class Release
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int32 Id { get; set; }

        public DateTime ReleaseDate { get; set; }
        public String Version { get; set; }

        public List<Asset> Assets { get; set; }
    }
}

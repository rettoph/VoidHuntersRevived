using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;

namespace VoidHuntersRevived.Common.Pieces.Services
{
    public interface IBlueprintService
    {
        Blueprint GetById(Id<Blueprint> id);
        IEnumerable<Blueprint> GetAll();
    }
}

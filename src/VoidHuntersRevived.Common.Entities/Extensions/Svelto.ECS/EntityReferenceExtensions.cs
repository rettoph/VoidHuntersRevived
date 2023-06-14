using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Svelto.ECS
{
    public static class EntityReferenceExtensions
    {
        public static unsafe int GetFilterId(this EntityReference reference)
        {
            return ((int*)&reference.uniqueID)[0];
        }
    }
}

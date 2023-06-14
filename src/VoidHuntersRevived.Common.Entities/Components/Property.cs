using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Entities.Components
{
    public struct Property<T> : IEntityComponent
        where T : IEntityProperty
    {
        public int Id;
    }
}

using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;

namespace VoidHuntersRevived.Common.Ships.Components
{
    public struct TractorBeamEmitter : IEntityComponent
    {
        public static FilterContextID TractorableFilterContext = FilterContextID.GetNewContextID();

        public readonly EntityId Id;
        public bool Active;
        public EntityId TargetId;

        public TractorBeamEmitter(EntityId id) : this()
        {
            this.Id = id;
            this.Active = false;
        }
    }
}

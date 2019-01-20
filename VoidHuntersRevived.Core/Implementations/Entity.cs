using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Core.Structs;

namespace VoidHuntersRevived.Core.Implementations
{
    public abstract class Entity : LayerObject, IEntity
    {
        public EntityInfo Info { get; protected set; }

        public Entity(EntityInfo info, IGame game) : base(game)
        {
            this.Info = info;

            this.Visible = true;
            this.Enabled = true;
        }
    }
}

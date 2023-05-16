using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Entities.ShipParts.Components
{
    public abstract class ShipPartComponent
    {
        public abstract ShipPartComponent Clone();

        public abstract void AttachTo(Entity entity);
    }

    public abstract class ShipPartComponent<TComponent> : ShipPartComponent
        where TComponent : ShipPartComponent<TComponent>
    {
        public override void AttachTo(Entity entity)
        {
            entity.Attach<TComponent>((this as TComponent)!);
        }
    }
}

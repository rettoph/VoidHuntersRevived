using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Components;

namespace VoidHuntersRevived.Common.Entities
{
    public partial class ShipPartConfiguration
    {
        private abstract class ComponentWrapper
        {
            public readonly IShipPartComponent Component;

            protected ComponentWrapper(IShipPartComponent component)
            {
                this.Component = component;
            }

            public abstract void Maker(Entity entity);
        }
        private class ComponentWrapper<TComponent> : ComponentWrapper
            where TComponent : class, IShipPartComponent
        {
            public new readonly TComponent Component;

            public ComponentWrapper(TComponent component) : base(component)
            {
                this.Component = component;
            }

            public override void Maker(Entity entity)
            {
                entity.Attach(this.Component);
            }
        }
    }
}

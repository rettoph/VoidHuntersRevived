using Guppy.EntityComponent;
using Guppy.EntityComponent.DependencyInjection;
using Guppy.EntityComponent.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Library.Components
{
    public class ReferenceComponent<TEntity, TReferenceComponent> : Component<TEntity>
        where TEntity : class, IEntity
        where TReferenceComponent : Component<TEntity>
    {
        public TReferenceComponent Reference { get; private set; }

        protected override void Initialize(ServiceProvider provider)
        {
            base.Initialize(provider);

            this.Reference = this.Entity.Components.Get<TReferenceComponent>();
        }

        protected override void PostUninitialize()
        {
            base.PostUninitialize();

            this.Reference = default;
        }
    }
}

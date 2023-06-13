using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Pieces;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Services;

namespace VoidHuntersRevived.Domain.Pieces
{
    internal abstract class PiecePropertyConfiguration : IPiecePropertyConfiguration
    {
        public abstract Type Type { get; }

        public abstract void Initialize(PieceProperty property, IEntityInitializer entity);
    }
    internal sealed class PiecePropertyConfiguration<T> : PiecePropertyConfiguration, IPiecePropertyConfiguration<T>
        where T : class, IPieceProperty
    {
        private Action<PieceProperty<T>, IEntityInitializer> _initializers = null!;

        public override Type Type => typeof(T);

        public PiecePropertyConfiguration()
        {
            this.RequiresComponent((PieceProperty<T> property, ref PiecePropertyId<T> component) =>
            {
                component.Value = property.Id.Value;
            });
        }

        public void RequiresComponent<TComponent>(InitializeComponentDelegate<T, TComponent> initializer)
            where TComponent : unmanaged
        {
            _initializers += (PieceProperty<T> property, IEntityInitializer entity) =>
            {
                initializer(property, ref entity.Get<TComponent>());
            };
        }

        public override void Initialize(PieceProperty property, IEntityInitializer entity)
        {
            _initializers(Unsafe.As<PieceProperty<T>>(property), entity);
        }
    }
}

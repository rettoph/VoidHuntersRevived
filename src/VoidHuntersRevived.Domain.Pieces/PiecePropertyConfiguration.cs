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
using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Abstractions;

namespace VoidHuntersRevived.Domain.Pieces
{
    public abstract class PiecePropertyConfiguration : IPiecePropertyConfiguration
    {
        public abstract Type Type { get; }
        public abstract IEnumerable<Type> Components { get; }

        public abstract void Initialize(PieceProperty property, ref EntityInitializer entity);
    }
    public sealed class PiecePropertyConfiguration<T> : PiecePropertyConfiguration, IPiecePropertyConfiguration<T>
        where T : class, IPieceProperty
    {
        private delegate void PieceInitializerDelegate(PieceProperty<T> property, ref EntityInitializer initializer);
        private PieceInitializerDelegate _initializers = null!;
        private List<Type> _components;

        public override Type Type => typeof(T);

        public override IEnumerable<Type> Components => _components;

        public PiecePropertyConfiguration()
        {
            _components = new List<Type>();

            this.RequiresComponent((PieceProperty<T> property, ref Piece<T> component) =>
            {
                component.Value = property.Component.Value;
            });
        }

        public void RequiresComponent<TComponent>(InitializeComponentDelegate<T, TComponent> initializer)
            where TComponent : unmanaged, IEntityComponent
        {
            _components.Add(typeof(TComponent));
            _initializers += (PieceProperty<T> property, ref EntityInitializer entity) =>
            {
                initializer(property, ref entity.Get<TComponent>());
            };
        }

        public override void Initialize(PieceProperty property, ref EntityInitializer entity)
        {
            _initializers(Unsafe.As<PieceProperty<T>>(property), ref entity);
        }
    }
}

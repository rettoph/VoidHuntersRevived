using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Pieces;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Services;

namespace VoidHuntersRevived.Domain.Pieces
{
    internal sealed class PieceCategoryConfiguration : IPieceCategoryConfiguration
    {
        private readonly HashSet<Type> _properties;

        public PieceCategory Category { get; private set; }

        public PieceCategoryConfiguration(PieceCategory category)
        {
            _properties = new HashSet<Type>();

            this.Category = category;
        }

        public IPieceCategoryConfiguration HasProperty<T>() 
            where T : class, IPieceProperty
        {
            _properties.Add(typeof(T));

            return this;
        }

        public void Initialize(IEntityTypeService entityTypes)
        {
            entityTypes.Configure(this.Category, configuration =>
            {
                foreach(Type property in _properties)
                {
                    configuration.Has(typeof(PiecePropertyId<>).MakeGenericType(property));
                }
            });
        }
    }
}

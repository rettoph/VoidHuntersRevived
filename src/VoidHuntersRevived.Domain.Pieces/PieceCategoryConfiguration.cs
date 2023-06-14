using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Pieces;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Pieces.Properties;
using VoidHuntersRevived.Domain.Pieces.Services;

namespace VoidHuntersRevived.Domain.Pieces
{
    public sealed class PieceCategoryConfiguration : IPieceCategoryConfiguration
    {
        private readonly HashSet<Type> _properties;

        public PieceCategory Category { get; private set; }

        public IEnumerable<Type> PropertyTypes => _properties;

        public PieceCategoryConfiguration(PieceCategory category)
        {
            _properties = new HashSet<Type>();

            this.Category = category;

            this.HasProperty<Core>();
        }

        public IPieceCategoryConfiguration HasProperty<T>() 
            where T : class, IPieceProperty
        {
            _properties.Add(typeof(T));

            return this;
        }

        public void Initialize(PiecePropertyService properties, IEntityTypeService entityTypes)
        {
            // entityTypes.Configure(this.Category, configuration =>
            // {
            //     foreach(Type property in _properties)
            //     {
            //         foreach(Type component in properties.Get(property).Components)
            //         {
            //             configuration.HasComponent(component);
            //         }
            //     }
            // });
        }
    }
}

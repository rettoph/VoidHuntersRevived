using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Pieces;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Pieces.Services;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Domain.Pieces.Services;
using Svelto.ECS;

namespace VoidHuntersRevived.Domain.Pieces
{
    internal sealed class PieceConfiguration : IPieceConfiguration
    {
        private readonly PieceCategoryService _categories;
        private readonly PiecePropertyService _properties;

        private HashSet<PieceProperty> _propertyInstances;

        public PieceType Type { get; }

        public PieceCategory Category { get; set; }

        public PieceConfiguration(PieceType id, PieceCategoryService categories, PiecePropertyService properties)
        {
            _propertyInstances = new HashSet<PieceProperty>();
            _categories = categories;
            _properties = properties;

            this.Type = id;
            this.Category = null!;
        }

        public IEnumerable<PieceProperty> Properties => _propertyInstances;

        public IPieceConfiguration AddProperty<T>(T property)
            where T : class, IPieceProperty
        {
            _propertyInstances.Add(_properties.Cache(property));

            return this;
        }

        public IPieceConfiguration SetCategory(PieceCategory category)
        {
            this.Category = category;

            return this;
        }

        public void Initialize(ref EntityInitializer initializer)
        {
            foreach (PieceProperty property in _propertyInstances)
            {
                _properties.Initialize(property, ref initializer);
            }
        }
    }
}

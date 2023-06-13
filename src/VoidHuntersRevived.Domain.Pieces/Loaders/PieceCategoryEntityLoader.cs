using Guppy.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Pieces;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Entities.Loaders;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Domain.Pieces.Services;

namespace VoidHuntersRevived.Domain.Pieces.Loaders
{
    [AutoLoad]
    internal sealed class PieceCategoryEntityLoader : IEntityTypeLoader
    {
        private readonly PieceCategoryService _categories;
        private readonly PiecePropertyService _properties;

        public PieceCategoryEntityLoader(PieceCategoryService categories, PiecePropertyService properties)
        {
            _categories = categories;
            _properties = properties;
        }

        public void Configure(IEntityTypeService entityTypes)
        {
            _categories.Initialize(_properties, entityTypes);
        }
    }

    public class TestProperty : IPieceProperty
    {

    }
}

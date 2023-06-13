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

        public PieceCategoryEntityLoader(PieceCategoryService categories)
        {
            _categories = categories;
        }

        public void Configure(IEntityTypeService entityTypes)
        {
            _categories.Initialize(entityTypes);
        }
    }

    public class TestProperty : IPieceProperty
    {

    }
}

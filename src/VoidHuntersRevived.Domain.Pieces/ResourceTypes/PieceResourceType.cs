﻿using Guppy.Attributes;
using Guppy.Files;
using Guppy.Files.Enums;
using Guppy.Files.Services;
using Guppy.Resources;
using Guppy.Resources.ResourceTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Pieces;

namespace VoidHuntersRevived.Domain.Pieces.ResourceTypes
{
    [AutoLoad]
    internal class PieceResourceType : ResourceType<Piece>
    {
        private readonly IFileService _files;

        public PieceResourceType(IFileService files)
        {
            _files = files;
        }

        protected override bool TryResolve(Resource<Piece> resource, string input, string root, out Piece value)
        {
            IFile<Piece> piece = _files.Get<Piece>(
                FileType.Source,
                Path.Combine(root, input),
                true);

            value = piece.Value;
            return piece.Success;
        }
    }
}

﻿using Guppy.Attributes;
using Guppy.Files;
using Guppy.Files.Enums;
using Guppy.Files.Services;
using Guppy.Resources;
using Guppy.Resources.Serialization.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Pieces;

namespace VoidHuntersRevived.Game.Pieces.Serialization.ResourceTypes
{
    [AutoLoad]
    internal class PieceResourceTypeResolver : ResourceTypeResolver<Piece>
    {
        private readonly IFileService _files;
        private string _root;

        public PieceResourceTypeResolver(IFileService files)
        {
            _files = files;
            _root = string.Empty;
        }

        protected override void Configure(ResourcePack pack)
        {
            base.Configure(pack);

            _root = pack.RootDirectory;
        }

        protected override bool TryResolve(Resource<Piece> resource, string input, out Piece value)
        {
            IFile<Piece> piece = _files.Get<Piece>(
                FileType.Source,
                Path.Combine(_root, input),
                true);

            value = piece.Value;
            return value != default;
        }
    }
}

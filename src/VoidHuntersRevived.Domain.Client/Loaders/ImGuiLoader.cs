using Guppy.Common;
using Guppy.Loaders;
using Guppy.MonoGame.Constants;
using Guppy.MonoGame.UI.Constants;
using Guppy.Resources.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guppy.MonoGame.UI.Providers;
using Guppy.Attributes;
using VoidHuntersRevived.Common.Constants;
using Guppy;

namespace VoidHuntersRevived.Domain.Client.Loaders
{
    [AutoLoad]
    internal sealed class ImGuiLoader : IGuppyLoader
    {
        private readonly IImGuiBatchProvider _batches;

        public ImGuiLoader(IImGuiBatchProvider batchs)
        {
            _batches = batchs;
        }

        public void Load(IGuppy guppy)
        {
            var batch = _batches.Get(ImGuiBatches.Primary);

            batch.Fonts.Add(ResourceConstants.DiagnosticsImGuiFont, ResourceConstants.DiagnosticsImGuiFontHeader);
            batch.RebuildFontAtlas();
        }
    }
}

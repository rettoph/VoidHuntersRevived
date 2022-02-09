using Guppy.EntityComponent.DependencyInjection;
using Guppy.Interfaces;
using Guppy.EntityComponent.Lists;
using Guppy.Network.Components;
using Guppy.Network.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.Chunks;
using VoidHuntersRevived.Library.Entities.WorldObjects;
using VoidHuntersRevived.Library.Interfaces;
using VoidHuntersRevived.Library.Scenes;
using VoidHuntersRevived.Library.Structs;
using Guppy.Network.Attributes;
using Guppy.EntityComponent;

namespace VoidHuntersRevived.Library.Components.Chunks
{
    [HostTypeRequired(HostType.Remote)]
    internal sealed class ChunkPipeComponent : Component<Chunk>
    {
        #region Private Fields
        private PrimaryScene _scene;
        #endregion

        #region Lifecycle Methods
        protected override void Initialize(ServiceProvider provider)
        {
            base.Initialize(provider);

            provider.Service(out _scene);

            this.Entity.Pipe = _scene.Room.Pipes.GetById(this.Entity.Id);
        }

        protected override void Uninitialize()
        {
            base.Uninitialize();

            this.Entity.Pipe?.Dispose();
            this.Entity.Pipe = default;
        }
        #endregion
    }
}

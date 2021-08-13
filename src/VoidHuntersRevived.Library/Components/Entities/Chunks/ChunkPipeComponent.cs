using Guppy.DependencyInjection;
using Guppy.Enums;
using Guppy.Interfaces;
using Guppy.Lists;
using Guppy.Lists.Interfaces;
using Guppy.Network.Components;
using Guppy.Network.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.Chunks;
using VoidHuntersRevived.Library.Entities.WorldObjects;
using VoidHuntersRevived.Library.Scenes;
using VoidHuntersRevived.Library.Structs;

namespace VoidHuntersRevived.Library.Components.Entities.Chunks
{
    internal sealed class ChunkPipeComponent : RemoteHostComponent<Chunk>
    {
        #region Private Fields
        private PrimaryScene _scene;
        #endregion

        #region Lifecycle Methods
        protected override void InitializeRemote(GuppyServiceProvider provider, NetworkAuthorization networkAuthorization)
        {
            base.InitializeRemote(provider, networkAuthorization);

            provider.Service(out _scene);

            this.Entity.OnPositionSet += this.HandleChunkPositionSet;
            this.Entity.OnChildrenSet += this.HandleChunkChildrenSet;
            this.Entity.OnStatus[ServiceStatus.Releasing] += this.HandleChunkReleasing;
        }

        protected override void ReleaseRemote(NetworkAuthorization networkAuthorization)
        {
            base.ReleaseRemote(networkAuthorization);

            this.Entity.OnPositionSet -= this.HandleChunkPositionSet;
            this.Entity.OnChildrenSet -= this.HandleChunkChildrenSet;
            this.Entity.OnStatus[ServiceStatus.Releasing] -= this.HandleChunkReleasing;
        }
        #endregion

        #region Event Handlers
        private void HandleChunkChildAdded(IServiceList<IWorldObject> sender, IWorldObject args)
        {
            this.Entity.Pipe.NetworkEntities.TryAdd(args);
        }

        private void HandleChunkPositionSet(Chunk sender, ChunkPosition args)
        {
            this.Entity.Pipe = _scene.Channel.Pipes.GetOrCreateById(this.Entity.Id);
        }

        private void HandleChunkChildrenSet(Chunk sender, ServiceList<IWorldObject> args)
        {
            this.Entity.Children.OnAdded += this.HandleChunkChildAdded;
        }

        private void HandleChunkReleasing(IService sender, ServiceStatus old, ServiceStatus value)
        {
            this.Entity.Children.OnAdded -= this.HandleChunkChildAdded;
        }
        #endregion
    }
}

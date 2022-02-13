using Guppy.EntityComponent;
using Guppy.EntityComponent.DependencyInjection;
using Guppy.Network.Attributes;
using Guppy.Network.Components;
using Guppy.Network.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.Chunks;
using VoidHuntersRevived.Library.Globals.Constants;
using VoidHuntersRevived.Library.Services;

namespace VoidHuntersRevived.Library.Components.Chunks
{
    [NetworkAuthorizationRequired(NetworkAuthorization.Master)]
    internal sealed class ChunkManagerMasterPopulationComponent : Component<ChunkManager>
    {
        private ChainService _chains;
        private Random _rand;

        protected override void PostInitialize(ServiceProvider provider)
        {
            base.PostInitialize(provider);

            _rand = new Random();

            provider.Service(out _chains);

            this.Entity.OnChunkAdded += (_, chunk) =>
            {
                int total = Random.Shared.Next(1, 3);
                //total = 01;

                for (var i = 0; i < total; i++)
                {
                    var square = _chains.Create(
                        contextName: ShipParts.HullSquare,
                        position: chunk.Bounds.Center.ToVector2() + _rand.NextVector2(-(Chunk.Size / 2), (Chunk.Size / 2)));

                    var triangle = _chains.Create(
                        contextName: ShipParts.HullTriangle,
                        position: chunk.Bounds.Center.ToVector2() + _rand.NextVector2(-(Chunk.Size / 2), (Chunk.Size / 2)));

                    int tTotal = Random.Shared.Next(1, 5);
                    for (int j = 0; j < tTotal; j++)
                    {
                        var thruster = _chains.Create(
                            contextName: ShipParts.Thruster,
                            position: chunk.Bounds.Center.ToVector2() + _rand.NextVector2(-(Chunk.Size / 2), (Chunk.Size / 2)));
                    }
                }
            };
        }
    }
}

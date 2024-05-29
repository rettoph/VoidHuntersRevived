using Guppy.Core.Common.Attributes;
using Guppy.Game.Common.Enums;
using Guppy.Game.MonoGame.Common.Utilities.Cameras;
using Serilog;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Components;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Pieces.Common.Components.Instance;
using VoidHuntersRevived.Domain.Pieces.Common.Components.Static;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Attributes;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;
using VoidHuntersRevived.Domain.Teams.Common.Components;
using VoidHuntersRevived.Game.Client.Common.Engines;
using VoidHuntersRevived.Game.Client.Common.Graphics.Vertices;
using VoidHuntersRevived.Game.Client.Common.Services;
using VoidHuntersRevived.Game.Client.Common.Utilities;

namespace VoidHuntersRevived.Game.Client.Engines
{
    [AutoLoad]
    [SimulationFilter(SimulationType.Predictive)]
    [Sequence<DrawSequence>(DrawSequence.Draw)]
    internal sealed class DrawVisibleEngine : BasicEngine, IDrawVisibleEngine
    {
        private readonly IEntityService _entities;
        private readonly IEntityTypeService _types;
        private readonly ILogger _logger;
        private readonly Camera2D _camera;

        public string name { get; } = nameof(DrawVisibleEngine);

        public DrawVisibleEngine(
            ILogger logger,
            IEntityService entities,
            IEntityTypeService types,
            Camera2D camera)
        {
            _entities = entities;
            _types = types;
            _logger = logger;
            _camera = camera;
        }

        public void Step(in IVertexBufferManagerService<VertexInstanceVisible, Id<IEntityType>> param)
        {
            foreach (var ((typeEntities, hasManyInstances, colorSchemes, _, typeCount), _) in _entities.QueryEntities<TypeEntity, HasMany<InstanceEntity, TypeEntity>, ColorScheme, Visible>())
            {
                for (int i = 0; i < typeCount; i++)
                {
                    Id<IEntityType> entityType = typeEntities[i].TypeId;

                    ref HasMany<InstanceEntity, TypeEntity> hasManyIntances = ref hasManyInstances[i];
                    ref ColorScheme colorScheme = ref colorSchemes[i];

                    VertexBufferManager<VertexInstanceVisible> vertexBufferManager = param.GetById(entityType);

                    foreach (var (indices, group) in hasManyIntances.Items)
                    {
                        var (statuses, nodes, instanceCount) = _entities.QueryEntities<EntityStatus, Node>(group);
                        vertexBufferManager.EnsureFit(instanceCount);

                        for (int j = 0; j < indices.count; j++)
                        {
                            uint index = indices[j];
                            if (statuses[index].IsDespawned)
                            { // Dont render pieces that have been despawned
                                continue;
                            }

                            ref Node node = ref nodes[index];

                            ref VertexInstanceVisible instanceVertex = ref vertexBufferManager.GetNextVertexUnsafe();

                            instanceVertex.LocalTransformation = node.XnaTransformation;
                            instanceVertex.PrimaryColor = colorScheme.Primary.Value.PackedValue;
                            instanceVertex.SecondaryColor = colorScheme.Secondary.Value.PackedValue;
                        }
                    }
                }
            }
        }
    }
}

using Guppy.Attributes;
using Guppy.Common.Attributes;
using Guppy.Game.Common.Enums;
using Guppy.Game.MonoGame.Utilities.Cameras;
using Serilog;
using Svelto.ECS;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Pieces.Common.Components.Instance;
using VoidHuntersRevived.Domain.Pieces.Common.Components.Static;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Attributes;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;
using VoidHuntersRevived.Game.Client.Common.Engines;
using VoidHuntersRevived.Game.Client.Common.Graphics.Vertices;
using VoidHuntersRevived.Game.Client.Common.Services;
using VoidHuntersRevived.Game.Client.Common.Utilities;

namespace VoidHuntersRevived.Game.Client.Engines
{
    [AutoLoad]
    [GuppyFilter<LocalGameGuppy>]
    [SimulationFilter(SimulationType.Predictive)]
    [Sequence<DrawSequence>(DrawSequence.Draw)]
    internal sealed class DrawVisibleEngine : BasicEngine, IDrawVisibleEngine
    {
        private readonly short[] _indexBuffer;
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
            foreach (var ((statics, entityTypes, _, _, typeCount), _) in _entities.QueryEntities<StaticEntity, Id<IEntityType>, Visible, zIndex>())
            {
                for (int i = 0; i < typeCount; i++)
                {
                    ref StaticEntity @static = ref statics[i];
                    ref Id<IEntityType> entityType = ref entityTypes[i];

                    ref var instanceFilter = ref _entities.GetFilter<EntityId>(@static.InstanceEntitiesFilterId);

                    VertexBufferManager<VertexInstanceVisible> vertexBufferManager = param.GetById(entityType);

                    foreach (var (indices, group) in instanceFilter)
                    {
                        var (statuses, nodes, colorSchemes, instanceCount) = _entities.QueryEntities<EntityStatus, Node, ColorScheme>(group);
                        vertexBufferManager.EnsureFit(instanceCount);

                        for (int j = 0; j < indices.count; j++)
                        {
                            uint index = indices[j];
                            if (statuses[index].IsDespawned)
                            {
                                continue;
                            }

                            ref Node node = ref nodes[index];
                            ref ColorScheme colorScheme = ref colorSchemes[index];

                            ref VertexInstanceVisible instanceVertex = ref vertexBufferManager.GetNextVertexUnsafe();

                            instanceVertex.LocalTransformation = node.XnaTransformation;
                            instanceVertex.PrimaryColor = colorScheme.Primary.Current.PackedValue;
                            instanceVertex.SecondaryColor = colorScheme.Secondary.Current.PackedValue;
                        }
                    }
                }
            }
        }
    }
}

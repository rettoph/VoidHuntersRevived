using Guppy.Attributes;
using Guppy.Common.Attributes;
using Guppy.GUI;
using Guppy.MonoGame.Primitives;
using Guppy.MonoGame.Utilities.Cameras;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Screens;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Simulations.Attributes;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Engines;
using VoidHuntersRevived.Common.Simulations.Enums;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Domain.Simulations;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Pieces.Services;
using VoidHuntersRevived.Common.FixedPoint.Utilities;
using VoidHuntersRevived.Common.FixedPoint.Extensions;

namespace VoidHuntersRevived.Game.Client.Engines
{
    [AutoLoad]
    [Sequence<DrawEngineSequence>(DrawEngineSequence.PostDraw)]
    [SimulationTypeFilter(SimulationType.Predictive)]
    internal class DrawSocketsEngine : BasicEngine, IStepEngine<GameTimeTeam>
    {
        private readonly IScreen _screen;
        private readonly IEntityService _entities;
        private readonly Camera2D _camera;
        private readonly PrimitiveBatch<VertexPositionColor> _primitiveBatch;
        private readonly PrimitiveShape _jointShape;
        private readonly Dictionary<TeamId, ITeamDescriptorGroup[]> _teamDescriptorGroups;

        public DrawSocketsEngine(IScreen screen, IEntityService entities, ITeamDescriptorGroupService teamDescriptorGroups, Camera2D camera, PrimitiveBatch<VertexPositionColor> primitiveBatch)
        {
            _screen = screen;
            _entities = entities;
            _camera = camera;
            _primitiveBatch = primitiveBatch;
            _jointShape = new ProjectedShape(camera, new[]
            {
                new Vector2(-0.05f, -0.05f),
                new Vector2(0f, 0f),
                new Vector2(-0.05f, 0.05f),
            });

            _teamDescriptorGroups = teamDescriptorGroups.GetAllWithComponentsByTeams(typeof(Sockets), typeof(Node));
        }
        public string name { get; } = nameof(DrawSocketsEngine);

        public void Step(in GameTimeTeam _param)
        {
            if(_camera.Zoom < 60)
            {
                return;
            }

            _primitiveBatch.Begin(_screen.Camera);
            foreach (ITeamDescriptorGroup teamDescriptorGroup in _teamDescriptorGroups[_param.Team.Id])
            {
                var (statuses, sockets, nodes, count) = _entities.QueryEntities<EntityStatus, Sockets, Node>(teamDescriptorGroup.GroupId);
                for (int index = 0; index < count; index++)
                {
                    if (_camera.Frustum.Contains(nodes[index].Transformation.GetBoudingSphere(5f)) == ContainmentType.Disjoint)
                    {
                        continue;
                    }

                    for (int j = 0; j < sockets[index].Items.count; j++)
                    {
                        if (statuses[index].IsSpawned)
                        {
                            Matrix socketsTransformation = FixMatrixHelper.FastMultiplyTransformationsToXnaMatrix(sockets[index].Items[j].Location.Transformation, nodes[index].Transformation);
                           
                            _primitiveBatch.Trace(_jointShape, teamDescriptorGroup.SecondaryColor, socketsTransformation);
                        }
                    }

                }
            }
            _primitiveBatch.End();
        }
    }
}

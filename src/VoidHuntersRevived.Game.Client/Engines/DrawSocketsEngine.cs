using Guppy.Attributes;
using Guppy.Common.Attributes;
using Guppy.Game.Common.Enums;
using Guppy.Game.MonoGame;
using Guppy.Game.MonoGame.Primitives;
using Guppy.Game.MonoGame.Utilities.Cameras;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Svelto.ECS;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Physics.Components;
using VoidHuntersRevived.Common.Pieces;
using VoidHuntersRevived.Common.Pieces.Components.Instance;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Attributes;
using VoidHuntersRevived.Common.Simulations.Engines;

namespace VoidHuntersRevived.Game.Client.Engines
{
    [AutoLoad]
    [Sequence<DrawSequence>(DrawSequence.PostDraw)]
    [SimulationFilter(SimulationType.Predictive)]
    internal class DrawSocketsEngine : BasicEngine, IStepEngine<GameTimeTeam>
    {
        private readonly IScreen _screen;
        private readonly IEntityService _entities;
        private readonly Camera2D _camera;
        private readonly PrimitiveBatch<VertexPositionColor> _primitiveBatch;
        private readonly PrimitiveShape _jointShape;
        private readonly Dictionary<Id<ITeam>, ITeamDescriptorGroup[]> _teamDescriptorGroups;

        public DrawSocketsEngine(IScreen screen, IEntityService entities, ITeamDescriptorGroupService teamDescriptorGroups, Camera2D camera, PrimitiveBatch<VertexPositionColor> primitiveBatch)
        {
            _screen = screen;
            _entities = entities;
            _camera = camera;
            _primitiveBatch = primitiveBatch;
            _jointShape = new PrimitiveShape(new[]
            {
                new Vector2(-0.05f, -0.05f),
                new Vector2(0f, 0f),
                new Vector2(-0.05f, 0.05f),
            });

            _teamDescriptorGroups = teamDescriptorGroups.GetAllWithComponentsByTeams(typeof(Sockets<SocketId>), typeof(Sockets<Location>), typeof(Node));
        }
        public string name { get; } = nameof(DrawSocketsEngine);

        public void Step(in GameTimeTeam _param)
        {
            // if (_camera.Zoom < 60)
            // {
            //     return;
            // }
            // 
            // var bounds = _camera.Frustum.ToBounds2D();
            // 
            // _primitiveBatch.Begin(_camera);
            // foreach (ITeamDescriptorGroup teamDescriptorGroup in _teamDescriptorGroups[_param.Team.Id])
            // {
            //     var (statuses, nodes, socketLocationses, count) = _entities.QueryEntities<EntityStatus, Node, Sockets<Location>>(teamDescriptorGroup.GroupId);
            //     for (uint index = 0; index < count; index++)
            //     {
            //         Node node = nodes[index];
            //         if (bounds.Contains(node.XnaTransformation) == false)
            //         {
            //             continue;
            //         }
            // 
            //         Sockets<Location> socketLocations = socketLocationses[index];
            //         for (int j = 0; j < socketLocations.Items.count; j++)
            //         {
            //             if (statuses[index].IsSpawned)
            //             {
            //                 Matrix transformationMatrix = FixMatrixHelper.FastMultiplyTransformationsToXnaMatrix(socketLocations.Items[j].Transformation, node.Transformation);
            //                 _primitiveBatch.Trace(_jointShape, teamDescriptorGroup.SecondaryColor, transformationMatrix);
            //             }
            //         }
            //     }
            // }
            // _primitiveBatch.End();
        }
    }
}

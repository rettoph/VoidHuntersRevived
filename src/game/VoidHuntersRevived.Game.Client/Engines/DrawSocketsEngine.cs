using Guppy.Core.Common.Attributes;
using Guppy.Game.MonoGame.Common;
using Guppy.Game.MonoGame.Common.Primitives;
using Guppy.Game.MonoGame.Common.Utilities.Cameras;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common.Attributes;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;

namespace VoidHuntersRevived.Game.Client.Engines
{
    [AutoLoad]
    [StrategyFilter(StrategyTypeEnum.Predictive)]
    internal class DrawSocketsEngine(IScreen screen, IEntityQueryService entityQueryService, Camera2D camera, PrimitiveBatch<VertexPositionColor> primitiveBatch) : StrategyEngine
    {
        private readonly IScreen _screen = screen;
        private readonly IEntityQueryService _entityQueryService = entityQueryService;
        private readonly Camera2D _camera = camera;
        private readonly PrimitiveBatch<VertexPositionColor> _primitiveBatch = primitiveBatch;
        private readonly PrimitiveShape _jointShape = new(new[]
            {
                new Vector2(-0.05f, -0.05f),
                new Vector2(0f, 0f),
                new Vector2(-0.05f, 0.05f),
            });

        public string name { get; } = nameof(DrawSocketsEngine);

        // public void Step(in GameTimeTeam _param)
        // {
        //     // if (_camera.Zoom < 60)
        //     // {
        //     //     return;
        //     // }
        //     // 
        //     // var bounds = _camera.Frustum.ToBounds2D();
        //     // 
        //     // _primitiveBatch.Begin(_camera);
        //     // foreach (ITeamDescriptorGroup teamDescriptorGroup in _teamDescriptorGroups[_param.Team.Id])
        //     // {
        //     //     var (statuses, nodes, socketLocationses, count) = _entities.QueryEntities<EntityStatus, Node, Sockets<Location>>(teamDescriptorGroup.GroupId);
        //     //     for (uint index = 0; index < count; index++)
        //     //     {
        //     //         Node node = nodes[index];
        //     //         if (bounds.Contains(node.XnaTransformation) == false)
        //     //         {
        //     //             continue;
        //     //         }
        //     // 
        //     //         Sockets<Location> socketLocations = socketLocationses[index];
        //     //         for (int j = 0; j < socketLocations.Items.count; j++)
        //     //         {
        //     //             if (statuses[index].IsSpawned)
        //     //             {
        //     //                 Matrix transformationMatrix = FixMatrixHelper.FastMultiplyTransformationsToXnaMatrix(socketLocations.Items[j].Transformation, node.Transformation);
        //     //                 _primitiveBatch.Trace(_jointShape, teamDescriptorGroup.SecondaryColor, transformationMatrix);
        //     //             }
        //     //         }
        //     //     }
        //     // }
        //     // _primitiveBatch.End();
        // }
    }
}

using Guppy.Attributes;
using Guppy.GUI;
using Guppy.MonoGame.Primitives;
using Guppy.MonoGame.Utilities.Cameras;
using Guppy.Network.Peers;
using Guppy.Resources.Providers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Serilog;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Simulations.Engines;
using VoidHuntersRevived.Game.Services;
using Guppy.Network.Identity;
using VoidHuntersRevived.Common.Entities.Services;
using System.Text.RegularExpressions;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Pieces;
using Guppy.Common.Attributes;
using VoidHuntersRevived.Common.Simulations.Enums;
using VoidHuntersRevived.Common.Simulations.Attributes;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Game.Client.Engines
{
    [AutoLoad]
    [Sequence<DrawEngineSequence>(DrawEngineSequence.PostDraw)]
    [SimulationTypeFilter(SimulationType.Predictive)]
    internal class TractorBeamSelectionEngine : BasicEngine, IStepEngine<GameTime>
    {
        private readonly short[] _indexBuffer;
        private readonly IScreen _screen;
        private readonly Camera2D _camera;
        private readonly PrimitiveBatch<VertexPositionColor> _primitiveBatch;
        private readonly IResourceProvider _resources;
        private readonly ILogger _logger;
        private readonly IEntityService _entities;
        private readonly ClientPeer _client;
        private readonly TractorBeamEmitterService _tractorBeamEmitterService;

        public string name { get; } = nameof(TractorBeamSelectionEngine);

        private Vector2 CurrentTargetPosition => _camera.Unproject(Mouse.GetState().Position.ToVector2());

        public TractorBeamSelectionEngine(
            IScreen screen, 
            ILogger logger, 
            IEntityService entities,
            Camera2D camera, 
            PrimitiveBatch<VertexPositionColor> primitiveBatch, 
            IResourceProvider resources,
            ClientPeer client,
            TractorBeamEmitterService tractorBeamEmitterService)
        {
            _screen = screen;
            _camera = camera;
            _entities = entities;
            _primitiveBatch = primitiveBatch;
            _resources = resources;
            _indexBuffer = new short[3];
            _logger = logger;
            _client = client;
            _tractorBeamEmitterService = tractorBeamEmitterService;
        }


        public void Step(in GameTime _param)
        {
            try
            {
                if (_client.Users.Current is null)
                {
                    return;
                }

                if (!_entities.TryGetId(_client.Users.Current.GetUserShipId(), out EntityId shipId))
                {
                    return;
                }

                if (!_tractorBeamEmitterService.Query(shipId, (FixVector2)this.CurrentTargetPosition, out EntityId targetId))
                {
                    return;
                }

                _primitiveBatch.BlendState = BlendState.AlphaBlend;
                _primitiveBatch.Begin(_screen.Camera);

                var nodes = this.entitiesDB.QueryEntitiesAndIndex<Node>(targetId.EGID, out uint index);
                var (visibles, _) = this.entitiesDB.QueryEntities<Visible>(targetId.EGID.groupID);

                Matrix transformation = nodes[index].Transformation.XnaMatrix;
                this.TracePaths(in visibles[index], ref transformation);

                _primitiveBatch.End();
            }
            catch
            {

            }

        }

        private void TracePaths(in Visible visible, ref Matrix transformation)
        {
            for (int i = 0; i < visible.Paths.count; i++)
            {
                this.TracePath(in visible.Paths[i], ref transformation);
            }
        }

        private void TracePath(in Shape shape, ref Matrix transformation)
        {
            _primitiveBatch.EnsureCapacity(shape.Vertices.count);

            Color color = Color.Red;

            ref VertexPositionColor v1 = ref _primitiveBatch.NextVertex(out _indexBuffer[0]);
            v1.Color = color;
            Vector3.Transform(ref shape.Vertices[0], ref transformation, out v1.Position);
            v1.Position = _camera.Project(v1.Position);

            for (int i = 1; i < shape.Vertices.count; i++)
            {
                ref VertexPositionColor v2 = ref _primitiveBatch.NextVertex(out _indexBuffer[1]);
                v2.Color = color;
                Vector3.Transform(ref shape.Vertices[i], ref transformation, out v2.Position);
                v2.Position = _camera.Project(v2.Position);

                _primitiveBatch.AddLineIndex(in _indexBuffer[0]);
                _primitiveBatch.AddLineIndex(in _indexBuffer[1]);

                _indexBuffer[0] = _indexBuffer[1];
            }
        }
    }
}

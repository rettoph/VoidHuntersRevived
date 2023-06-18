using Guppy.Attributes;
using Guppy.MonoGame.Primitives;
using Guppy.MonoGame.Utilities.Cameras;
using Guppy.Resources.Providers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Svelto.DataStructures;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Physics;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Engines;
using VoidHuntersRevived.Domain.Common.Components;
using VoidHuntersRevived.Game.Pieces;
using VoidHuntersRevived.Game.Pieces.Properties;
using VoidHuntersRevived.Game.Pieces.Utilities;

namespace VoidHuntersRevived.Game.Client.Engines
{
    [AutoLoad]
    internal sealed class VisibleNodesEngine : BasicEngine, IStepEngine<GameTime>
    {
        private readonly Camera2D _camera;
        private readonly PrimitiveBatch<VertexPositionColor> _primitiveBatch;
        private readonly IResourceProvider _resources;
        private readonly Color _tint;
        private readonly Dictionary<int, VisibleRenderer> _renderers;

        public string name { get; } = nameof(VisibleNodesEngine);

        public VisibleNodesEngine(Camera2D camera, PrimitiveBatch<VertexPositionColor> primitiveBatch, IResourceProvider resources)
        {
            _camera = camera;
            _primitiveBatch = primitiveBatch;
            _resources = resources;
            _renderers = new Dictionary<int, VisibleRenderer>();
            _tint = Color.Wheat;
        }

        public override void Initialize(ISimulation simulation)
        {
            base.Initialize(simulation);

            foreach((int id, Visible visible) in this.Simulation.Entities.GetProperties<Visible>())
            {
                _renderers.Add(id, new VisibleRenderer(_camera, _primitiveBatch, _resources, visible, _tint));
            }
        }

        public void Step(in GameTime _param)
        {
            _primitiveBatch.Begin(_camera);
            LocalFasterReadOnlyList<ExclusiveGroupStruct> groups = this.entitiesDB.FindGroups<Property<Visible>, Node>();
            foreach (var ((visibleIds, nodes, count), _) in this.entitiesDB.QueryEntities<Property<Visible>, Node>(groups))
            {
                for (int i = 0; i < count; i++)
                {
                    _renderers[visibleIds[i].Id].RenderShapes(nodes[i].Transformation.XnaMatrix);
                }
            }
            _primitiveBatch.End();
        }
    }
}

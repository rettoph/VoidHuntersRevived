using Guppy.Attributes;
using Guppy.GUI;
using Guppy.MonoGame.Primitives;
using Guppy.MonoGame.Utilities.Cameras;
using Guppy.Resources;
using Guppy.Resources.Providers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Serilog;
using Svelto.DataStructures;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Physics;
using VoidHuntersRevived.Common.Pieces;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Attributes;
using VoidHuntersRevived.Common.Simulations.Engines;

namespace VoidHuntersRevived.Game.Client.Engines
{
    [AutoLoad]
    //[SimulationTypeFilter(SimulationType.Predictive)]
    internal sealed class VisibleNodesEngine : BasicEngine, IStepEngine<GameTime>
    {
        private readonly short[] _indexBuffer;
        private readonly IScreen _screen;
        private readonly Camera2D _camera;
        private readonly PrimitiveBatch<VertexPositionColor> _primitiveBatch;
        private readonly IResourceProvider _resources;
        private readonly ILogger _logger;

        public string name { get; } = nameof(VisibleNodesEngine);

        public VisibleNodesEngine(IScreen screen, ILogger logger, Camera2D camera, PrimitiveBatch<VertexPositionColor> primitiveBatch, IResourceProvider resources)
        {
            _screen = screen;
            _camera = camera;
            _primitiveBatch = primitiveBatch;
            _resources = resources;
            _indexBuffer = new short[3];
            _logger = logger;
        }

        public override void Initialize(ISimulation simulation)
        {
            base.Initialize(simulation);
        }

        public void Step(in GameTime _param)
        {
            var groups = this.entitiesDB.FindGroups<Visible, Node>();

            _primitiveBatch.Begin(_camera);
            foreach (var ((vhids, visibles, nodes, count), _) in this.entitiesDB.QueryEntities<EntityId, Visible, Node>(groups))
            {
                for (int i = 0; i < count; i++)
                {
                    try
                    {
                        Matrix transformation = nodes[i].Transformation.XnaMatrix;
                        this.FillShapes(in visibles[i], ref transformation);
                    }
                    catch(Exception e)
                    {
                        _logger.Error(e, "{ClassName}::{MethodName} - Exception attempting to fill shapes for visible {VisibleVhId}", nameof(VisibleNodesEngine), nameof(Step), vhids[i].VhId.Value);
                    }
                }
            }
            _primitiveBatch.End();

            _primitiveBatch.Begin(_screen.Camera);
            foreach (var ((vhids, visibles, nodes, count), _) in this.entitiesDB.QueryEntities<EntityId, Visible, Node>(groups))
            {
                for (int i = 0; i < count; i++)
                {
                    try
                    {
                        Matrix transformation = nodes[i].Transformation.XnaMatrix;
                        this.TracePaths(in visibles[i], ref transformation);
                    }
                    catch (Exception e)
                    {
                        _logger.Error(e, "{ClassName}::{MethodName} - Exception attempting to trace paths for visible {VisibleVhId}", nameof(VisibleNodesEngine), nameof(Step), vhids[i].VhId.Value);
                    }
                }
            }
            _primitiveBatch.End();
        }

        private void FillShapes(in Visible visible, ref Matrix transformation)
        {
            for(int i=0; i<visible.Shapes.count; i++)
            {
                this.FillShape(in visible.Shapes[i], ref transformation);
            }
        }

        private void FillShape(in Shape shape, ref Matrix transformation)
        {
            _primitiveBatch.EnsureCapacity(shape.Vertices.count);

            Color color = _resources.Get(shape.Color);

            ref VertexPositionColor v1 = ref _primitiveBatch.NextVertex(out _indexBuffer[0]);
            v1.Color = color;
            Vector3.Transform(ref shape.Vertices[0], ref transformation, out v1.Position);

            ref VertexPositionColor v2 = ref _primitiveBatch.NextVertex(out _indexBuffer[1]);
            v2.Color = color;
            Vector3.Transform(ref shape.Vertices[1], ref transformation, out v2.Position);
            

            for (int i=2; i<shape.Vertices.count; i++)
            {
                ref VertexPositionColor v3 = ref _primitiveBatch.NextVertex(out _indexBuffer[2]);
                v3.Color = color;
                Vector3.Transform(ref shape.Vertices[i], ref transformation, out v3.Position);

                _primitiveBatch.AddTriangleIndex(in _indexBuffer[0]);
                _primitiveBatch.AddTriangleIndex(in _indexBuffer[1]);
                _primitiveBatch.AddTriangleIndex(in _indexBuffer[2]);

                _indexBuffer[1] = _indexBuffer[2];
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

            Color color = _resources.Get(shape.Color);

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

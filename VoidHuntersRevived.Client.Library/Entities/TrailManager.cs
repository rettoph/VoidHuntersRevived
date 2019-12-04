using Guppy;
using Guppy.Extensions.Collection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Client.Library.Utilities.Cameras;
using VoidHuntersRevived.Library.Entities.ShipParts.Thrusters;

namespace VoidHuntersRevived.Client.Library.Entities
{
    /// <summary>
    /// Simple class used to render all thruster trails
    /// </summary>
    public sealed class TrailManager : Entity
    {
        #region Internal Structs & Classes
        private class TrailSegment
        {
            private static Vector3 Speed { get; set; } = Vector3.UnitX * 0.0025f;

            public Double Age { get; private set; }
            public Vector3 Port;
            public Vector3 Starboard;
            public Single Direction;

            /// <summary>
            /// Step the trail's reference points forward
            /// </summary>
            /// <param name="gameTime"></param>
            public void Step(GameTime gameTime)
            {
                this.Age += gameTime.ElapsedGameTime.TotalMilliseconds;

                this.Port += Vector3.Transform(TrailSegment.Speed, Matrix.CreateRotationZ(this.Direction + MathHelper.PiOver2));
                this.Starboard += Vector3.Transform(TrailSegment.Speed, Matrix.CreateRotationZ(this.Direction - MathHelper.PiOver2));
            }
        }

        private class Trail
        {
            public Guid ThrusterId { get; set; }
            private List<TrailSegment> _segments = new List<TrailSegment>();

            /// <summary>
            /// Automatically add a new segment to the trail
            /// </summary>
            /// <param name="thruster"></param>
            public void AddSegment(Thruster thruster)
            {
                _segments.Add(new TrailSegment()
                {
                    Direction = thruster.Rotation,
                    Port = new Vector3(thruster.Position, 0),
                    Starboard = new Vector3(thruster.Position, 0)
                });
            }

            /// <summary>
            /// Add all trail vertices (if there are any)
            /// This will automatically step all segments
            /// forward by one
            /// </summary>
            /// <param name="vertices"></param>
            public void AddVertices(List<VertexPositionColor> vertices, GameTime gameTime)
            {
                if (_segments.Any())
                {
                    _segments[0].Step(gameTime);
                    var count = _segments.Count;

                    while(count > 500 || (_segments.Any() && _segments[0].Age > 2000))
                    {
                        _segments.RemoveAt(0);
                        count--;
                    }

                    if (count > 1)
                    { // Only proceed if there are more than one pair of segments in the trail...
                        var baseColor = new Color(1, 142, 238, 255);

                        for (Int32 i = 1; i < count; i++)
                        { // Starting from the second segment...
                            // Step segment
                            _segments[i].Step(gameTime);

                            var nC = Color.Lerp(baseColor, Color.Transparent, ((Single)_segments[i].Age / 2000f));
                            var oC = Color.Lerp(baseColor, Color.Transparent, ((Single)_segments[i - 1].Age / 2000f));

                            // Add First triangle...
                            vertices.Add(new VertexPositionColor(_segments[i].Port, nC));
                            vertices.Add(new VertexPositionColor(_segments[i].Starboard, nC));
                            vertices.Add(new VertexPositionColor(_segments[i - 1].Starboard, oC));

                            // Add Second triagle...
                            vertices.Add(new VertexPositionColor(_segments[i - 1].Starboard, oC));
                            vertices.Add(new VertexPositionColor(_segments[i - 1].Port, oC));
                            vertices.Add(new VertexPositionColor(_segments[i].Port, nC));
                        }
                    }
                }
            }
        }
        #endregion

        #region Private Fields
        /// <summary>
        /// Dictionary containing all trail segments for specific
        /// thrusters.
        /// </summary>
        private Dictionary<Guid, Trail> _trails;

        private GraphicsDevice _graphics;
        private FarseerCamera2D _camera;
        private BasicEffect _effect;
        #endregion

        #region Constructor
        public TrailManager(FarseerCamera2D camera, BasicEffect effect, GraphicsDevice graphics)
        {
            _graphics = graphics;
            _effect = effect;
            _camera = camera;
        }
        #endregion

        #region Lifecycle Methods
        protected override void Initialize()
        {
            base.Initialize();

            _trails = new Dictionary<Guid, Trail>();

            _effect.VertexColorEnabled = true;

            this.SetDrawOrder(0);
            this.SetLayerDepth(1);
        }
        #endregion

        #region Frame Methods
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            // Create a new buffer for all trail primitives
            var vertices = new List<VertexPositionColor>();
            
            // Populate all the trail primitives
            _trails.Values.ForEach(trail => trail.AddVertices(vertices, gameTime));

            if (vertices.Any())
            {
                var vertexBuffer = new VertexBuffer(_graphics, typeof(VertexPositionColor), vertices.Count, BufferUsage.WriteOnly);
                vertexBuffer.SetData<VertexPositionColor>(vertices.ToArray());
            
                _graphics.SetVertexBuffer(vertexBuffer);
            
                RasterizerState rasterizerState = new RasterizerState();
                rasterizerState.CullMode = CullMode.None;
                _graphics.RasterizerState = rasterizerState;
                _effect.Projection = _camera.Projection;
                _effect.View = _camera.View;
                _effect.World = _camera.World;
            
                foreach (EffectPass pass in _effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    _graphics.DrawPrimitives(PrimitiveType.TriangleList, 0, vertices.Count / 3);
                }
            }
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Add the submited thruster's current position
        /// into the trail list.
        /// </summary>
        /// <param name="thruster"></param>
        public void AddSegment(Thruster thruster)
        {
            // Add new trails
            if (!_trails.ContainsKey(thruster.Id))
                _trails[thruster.Id] = new Trail() {
                    ThrusterId = thruster.Id
                };


            _trails[thruster.Id].AddSegment(thruster);
        }
        #endregion
    }
}

using Guppy;
using Guppy.Extensions.Collection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Client.Library.Utilities.Cameras;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.ShipParts.Thrusters;

namespace VoidHuntersRevived.Client.Library.Entities
{
    /// <summary>
    /// Simple class used to render all thruster trails
    /// </summary>
    public sealed class TrailManager : Entity
    {
        #region Internal Classes
        /// <summary>
        /// A single set of vertices within a trail.
        /// </summary>
        private class TrailSegment
        {
            private static Vector3 Speed = Vector3.UnitX * 0.001f;

            private readonly Vector3 _portDelta;
            private readonly Vector3 _starboardDelta;

            public Vector3 Port;
            public Vector3 Starboard;
            public Double Age { get; private set; }
            public readonly Single Direction;
            public readonly Single Strength;

            public VertexPositionColor[] Vertices;

            public TrailSegment(Single direction, Single strength)
            {
                this.Direction = direction;
                this.Strength = strength;
                this.Age = 1;
                this.Port = Vector3.Zero;
                this.Starboard = Vector3.Zero;
                this.Vertices = new VertexPositionColor[6] { 
                    new VertexPositionColor(),
                    new VertexPositionColor(),
                    new VertexPositionColor(),
                    new VertexPositionColor(),
                    new VertexPositionColor(),
                    new VertexPositionColor()
                };


                _portDelta = Vector3.Transform(TrailSegment.Speed, Matrix.CreateRotationZ(this.Direction + MathHelper.Pi - (MathHelper.Pi / 3)));
                _starboardDelta = Vector3.Transform(TrailSegment.Speed, Matrix.CreateRotationZ(this.Direction + MathHelper.Pi + (MathHelper.Pi / 3)));
            }

            /// <summary>
            /// Step the trail's reference points forward
            /// </summary>
            /// <param name="gameTime"></param>
            public void Step(GameTime gameTime)
            {
                this.Age = this.Age + gameTime.ElapsedGameTime.TotalMilliseconds;

                this.Port += _portDelta;
                this.Starboard += _starboardDelta;
            }
        }

        /// <summary>
        /// Represents a collection of trail segments
        /// </summary>
        public class Trail
        {
            public static Double Interval = 96;

            private List<TrailSegment> _segments;
            private Double _interval;
            private Thruster _thruster;

            public Trail(Thruster thruster)
            {
                _thruster = thruster;
                _segments = new List<TrailSegment>();
                _interval = Trail.Interval;
            }

            /// <summary>
            /// Automatically add a new segment to the trail
            /// </summary>
            /// <param name="thruster"></param>
            public void AddSegment(Thruster thruster, Single strength, GameTime gameTime)
            {
                _interval += gameTime.ElapsedGameTime.TotalMilliseconds;

                if(_interval >= Trail.Interval)
                {
                    _segments.Add(new TrailSegment(thruster.Rotation, strength)
                    {
                        Port = new Vector3(thruster.Position, 0) + Vector3.Transform(Vector3.UnitX * 0.25f, Matrix.CreateRotationZ(thruster.Rotation + MathHelper.PiOver2)),
                        Starboard = new Vector3(thruster.Position, 0) + Vector3.Transform(Vector3.UnitX * 0.25f, Matrix.CreateRotationZ(thruster.Rotation - MathHelper.PiOver2)),
                    });

                    _interval %= Trail.Interval;
                }
            }

            /// <summary>
            /// Add all trail vertices (if there are any)
            /// This will automatically step all segments
            /// forward by one
            /// </summary>
            /// <param name="vertices"></param>
            public Boolean AddVertices(List<VertexPositionColor> vertices, FarseerCamera2D camera, GameTime gameTime)
            {
                if (_segments.Any() && _thruster.Root.Ship != default(Ship))
                { // Only render the trail if the thruster is attached to a ship
                    var count = _segments.Count;

                    while (count > 5000 || (_segments.Any() && _segments[0].Age > 2000))
                    {
                        _segments.RemoveAt(0);
                        count--;
                    }

                    if (count > 0)
                    { // Only proceed if there are more than one pair of segments in the trail...
                        TrailSegment cur;
                        TrailSegment last = new TrailSegment(_thruster.Rotation, _segments.Last().Strength)
                        {
                            Port = new Vector3(_thruster.Position, 0) + Vector3.Transform(Vector3.UnitX * 0.25f, Matrix.CreateRotationZ(_thruster.Rotation + MathHelper.PiOver2)),
                            Starboard = new Vector3(_thruster.Position, 0) + Vector3.Transform(Vector3.UnitX * 0.25f, Matrix.CreateRotationZ(_thruster.Rotation - MathHelper.PiOver2)),
                        };
                        Color curColor;
                        Color lastColor = Color.Lerp(Color.Transparent, _thruster.Color, last.Strength * (1 - ((Single)last.Age / 2000f)));


                        for (Int32 i = count - 1; i >= 0; i--)
                        { // Starting from the second segment...
                            cur = _segments[i];

                            curColor = Color.Lerp(Color.Transparent, _thruster.Color, cur.Strength * (1 - ((Single)cur.Age / 2000f)));

                            // Update First triangle...
                            cur.Vertices[0].Position = cur.Port;
                            cur.Vertices[0].Color    = curColor;
                            cur.Vertices[1].Position = cur.Starboard;
                            cur.Vertices[1].Color    = curColor;
                            cur.Vertices[2].Position = last.Starboard;
                            cur.Vertices[2].Color    = lastColor;


                            // Update Second triagle...
                            cur.Vertices[3].Position = last.Starboard;
                            cur.Vertices[3].Color    = lastColor;
                            cur.Vertices[4].Position = last.Port;
                            cur.Vertices[4].Color    = lastColor;
                            cur.Vertices[5].Position = cur.Port;
                            cur.Vertices[5].Color    = curColor;

                            vertices.AddRange(cur.Vertices);

                            // Update the last segment item
                            last = cur;
                            lastColor = curColor;
                        }
                    }

                    return true;
                }

                return false;
            }

            public void Update(GameTime gameTime)
            {
                _segments.ForEach(s => s.Step(gameTime));
            }
        }
        #endregion

        #region Private Fields
        private HashSet<Trail> _trails;
        private Queue<Trail> _emptyTrails;

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

            _trails = new HashSet<Trail>();
            _emptyTrails = new Queue<Trail>();

            _effect.VertexColorEnabled = true;

            this.SetDrawOrder(0);
            this.SetLayerDepth(1);
        }
        #endregion

        #region Helper Methods
        public Trail CreateTrail(Thruster thruster)
        {
            var trail = new Trail(thruster);
            _trails.Add(trail);

            return trail;
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _trails.ForEach(t => t.Update(gameTime));
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            // Create a new buffer for all trail primitives
            var vertices = new List<VertexPositionColor>();

            // Populate all the trail primitives
            _trails.ForEach(trail =>
            {
                if (!trail.AddVertices(vertices, _camera, gameTime))
                    _emptyTrails.Enqueue(trail);
            });

            while (_emptyTrails.Any()) // Auto remove empty trails...
                _trails.Remove(_emptyTrails.Dequeue());

            if (vertices.Any())
            {
                var vertexBuffer = new VertexBuffer(_graphics, typeof(VertexPositionColor), vertices.Count, BufferUsage.WriteOnly);
                vertexBuffer.SetData<VertexPositionColor>(vertices.ToArray());

                _graphics.SetVertexBuffer(vertexBuffer);

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
    }
}

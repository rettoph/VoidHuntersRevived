using Guppy;
using Guppy.DependencyInjection;
using Guppy.Utilities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Client.Library.Graphics.Effects;
using VoidHuntersRevived.Client.Library.Graphics.Vertices;
using VoidHuntersRevived.Library;
using VoidHuntersRevived.Library.Entities.ShipParts.Thrusters;

namespace VoidHuntersRevived.Client.Library.Services
{
    /// <summary>
    /// Simple service used to manage TrailSegment's.
    /// </summary>
    public class Trail : Entity
    {
        #region Structs
        private struct TrailSegment
        {
            public Double CreatedTimestamp;
            public Double MaxAge;

            public VertexTrail Port;
            public VertexTrail Center;
            public VertexTrail Starboard;
        }
        #endregion

        #region Private Fields
        private Queue<TrailSegment> _segments;
        private TrailSegment _youngestSegment;
        private PrimitiveBatch<VertexTrail, TrailEffect> _primitiveBatch;
        #endregion

        #region Public Properties
        public Thruster Thruster { get; set; }
        #endregion

        #region Lifecycle Methods
        protected override void Initialize(ServiceProvider provider)
        {
            base.Initialize(provider);

            provider.Service(out _primitiveBatch);

            _segments = new Queue<TrailSegment>();

            this.LayerGroup = VHR.LayersContexts.Trail.Group.GetValue();

            this.OnPreUpdate += this.Setup;
        }

        protected override void Release()
        {
            base.Release();

            _primitiveBatch = null;

            _segments.Clear();

            this.OnPreUpdate -= this.Setup;
        }
        #endregion

        #region Frame Methods
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            if (!_segments.Any())
                return;

            TrailSegment older = _segments.First();
            TrailSegment newer;

            foreach(TrailSegment segment in _segments.Skip(1))
            {
                newer = segment;

                Trail.DrawPair(_primitiveBatch, ref older, ref newer);

                older = newer;
            }

            Trail.DrawPair(_primitiveBatch, ref older, ref _youngestSegment);
        }

        private void Setup(GameTime gameTime)
        {
            // Create the initial segment...
            if(!_segments.Any())
                this.TryAddSegment(gameTime);

            this.OnPreUpdate -= this.Setup;
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (_segments.Any())
            {
                // Refresh the youngest vertex...
                if (this.Thruster != default)
                    _youngestSegment = this.BuildSegment(gameTime);

                // Check the age of the oldest segment & remove it as needed...
                var oldest = _segments.First();
                while (gameTime.TotalGameTime.TotalSeconds - oldest.CreatedTimestamp > oldest.MaxAge)
                {
                    _segments.Dequeue();

                    if(_segments.Any())
                        oldest = _segments.First();
                    else
                    {
                        this.TryRelease();
                        break;
                    }
                } 
            }
            else
                this.TryRelease();
        }
        #endregion

        #region API Methods
        /// <summary>
        /// Create a new segment within the current trail & add it to the segments queue
        /// </summary>
        /// <param name="gameTime"></param>
        public void TryAddSegment(GameTime gameTime)
        {
            if (this.Thruster == default)
                return;

            _segments.Enqueue(this.BuildSegment(gameTime));
        }

        /// <summary>
        /// Create a new segment within the current trail.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <returns></returns>
        private TrailSegment BuildSegment(GameTime gameTime)
        {
            var currentTimestamp = (Single)gameTime.TotalGameTime.TotalSeconds;
            var maxAge = 3f;
            var spreadSpeed = 5f;
            var color = Color.Lerp(Color.Transparent, this.Thruster.Color, this.Thruster.ImpulseModifier * 0.5f);

            return new TrailSegment()
            {
                CreatedTimestamp = gameTime.TotalGameTime.TotalSeconds,
                MaxAge = maxAge,
                Port = new VertexTrail()
                {
                    Position = this.Thruster.Position,
                    Velocity = -this.Thruster.LinearVelocity,
                    Color = color,
                    SpreadDirection = this.Thruster.Rotation + MathHelper.PiOver2,
                    SpreadSpeed = spreadSpeed,
                    CreatedTimestamp = currentTimestamp,
                    MaxAge = maxAge,
                    Center = false
                },
                Center = new VertexTrail()
                {
                    Position = this.Thruster.Position,
                    Velocity = -this.Thruster.LinearVelocity,
                    Color = color,
                    SpreadDirection = this.Thruster.Rotation,
                    SpreadSpeed = spreadSpeed,
                    CreatedTimestamp = currentTimestamp,
                    MaxAge = maxAge,
                    Center = true
                },
                Starboard = new VertexTrail()
                {
                    Position = this.Thruster.Position,
                    Velocity = -this.Thruster.LinearVelocity,
                    Color = color,
                    SpreadDirection = this.Thruster.Rotation - MathHelper.PiOver2,
                    SpreadSpeed = spreadSpeed,
                    CreatedTimestamp = currentTimestamp,
                    MaxAge = maxAge,
                    Center = false
                }
            };
        }

        private static void DrawPair(PrimitiveBatch<VertexTrail, TrailEffect> primitiveBatch, ref TrailSegment older, ref TrailSegment newer)
        {
            primitiveBatch.DrawTriangle(ref newer.Center, ref newer.Port, ref older.Port);
            primitiveBatch.DrawTriangle(ref older.Center, ref newer.Center, ref older.Port);

            primitiveBatch.DrawTriangle(ref newer.Center, ref older.Starboard, ref newer.Starboard);
            primitiveBatch.DrawTriangle(ref newer.Center, ref older.Center, ref older.Starboard);
        }
        #endregion
    }
}

using FarseerPhysics.Common;
using Guppy;
using Guppy.Extensions.Collection;
using Guppy.UI.Utilities;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Client.Library.Utilities.Cameras;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.ShipParts.Thrusters;
using VoidHuntersRevived.Library.Utilities;

namespace VoidHuntersRevived.Client.Library.Entities
{
    /// <summary>
    /// Simple class used to render all thruster trails
    /// </summary>
    public sealed class TrailManager : Entity
    {
        #region Private Fields
        private BasicEffect _effect;
        private FarseerCamera2D _camera;
        private Dictionary<Thruster, Trail> _trails;
        private Queue<Trail> _empty;
        private PrimitiveBatch _primitives;
        #endregion

        #region Constructor
        public TrailManager(BasicEffect effect, PrimitiveBatch primitives, FarseerCamera2D camera)
        {
            _effect = effect;
            _camera = camera;
            _primitives = primitives;
        }
        #endregion
    
        #region Lifecycle Methods
        protected override void Create(IServiceProvider provider)
        {
            base.Create(provider);

            _trails = new Dictionary<Thruster, Trail>();
            _empty = new Queue<Trail>();
            _effect.VertexColorEnabled = true;

            this.LayerDepth = 2;
        }
        #endregion
    
        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
    
            _trails.Values.ForEach(t =>
            {
                t.Update(gameTime);
            
                // Add the trail to the empty queue so it will be auto removed...
                if (!t.HasSegments)
                    _empty.Enqueue(t);
            });
            
            while (_empty.Any())
                this.RemoveTrail(_empty.Dequeue());
        }
    
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            _effect.Projection = _camera.Projection;
            _effect.View = _camera.View;
            _effect.World = _camera.World;

            // Draw all internal trails..
            _trails.Values.ForEach(t => t.Draw(gameTime));
        }
        #endregion
    
        #region Helper Methods
        /// <summary>
        /// Auto enqueue a pair of vertices within 2 trail
        /// segments into the vertice buffer.
        /// 
        /// This will draw the vertices if the buffer is overflowed.
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        internal void EnqueueSegmentVertices(TrailSegment s1, TrailSegment s2)
        {
            _primitives.DrawTriangle(s1.Port, s1.Color, s1.Starboard, s1.Color, s2.Starboard, s2.Color);
            _primitives.DrawTriangle(s2.Starboard, s2.Color, s2.Port, s2.Color, s1.Port, s1.Color);
        }
    
        /// <summary>
        /// Create a new trail for the requested thruster
        /// if it is currently active & doesnt already have
        /// an active trail...
        /// </summary>
        /// <param name="thruster"></param>
        public void TryAddTrail(Thruster thruster)
        {
            if(!_trails.ContainsKey(thruster) && thruster.Strength > 0.001f)
            { // Create a new trail for the thruster...
                var trail = Trail.Build(this, thruster);
                _trails.Add(thruster, trail);
            }
        }
    
        private void RemoveTrail(Trail trail)
        {
            _trails.Remove(trail.Thruster);
            trail.Dispose();
        }
        #endregion
    }
    
    /// <summary>
    /// Class containing a list of trail segments.
    /// Each segment represents a single point of
    /// thrust the thruster has passed.
    /// </summary>
    internal sealed class Trail : IDisposable
    {
        #region Static Fields
        public static Double SegmentInterval { get; private set; } = 64;
        private static Queue<Trail> Queue = new Queue<Trail>();
        #endregion
    
        #region Private Fields
        private ActionTimer _segmentTimer;
        private List<TrailSegment> _segments;
        private TrailManager _manager;
        private Queue<TrailSegment> _empty;
        private TrailSegment _live;
        #endregion
    
        #region Public Attributes
        public Color Color { get; private set; }
        public Color BaseColor { get; private set; }
        public Thruster Thruster { get; private set; }
        public Boolean HasSegments { get => _segments.Any(); }
        #endregion
    
        #region Constructor
        public Trail()
        {
            _empty = new Queue<TrailSegment>();
            _segments = new List<TrailSegment>();
            _segmentTimer = new ActionTimer(Trail.SegmentInterval);
        }
        #endregion
    
        #region Lifecycle Methods
        public Trail Initialize(TrailManager manager, Thruster thruster)
        {
            this.Thruster = thruster;
            this.Color = this.Thruster.Color;
    
            _manager = manager;
            _live = TrailSegment.Build(this);
    
            // Create a single vertice by default
            this.AddSegment();
    
            return this;
        }
        public void Dispose()
        {
            _live.Dispose();
    
            while (_segments.Any())
                this.RemoveSegment(_segments[0]);
    
            // Return the current trail back into the queue...
            Trail.Queue.Enqueue(this);
        }
        #endregion
    
        #region Frame Methods
        public void Update(GameTime gameTime)
        {
            // Update the internal base color value
            this.BaseColor = new Color(Color.Lerp(Color.Red, this.Color, this.Thruster.HealthRate), 150);

            _segmentTimer.Update(
                gameTime: gameTime,
                filter: triggered => this.Thruster.Strength > 0.001f && triggered,
                action: this.AddSegment);
    
            _segments.ForEach(s =>
            {
                s.Update(gameTime);
    
                // Auto remove the segment when it becomes too old
                if (s.Age > TrailSegment.MaxAge)
                    _empty.Enqueue(s);
            });
    
            while (_empty.Any())
                this.RemoveSegment(_empty.Dequeue());
        }
    
        public void Draw(GameTime gameTime)
        {
            for (Int32 i = 1; i < _segments.Count; i++)
                _manager.EnqueueSegmentVertices(_segments[i - 1], _segments[i]);
    
            _live.Refresh();
            _manager.EnqueueSegmentVertices(_segments.Last(), _live);
        }
        #endregion
    
        #region Helper Methods
        private void AddSegment()
        { // Create a new trail segment instance...
            _segments.Add(TrailSegment.Build(this));
        }
        private void RemoveSegment(TrailSegment segment)
        {
            segment.Dispose();
            _segments.Remove(segment);
        }
        #endregion
    
        #region Static Methods
        public static Trail Build(TrailManager manager, Thruster thruster)
        {
            return (Trail.Queue.Any() ? Trail.Queue.Dequeue() : new Trail()).Initialize(manager, thruster);
        }
        #endregion
    }
    
    /// <summary>
    /// An independent pair of vertices
    /// represting a trail exhaust and move
    /// tangent to the original thruster's
    /// direction.
    /// </summary>
    internal sealed class TrailSegment : IDisposable
    {
        #region Static Fields
        private static Queue<TrailSegment> Queue = new Queue<TrailSegment>();
        public static Double MaxAge { get; private set; } = 2000;
        private static Vector3 Spread = Vector3.UnitX * 0.25f;
        private static Vector3 Speed = Vector3.UnitX * 0.001f;
        #endregion
    
        #region Private Fields
        private Trail _trail;
        private Vector3 _tangentDelta;
        #endregion
    
        #region Public Attributes
        public Double Age { get; private set; }
        public Color Color;
        public Vector3 Port;
        public Vector3 Starboard;
        public Single Direction;
        public Single Strength;
        #endregion
    
        #region Lifecycle Methods
        public TrailSegment Initialize(Trail trail)
        {
            _trail = trail;
            this.Refresh();
    
            return this;
        }
    
        public void Refresh()
        {
            this.Age = 0;
            this.Direction = _trail.Thruster.Rotation;
            this.Strength = _trail.Thruster.Strength;
            this.Port = new Vector3(_trail.Thruster.Position, 0) + Vector3.Transform(TrailSegment.Spread, Matrix.CreateRotationZ(this.Direction + MathHelper.PiOver2));
            this.Starboard = Starboard = new Vector3(_trail.Thruster.Position, 0) + Vector3.Transform(TrailSegment.Spread, Matrix.CreateRotationZ(this.Direction - MathHelper.PiOver2));
            this.Color = Color.Lerp(Color.Transparent, _trail.BaseColor, this.Strength * (1 - ((Single)(this.Age / TrailSegment.MaxAge))));
            _tangentDelta = Vector3.Transform(TrailSegment.Speed, Matrix.CreateRotationZ(this.Direction + MathHelper.Pi - (MathHelper.Pi / 2)));
        }
    
        public void Dispose()
        {
            TrailSegment.Queue.Enqueue(this);
        }
        #endregion
    
        #region Frame Methods
        public void Update(GameTime gameTime)
        {
            this.Age += gameTime.ElapsedGameTime.TotalMilliseconds;
            this.Port += _tangentDelta;
            this.Starboard -= _tangentDelta;
            this.Color = Color.Lerp(Color.Transparent, _trail.BaseColor, this.Strength * (1 - ((Single)(this.Age / TrailSegment.MaxAge))));
        }
        #endregion
    
        #region Static Methods
        public static TrailSegment Build(Trail trail)
        {
            return (TrailSegment.Queue.Any() ? TrailSegment.Queue.Dequeue() : new TrailSegment()).Initialize(trail);
        }
        #endregion
    }
}

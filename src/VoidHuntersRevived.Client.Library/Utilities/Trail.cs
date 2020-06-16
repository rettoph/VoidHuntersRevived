using Guppy;
using Guppy.DependencyInjection;
using Guppy.Extensions.Collections;
using Guppy.Interfaces;
using Guppy.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Client.Library.Entities;
using VoidHuntersRevived.Library.Entities.ShipParts.Thrusters;
using VoidHuntersRevived.Library.Utilities;

namespace VoidHuntersRevived.Client.Library.Utilities
{
    /// <summary>
    /// Class containing a list of trail segments.
    /// Each segment represents a single point of 
    /// thrust the thruster passed.
    /// </summary>
    internal sealed class Trail : Service
    {
        #region Static Fields
        public static Double SegmentInterval { get; private set; } = 64;
        #endregion

        #region Private Fields
        private TrailManager _manager;
        private ServiceProvider _provider;
        private Queue<TrailSegment> _segments;
        private Int32 _expired;
        private ActionTimer _segmentTimer;
        private PrimitiveBatch _primitiveBatch;
        private TrailSegment _live;
        #endregion

        #region Internal Fields
        internal TrailManager manager => _manager;
        internal Thruster thruster { get; set; }
        #endregion

        #region Events
        public event GuppyEventHandler<Trail> OnDirty;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            _provider = provider;
            provider.Service(out _manager);
            provider.Service(out _primitiveBatch);

            _segmentTimer = new ActionTimer(Trail.SegmentInterval);
            _segments = new Queue<TrailSegment>();
        }

        protected override void PostInitialize(ServiceProvider provider)
        {
            base.PostInitialize(provider);

            _live = _provider.GetService<TrailSegment>().Initialize(this);
        }
        #endregion

        #region Frame Methods
        public void Update(GameTime gameTime)
        {
            _segmentTimer.Update(
                gameTime: gameTime,
                filter: t => t && this.thruster.Strength > 0.01f,
                action: () => _segments.Enqueue(_provider.GetService<TrailSegment>().Initialize(this)));

            _expired = 0;
            _segments.ForEach(s =>
            {
                s.Update(gameTime);

                if (s.Age > 2000)
                    _expired++;
            });

            if(_expired > 0)
                for(var i=0; i< _expired; i++)
                    _segments.Dequeue().Dispose();

            if(_segments.Any())
                this.OnDirty.Invoke(this);
        }

        public void Draw(GameTime gameTime)
        {
            TrailSegment _last, _cur = default(TrailSegment);
            Int32 i = 0;

            foreach (TrailSegment segment in _segments)
            {
                _last = _cur;
                _cur = segment;

                if (i > 0)
                    this.DrawSegments(_last, _cur);

                i++;
            }

            _live.Refresh();
            _live.Update(gameTime);
            this.DrawSegments(_cur, _live);
        }

        private void DrawSegments(TrailSegment s1, TrailSegment s2)
        {
            _primitiveBatch.DrawTriangle(s1.Port, s1.Color, s1.Starboard, s1.Color, s2.Starboard, s2.Color);
            _primitiveBatch.DrawTriangle(s2.Starboard, s2.Color, s2.Port, s2.Color, s1.Port, s1.Color);
        }
        #endregion
    }
}

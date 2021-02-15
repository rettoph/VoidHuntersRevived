using Guppy;
using Guppy.DependencyInjection;
using Guppy.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Client.Library.Services;
using VoidHuntersRevived.Library.Contexts;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Entities.ShipParts.Special;
using Guppy.Extensions.Microsoft.Xna.Framework;
using Guppy.Extensions.Utilities;

namespace VoidHuntersRevived.Client.Library.Drivers.Entities.ShipParts.Special
{
    internal sealed class ShieldGeneratorGraphicsDriver : Driver<ShieldGenerator>
    {
        #region Private Structs
        private struct StrandData
        {
            public Single Direction;
            public Single Value;
            public Single Speed;
        }
        #endregion

        #region Private Fields
        private PrimitiveBatch<VertexPositionColor> _primitiveBatch;

        private Single[] _localSegments;
        private StrandData[] _strands;
        #endregion

        #region Lifecycle Methods
        protected override void Initialize(ShieldGenerator driven, ServiceProvider provider)
        {
            base.Initialize(driven, provider);

            provider.Service(out _primitiveBatch);

            _localSegments = new Single[10];
            for(var i=0; i< _localSegments.Length; i++)
                _localSegments[i] = (this.driven.Context.Range / -2) + ((this.driven.Context.Range / (_localSegments.Length - 1)) * i);

            var rand = new Random();
            _strands = new StrandData[10];
            for (var i = 0; i < _strands.Length; i++)
                _strands[i] = new StrandData()
                {
                    Direction = rand.Next(1) == 1 ? -1 : 1,
                    Speed = 0.1f + ((Single)rand.NextDouble() * 0.1f),
                    Value = (Single)rand.NextDouble()
                };

            this.driven.OnPreDraw += this.Draw;
        }

        protected override void Release(ShieldGenerator driven)
        {
            base.Release(driven);

            _primitiveBatch = null;

            this.driven.OnPreDraw -= this.Draw;
        }
        #endregion

        #region Frame Methods
        private void Draw(GameTime gameTime)
        {
            if (this.driven?.Chain.Ship == default)
                return;

            var live = this.driven.Rotation + MathHelper.Pi;
            
            for (var i = 1; i < _strands.Length; i++)
            {
                _primitiveBatch.DrawLine(
                    Color.Gray,
                    this.driven.Position,
                    this.driven.Position + this.driven.Context.Radius.ToVector2().RotateTo(live + MathHelper.Lerp(_localSegments[0], _localSegments[9], _strands[i].Value)));

                _strands[i].Value = _strands[i].Value + _strands[i].Direction * _strands[i].Speed * (Single)gameTime.ElapsedGameTime.TotalSeconds;
                if (_strands[i].Value <- 0 || _strands[i].Value >= 1)
                {
                    _strands[i].Value = MathHelper.Clamp(_strands[i].Value, 0, 1);
                    _strands[i].Direction = _strands[i].Direction * -1;
                }
            }

            for (var i=1; i < _localSegments.Length; i++)
            {
                _primitiveBatch.DrawLine(
                    Color.Yellow,
                    this.driven.Position + this.driven.Context.Radius.ToVector2().RotateTo(live + _localSegments[i - 1]),
                    this.driven.Position + this.driven.Context.Radius.ToVector2().RotateTo(live + _localSegments[i]));
            }

            _primitiveBatch.DrawLine(
                Color.LightGray, 
                this.driven.Position, 
                this.driven.Position + this.driven.Context.Radius.ToVector2().RotateTo(live + _localSegments[0]));

            _primitiveBatch.DrawLine(
                Color.LightGray,
                this.driven.Position,
                this.driven.Position + this.driven.Context.Radius.ToVector2().RotateTo(live + _localSegments[9]));
        }
        #endregion
    }
}

using FarseerPhysics;
using GalacticFighters.Client.Library.Scenes;
using GalacticFighters.Library.Entities;
using GalacticFighters.Library.Entities.ShipParts;
using Guppy;
using Guppy.Attributes;
using Guppy.Collections;
using Guppy.Loaders;
using Guppy.Network.Extensions.Lidgren;
using Guppy.Utilities.Cameras;
using Lidgren.Network;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GalacticFighters.Client.Library.Drivers.Entities
{
    [IsDriver(typeof(Ship))]
    public class ClientShipDriver : Driver<Ship>
    {
        #region Private Fields
        private EntityCollection _entities;
        private SpriteBatch _spriteBatch;
        private Texture2D _com;
        #endregion

        #region Constructor
        public ClientShipDriver(SpriteBatch spriteBatch, ContentLoader content, EntityCollection entities, Ship driven) : base(driven)
        {
            _spriteBatch = spriteBatch;
            _com = content.TryGet<Texture2D>("com");
            _entities = entities;
        }
        #endregion

        #region Lifecycle Methods
        protected override void Initialize()
        {
            base.Initialize();

            this.driven.Actions.TryAdd("target:changed", this.HandleTargetChanged);
            this.driven.Actions.TryAdd("bridge:changed", this.HandleBridgeChanged);
            this.driven.Actions.TryAdd("direction:changed", this.HandleDirectionChanged);
            this.driven.Actions.TryAdd("firing:changed", this.HandleFiringChanged);
            this.driven.Actions.TryAdd("tractor-beam:selected", this.HandleTractorBeamSelected);
            this.driven.Actions.TryAdd("tractor-beam:released", this.HandleTractorBeamReleased);
            this.driven.Actions.TryAdd("tractor-beam:attached", this.HandleTractorBeamAttached);
        }
        #endregion

        #region Frame Methods
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            if(this.driven.Bridge != default(ShipPart))
            {
                _spriteBatch.Draw(
                    texture: _com,
                    position: this.driven.Bridge.WorldCenter,
                    sourceRectangle: _com.Bounds,
                    color: Color.White,
                    rotation: this.driven.Bridge.Rotation,
                    origin: _com.Bounds.Center.ToVector2(),
                    scale: 0.01f,
                    effects: SpriteEffects.None,
                    layerDepth: 0);
            }
        }
        #endregion

        #region Action Handlers
        private void HandleTargetChanged(object sender, NetIncomingMessage arg)
        {
            this.driven.ReadTargetOffset(arg);
        }

        private void HandleBridgeChanged(object sender, NetIncomingMessage im)
        {
            this.logger.LogInformation($"Reading Bridge data...");

            this.driven.ReadBridge(im);
        }

        private void HandleDirectionChanged(object sender, NetIncomingMessage im)
        {
            this.driven.ReadDirection(im);
        }

        private void HandleFiringChanged(object sender, NetIncomingMessage arg)
        {
            this.driven.SetFiring(arg.ReadBoolean());
            this.driven.ReadTargetOffset(arg);
        }

        private void HandleTractorBeamSelected(object sender, NetIncomingMessage arg)
        {
            this.driven.ReadTargetOffset(arg);
            this.driven.TractorBeam.TrySelect(arg.ReadEntity<ShipPart>(_entities));
        }

        private void HandleTractorBeamReleased(object sender, NetIncomingMessage arg)
        {
            this.driven.ReadTargetOffset(arg);
            this.driven.TractorBeam.TryRelease();
        }

        private void HandleTractorBeamAttached(object sender, NetIncomingMessage arg)
        {
            this.driven.TractorBeam.TryAttach(
                arg.ReadEntity<ShipPart>(_entities).FemaleConnectionNodes[arg.ReadInt32()]);
        }
        #endregion
    }
}

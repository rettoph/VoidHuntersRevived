using Guppy;
using Guppy.Collections;
using Guppy.Configurations;
using Guppy.Network;
using Guppy.Network.Extensions.Lidgren;
using Guppy.Network.Groups;
using Guppy.Network.Security;
using Lidgren.Network;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.Drivers;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Scenes;
using VoidHuntersRevived.Library.Enums;
using System.Linq;

namespace VoidHuntersRevived.Library.Entities.Players
{
    public class Player : NetworkEntity
    {
        private EntityCollection _entities;
        private PlayerDriver _driver;

        public User User { get; private set; }
        public String Name { get { return this.User.Get("name"); } }
        public ShipPart Bridge { get; private set; }

        public Dictionary<Direction, Boolean> Directions { get; private set; }

        public Player(User user, EntityConfiguration configuration, Scene scene, ILogger logger, IServiceProvider provider) : base(configuration, scene, logger)
        {
            _entities = provider.GetRequiredService<EntityCollection>();

            this.User = user;

            this.SetUpdateOrder(100);
        }

        public Player(Guid id, EntityConfiguration configuration, Scene scene, ILogger logger, IServiceProvider provider) : base(id, configuration, scene, logger)
        {
            _entities = provider.GetRequiredService<EntityCollection>();
        }

        #region Initialization Methods
        protected override void Boot()
        {
            base.Boot();

            this.Directions = ((Direction[])Enum.GetValues(typeof(Direction)))
                .ToDictionary(
                    keySelector: d => d,
                    elementSelector: d => false);
        }

        protected override void Initialize()
        {
            _driver = _entities.Create<PlayerDriver>("driver:player", this);

            base.Initialize();
        }
        #endregion

        #region Frame Methods
        public override void Draw(GameTime gameTime)
        {
            _driver.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            if (this.Bridge != null)
            {
                var thrust = Vector2.Transform(new Vector2(0.1f, 0), Matrix.CreateRotationZ(this.Bridge.Body.Rotation));
                if (this.Directions[Direction.Forward])
                    this.Bridge.Body.ApplyLinearImpulse(thrust);
                if (this.Directions[Direction.Backward])
                    this.Bridge.Body.ApplyLinearImpulse(-thrust);
                if (this.Directions[Direction.TurnLeft])
                    this.Bridge.Body.ApplyAngularImpulse(-0.05f);
                if (this.Directions[Direction.TurnRight])
                    this.Bridge.Body.ApplyAngularImpulse(0.05f);
            }

            _driver.Update(gameTime);
        }
        #endregion

        #region Utility Methods
        public void UpdateBridge(ShipPart bridge)
        {
            if (bridge != this.Bridge)
            {
                this.logger.LogDebug($"Updating Player({this.Id}) bridge to {bridge.GetType().Name}({bridge.Id})");

                this.Bridge = bridge;

                _driver.HandleUpdateBridge();
            }
        }

        public void UpdateDirection(Direction direction, Boolean value)
        {
            if (this.Directions[direction] != value)
            {
                this.logger.LogDebug($"Updating Player({this.Id}) Direction[{direction}] to {value}");

                this.Directions[direction] = value;

                _driver.HandleUpdateDirection(direction, value);
            }
        }
        #endregion

        #region Network Methods
        public override void Read(NetIncomingMessage im)
        {
            base.Read(im);

            // Load the player's user by id...
            this.User = (this.scene as VoidHuntersWorldScene).Users.GetById(im.ReadGuid());

            if (im.ReadBoolean())
                this.UpdateBridge(_entities.GetById(im.ReadGuid()) as ShipPart);
        }

        public override void Write(NetOutgoingMessage om)
        {
            base.Write(om);

            // Write the player's user id...
            om.Write(this.User.Id);

            if (this.Bridge == null)
                om.Write(false);
            else
            {
                om.Write(true);
                om.Write(this.Bridge.Id);
            }
        }
        #endregion
    }
}

using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Lidgren.Network;
using Lidgren.Network.Xna;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Core.Structs;
using VoidHuntersRevived.Library.Entities.Interfaces;
using VoidHuntersRevived.Library.Interfaces;
using VoidHuntersRevived.Library.Scenes.Interfaces;
using VoidHuntersRevived.Networking.Implementations;
using VoidHuntersRevived.Networking.Interfaces;

namespace VoidHuntersRevived.Library.Entities
{
    public class NetworkedFarseerEntity : NetworkEntity, IFarseerEntity
    {
        // The default driver type used
        public static Type DefaultDriverType { get; set; }

        public World World { get; private set; }
        public Body Body { get; protected set; }

        public IFarseerEntityDriver Driver { get; protected set; }
        private Type _driverType;

        public NetworkedFarseerEntity(EntityInfo info, IGame game, Type driverType = null) : base(info, game)
        {
            _driverType = driverType ?? NetworkedFarseerEntity.DefaultDriverType;
        }
        public NetworkedFarseerEntity(long id, EntityInfo info, IGame game, Type driverType = null) : base(id, info, game)
        {
            _driverType = driverType ?? NetworkedFarseerEntity.DefaultDriverType;
        }

        protected override void PreInitialize()
        {
            base.PreInitialize();

            if (typeof(IFarseerEntityDriver).IsAssignableFrom(_driverType))
                this.Driver = (IFarseerEntityDriver)ActivatorUtilities.CreateInstance(this.Game.Provider, _driverType, this);
            else
                this.Game.Logger.LogCritical($"Invalid IFarseerEntityDriver type => '{_driverType?.Name}'");
        }

        protected override void Initialize()
        {
            this.World = (this.Scene as IFarseerScene).World;
            this.Body = BodyFactory.CreateBody(world: this.World, userData: this);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            this.Driver.Update(gameTime);
        }

        public override void Read(NetIncomingMessage im)
        {
            if (im.ReadBoolean())
            { // Only read body data if a true byte marker was sent
                this.Driver.Position = im.ReadVector2();
                this.Driver.LinearVelocity = im.ReadVector2();

                this.Driver.Rotation = im.ReadSingle();
                this.Driver.AngularVelocity = im.ReadSingle();
            }
        }

        public override void Write(NetOutgoingMessage om)
        {
            om.Write(this.Id);

            if (this.Body != null && this.Body.Awake)
            { // Only write the body data if it is not null and is awake..
                om.Write(true);

                om.Write(this.Body.Position);
                om.Write(this.Body.LinearVelocity);

                om.Write(this.Body.Rotation);
                om.Write(this.Body.AngularVelocity);
            }
            else
            { // Otherwise just write a false byte
                om.Write(false);
            }
        }
    }
}

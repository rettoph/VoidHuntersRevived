using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;
using Lidgren.Network;
using Lidgren.Network.Xna;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Core.Implementations;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Core.Structs;
using VoidHuntersRevived.Library.Entities.Connections;
using VoidHuntersRevived.Library.Entities.Interfaces;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Interfaces;
using VoidHuntersRevived.Library.Scenes;

namespace VoidHuntersRevived.Library.Entities
{
    public class TractorBeam : FarseerEntity, ITractorBeam
    {
        public TractorBeamConnection Connection { get; private set; }
        public Vector2 Position { get; set; }

        private MainScene _scene;

        public event EventHandler<ITractorBeam> OnConnected;
        public event EventHandler<ITractorBeam> OnDisconnected;

        public TractorBeam(IPlayer player, EntityInfo info, IGame game) : base(info, game)
        {
            this.Connection = null;
        }

        protected override void Initialize()
        {
            base.Initialize();

            _scene = this.Scene as MainScene;

            this.Body.BodyType = FarseerPhysics.Dynamics.BodyType.Dynamic;
            this.Body.SleepingAllowed = false;
        }

        public void CreateConnection(ITractorableEntity target)
        {
            if (this.Connection != null)
                this.Game.Logger.LogCritical("Unable to create TractorBeam Connection, TractorBeam already has an active connection!");
            else if (target.TractorBeamConnection != null)
                this.Game.Logger.LogCritical("Unable to create TractorBeam Connection, Target already has an active connection!");
            else // Create the tractor beam connection
                this.Scene.Entities.Create<TractorBeamConnection>("entity:connection:tractor_beam", null, this, target);
        }
        public void Connect(TractorBeamConnection connection)
        {
            // The connection can only connect if its not already connnected
            if (this.Connection != null)
                throw new Exception("Unable to connect! Already bound to another connection.");
            else if (connection.TractorBeam != this)
                throw new Exception("Unable to connect! Connection already mapped to another tractor beam");
            else if(connection.State != ConnectionState.Connecting)
                throw new Exception("Unable to connect! Current connection status invalid.");

            // Save the connection
            this.Connection = connection;
            this.OnConnected?.Invoke(this, this);
        }
        public void Disconnect()
        {
            if (this.Connection.State != ConnectionState.Disconnecting)
                throw new Exception("Unable to disconnect! Connection state is not set to Disconnected.");

            this.Connection = null;

            this.OnDisconnected?.Invoke(this, this);
        }

        public void Read(NetIncomingMessage im)
        {
            this.Body.Position = im.ReadVector2();

            if (im.ReadBoolean())
            { // The next boolean indicates wether or not the tractor beam has an object selected
                var selectedId = im.ReadInt64();

                if (this.Connection == null)
                { // We only need to do anything if the tractor beam doesnt current have a selection
                    var target = _scene.NetworkEntities.GetById(selectedId);

                    // Only bother trying if the input entity is a tractorable object
                    if (target is ITractorableEntity)
                        this.CreateConnection(target as ITractorableEntity);
                }
            }
            else
            {
                if (this.Connection != null)
                { // try to release the input entity
                    this.Connection.Disconnect();
                }
            }
        }

        public void Write(NetOutgoingMessage om)
        {
            om.Write(this.Body.Position);

            if (this.Connection == null)
            {
                om.Write(false);
            }
            else
            {
                om.Write(true);
                om.Write(this.Connection.Target.Id);
            }
        }
    }
}

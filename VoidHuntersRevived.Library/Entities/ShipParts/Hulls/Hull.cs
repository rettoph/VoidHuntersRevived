using System;
using System.Collections.Generic;
using System.Text;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoidHuntersRevived.Core.Extensions;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Core.Providers;
using VoidHuntersRevived.Core.Structs;
using VoidHuntersRevived.Library.Entities.ConnectionNodes;
using VoidHuntersRevived.Library.Entities.MetaData;
using System.Linq;
using Lidgren.Network;
using Lidgren.Network.Xna;

namespace VoidHuntersRevived.Library.Entities.ShipParts.Hulls
{
    public class Hull : ShipPart
    {
        public readonly HullData HullData;

        public FemaleConnectionNode[] FemaleConnectionNodes { get; private set; }


        public Hull(EntityInfo info, IGame game) : base(info, game)
        {
            this.HullData = this.Info.Data as HullData;
        }
        public Hull(Int64 id, EntityInfo info, IGame game) : base(id, info, game)
        {
            this.HullData = this.Info.Data as HullData;
        }

        protected override void Initialize()
        {
            base.Initialize();

            this.Body.CreateFixture(new PolygonShape(this.HullData.Vertices, 0.1f));

            // Create the female connection nodes
            this.FemaleConnectionNodes = this.HullData.FemaleConnections
                .Select(fcnd =>
                {
                    return this.Scene.Entities.Create<FemaleConnectionNode>("entity:connection_node:female", null, fcnd, this);
                })
                .ToArray();
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

        public override void Read(NetIncomingMessage im)
        {
            this.Body.Position = im.ReadVector2();
            this.Body.Rotation = im.ReadSingle();
            this.Body.LinearVelocity = im.ReadVector2();
            this.Body.AngularVelocity = im.ReadSingle();
        }

        public override void Write(NetOutgoingMessage om)
        {
            om.Write(this.Id);
            om.Write(this.Body.Position);
            om.Write(this.Body.Rotation);
            om.Write(this.Body.LinearVelocity);
            om.Write(this.Body.AngularVelocity);
        }
    }
}

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
using VoidHuntersRevived.Library.Entities.MetaData;
using System.Linq;
using Lidgren.Network;
using Lidgren.Network.Xna;
using VoidHuntersRevived.Library.Interfaces;
using VoidHuntersRevived.Library.Entities.Interfaces;
using VoidHuntersRevived.Library.Entities.Connections.Nodes;

namespace VoidHuntersRevived.Library.Entities.ShipParts.Hulls
{
    public class Hull : ShipPart
    {
        public readonly HullData HullData;

        public FemaleConnectionNode[] FemaleConnectionNodes { get; private set; }
        public IPlayer BridgeFor { get; set; }


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

        public override bool CanBeSelectedBy(ITractorBeam tractorBeam)
        {
            if(base.CanBeSelectedBy(tractorBeam))
            {
                return this.BridgeFor == null;
            }

            return false;
        }

        /// <summary>
        /// Return a list of all available female connection nodes found
        /// within the current hull piece (and any of its children)
        /// </summary>
        /// <returns></returns>
        public List<FemaleConnectionNode> GetAvailabaleFemaleConnectioNodes(List<FemaleConnectionNode> addTo = null)
        {
            if (addTo == null)
                addTo = new List<FemaleConnectionNode>();

            foreach(FemaleConnectionNode female in this.FemaleConnectionNodes)
            {
                if(female.Connection == null)
                    addTo.Add(female);
                else if(female.Connection.MaleNode.Owner is Hull)
                    (female.Connection.MaleNode.Owner as Hull).GetAvailabaleFemaleConnectioNodes(addTo);
            }

            return addTo;
        }
    }
}

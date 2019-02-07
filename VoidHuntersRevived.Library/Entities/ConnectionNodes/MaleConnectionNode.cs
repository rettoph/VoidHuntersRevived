using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Core.Structs;
using VoidHuntersRevived.Library.Entities.Connections;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Extensions;

namespace VoidHuntersRevived.Library.Entities.ConnectionNodes
{
    public class MaleConnectionNode : ConnectionNode
    {
        public MaleConnectionNode(
            ShipPart owner,
            Vector2 connectionData,
            EntityInfo info,
            IGame game,
            SpriteBatch spriteBatch = null) : base("texture:connection_node:male", owner, connectionData.ToVector3((float)Math.PI), info, game, spriteBatch)
        {
        }

        internal override void Disconnect()
        {
            // Ensure both the root and current connection node owner are up to date
            this.Owner.Root.UpdateTransformationData();
            this.Owner.UpdateTransformationData();

            this.Owner.Body.Position = this.Owner.Root.Body.Position + Vector2.Transform(
                this.Owner.Body.LocalCenter,
                this.Owner.OffsetTranslationMatrix * this.Owner.Root.RotationMatrix);

            this.Owner.Body.Rotation = this.Owner.Root.Body.Rotation + this.Owner.OffsetRotation;

            base.Disconnect();
        }
    }
}

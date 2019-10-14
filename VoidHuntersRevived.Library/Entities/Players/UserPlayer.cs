using VoidHuntersRevived.Library.Scenes;
using Guppy.Network.Extensions.Lidgren;
using Guppy.Network.Security;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.Entities.Players
{
    public sealed class UserPlayer : Player
    {
        #region Public Attributes
        public User User { get; set; }

        public override String Name { get { return this.User.Name; } }
        #endregion

        #region Constructor
        public UserPlayer(WorldScene scene) : base(scene)
        {
        }
        #endregion

        #region Network Methods
        protected override void WritePreInitialize(NetOutgoingMessage om)
        {
            base.WritePreInitialize(om);

            if (om.WriteExists(this.User))
                om.Write(this.User.Id);
        }

        protected override void ReadPreInitialize(NetIncomingMessage im)
        {
            base.ReadPreInitialize(im);

            if (im.ReadBoolean())
                this.User = this.scene.Group.Users.GetById(im.ReadGuid());
        }
        #endregion
    }
}

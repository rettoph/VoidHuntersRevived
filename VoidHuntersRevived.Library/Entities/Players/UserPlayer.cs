using Guppy.Network.Extensions.Lidgren;
using Guppy.Network.Security;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Scenes;

namespace VoidHuntersRevived.Library.Entities.Players
{
    public sealed class UserPlayer : Player
    {
        #region Public Attributes
        public User User { get; set; }

        public override String Name { get { return this.User.Name; } }
        #endregion

        #region Constructor
        public UserPlayer() : base()
        {
        }
        #endregion

        #region Network Methods
        protected override void WriteSetup(NetOutgoingMessage om)
        {
            base.WriteSetup(om);

            if (om.WriteExists(this.User))
                om.Write(this.User.Id);
        }

        protected override void ReadSetup(NetIncomingMessage im)
        {
            base.ReadSetup(im);

            Boolean test;
            Guid id;
            if (test = im.ReadBoolean())
                this.User = this.group.Users.GetById(id = im.ReadGuid());

            if (this.User == null)
            {

            }
        }
        #endregion
    }
}

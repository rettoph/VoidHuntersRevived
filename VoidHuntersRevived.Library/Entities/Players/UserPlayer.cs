using System;
using System.Collections.Generic;
using System.Text;
using Guppy.Configurations;
using Guppy.Network;
using Guppy.Network.Extensions.Lidgren;
using Guppy.Network.Security;
using Guppy.Network.Security.Collections;
using Lidgren.Network;
using Microsoft.Extensions.DependencyInjection;

namespace GalacticFighters.Library.Entities.Players
{
    /// <summary>
    /// A player implementation used to interface
    /// directly with a human user
    /// </summary>
    public class UserPlayer : Player
    {
        #region Public Attributes
        public override String Name { get { return this.User.Name; } }

        public User User { get; private set; }
        #endregion

        #region Constructors
        #endregion

        #region Network Methods
        protected override void Read(NetIncomingMessage im)
        {
            base.Read(im);

            // Read and set the current user from the stream
            this.User = this.provider.GetRequiredService<UserCollection>().GetById(
                im.ReadGuid());
        }

        protected override void Write(NetOutgoingMessage om)
        {
            base.Write(om);

            // Write the current user to the message...
            om.Write(this.User.Id);
        }
        #endregion
    }
}

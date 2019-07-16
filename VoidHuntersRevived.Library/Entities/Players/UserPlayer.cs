using System;
using System.Collections.Generic;
using System.Text;
using Guppy.Configurations;
using Guppy.Network;
using Guppy.Network.Extensions.Lidgren;
using Guppy.Network.Security;
using Lidgren.Network;

namespace VoidHuntersRevived.Library.Entities.Players
{
    /// <summary>
    /// A player implementation used to interface
    /// directly with a human user
    /// </summary>
    public class UserPlayer : Player
    {
        #region Public Attributes
        public override String Name { get { return this.User.Get("name"); } }

        public User User { get; private set; }
        #endregion

        #region Constructors
        public UserPlayer(User user, EntityConfiguration configuration, IServiceProvider provider) : base(configuration, provider)
        {
            this.User = user;
        }
        public UserPlayer(Guid id, EntityConfiguration configuration, IServiceProvider provider) : base(id, configuration, provider)
        {
        }
        #endregion

        #region Network Methods
        protected override void read(NetIncomingMessage im)
        {
            base.read(im);

            // Read and set the current user from the stream
            this.User = (this.scene as NetworkScene).Group.Users.GetById(
                im.ReadGuid());
        }

        protected override void write(NetOutgoingMessage om)
        {
            base.write(om);

            // Write the current user to the message...
            om.Write(this.User.Id);
        }
        #endregion
    }
}

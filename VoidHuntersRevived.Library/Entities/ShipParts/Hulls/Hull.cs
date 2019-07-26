using System;
using System.Collections.Generic;
using System.Text;
using Guppy.Configurations;

namespace VoidHuntersRevived.Library.Entities.ShipParts.Hulls
{
    public class Hull : RigidShipPart
    {
        #region Constructors
        public Hull(EntityConfiguration configuration, IServiceProvider provider) : base(configuration, provider)
        {
        }
        public Hull(Guid id, EntityConfiguration configuration, IServiceProvider provider) : base(id, configuration, provider)
        {
        }
        #endregion
    }
}

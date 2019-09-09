using GalacticFighters.Library.Entities.ShipParts.ConnectionNodes;
using Guppy.Factories;
using Guppy.Pooling.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GalacticFighters.Library.Factories
{
    public class ConnectionNodeFactory : CreatableFactory<ConnectionNode>
    {
        #region Constructor
        public ConnectionNodeFactory(IPoolManager<ConnectionNode> pools, IServiceProvider provider) : base(pools, provider)
        {
        }
        #endregion
    }
}

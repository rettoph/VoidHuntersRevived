using Guppy;
using Guppy.Configurations;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;

namespace VoidHuntersRevived.Library.Entities.Drivers
{
    /// <summary>
    /// Base entity that represents another
    /// entities driver.
    /// </summary>
    public abstract class Driver<TParent> : Entity
        where TParent : DrivenEntity
    {
        protected TParent parent;

        public Driver(TParent parent, EntityConfiguration configuration, Scene scene, ILogger logger) : base(configuration, scene, logger)
        {
            this.parent = parent;
        }

        public Driver(TParent parent, Guid id, EntityConfiguration configuration, Scene scene, ILogger logger) : base(id, configuration, scene, logger)
        {
            this.parent = parent;
        }
    }
}

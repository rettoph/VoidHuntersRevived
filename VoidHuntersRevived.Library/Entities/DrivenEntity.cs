using Guppy;
using Guppy.Collections;
using Guppy.Configurations;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.Drivers;
using Microsoft.Extensions.DependencyInjection;
using Guppy.Network;

namespace VoidHuntersRevived.Library.Entities
{
    /// <summary>
    /// Entity that contains a driver
    /// </summary>
    public abstract class DrivenEntity : NetworkEntity
    {
        private String _driverHandle;
        private IServiceProvider _provider;

        protected Driver<DrivenEntity> driver;

        public DrivenEntity(EntityConfiguration configuration, Scene scene, ILogger logger, IServiceProvider provider, String driverHandle) : base(configuration, scene, logger)
        {
            _provider = provider;
            _driverHandle = driverHandle;
        }

        public DrivenEntity(Guid id, EntityConfiguration configuration, Scene scene, ILogger logger, IServiceProvider provider, String driverHandle) : base(id, configuration, scene, logger)
        {
            _provider = provider;
            _driverHandle = driverHandle;
        }

        protected override void Boot()
        {
            base.Boot();

            // Create a new entity driver
            this.driver = _provider.GetService<EntityCollection>().Create<Driver<DrivenEntity>>(_driverHandle, this);
        }
    }
}

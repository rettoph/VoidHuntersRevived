using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Core.Interfaces
{
    /// <summary>
    /// Providers can be used to manage non service based assets.
    /// A gobal provider collection will reside within a game, and each provider
    /// will be registered to the main service collection as a singleton
    /// 
    /// Custom providers should be added within the ServiceProvider's boot
    /// method via Game.Providers.Add();
    /// </summary>
    public interface ILoader
    {
        void Initialize();
    }
}

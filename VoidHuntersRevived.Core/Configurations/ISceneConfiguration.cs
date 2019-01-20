using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Core.Interfaces;

namespace VoidHuntersRevived.Core.Configurations
{
    /// <summary>
    /// Scene configurations can be added
    /// to specific scene types (and their
    /// parent types) using the
    /// SceneFactory.ApplyConfiguration() method
    /// (SceneFActory is a singleton)
    /// method.
    /// </summary>
    public interface ISceneConfiguration
    {
        void Configure(IScene scene);
    }
}

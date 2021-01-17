using Guppy;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Builder.Services
{
    /// <summary>
    /// The base class representing a service capable of designing a
    /// ShipPartContext instance. Extend this type and add the
    /// ShipPartContextBuilderService Attribute to automatically load
    /// the service when creating a new context instance.
    /// </summary>
    public abstract class ShipPartContextBuilderService : Frameable
    {
    }
}

using Guppy;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.Drivers
{
    public abstract class SlaveNetworkAuthorizationDriver<TDriven> : RemoteHostDriver<TDriven>
        where TDriven : Driven
    {
    }
}

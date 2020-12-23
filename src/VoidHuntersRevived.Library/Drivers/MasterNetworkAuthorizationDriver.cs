using Guppy;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.Drivers
{
    public abstract class MasterNetworkAuthorizationDriver<TDriven> : RemoteHostDriver<TDriven>
        where TDriven : Driven
    {
    }
}

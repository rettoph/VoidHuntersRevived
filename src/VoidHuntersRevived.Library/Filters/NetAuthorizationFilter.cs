using Guppy.Common;
using Guppy.Common.Filters;
using Guppy.Network.Enums;
using Guppy.Resources.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Library.Filters
{
    public class NetAuthorizationFilter : SettingFilter<NetAuthorization>
    {
        public NetAuthorizationFilter(NetAuthorization requiredAuth, Type implementationType) : base(requiredAuth, implementationType)
        {
        }
    }

    public class NetAuthorizationFilter<TImplementation> : NetAuthorizationFilter
    {
        public NetAuthorizationFilter(NetAuthorization requiredAuth) : base(requiredAuth, typeof(TImplementation))
        {
        }
    }
}

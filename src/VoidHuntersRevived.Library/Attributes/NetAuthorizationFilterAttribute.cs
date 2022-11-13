using Guppy.Attributes;
using Guppy.Network.Enums;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Filters;

namespace VoidHuntersRevived.Library.Attributes
{
    public sealed class NetAuthorizationFilterAttribute : InitializableAttribute
    {
        public readonly NetAuthorization RequiredAuth;

        public NetAuthorizationFilterAttribute(NetAuthorization requiredAuth)
        {
            this.RequiredAuth = requiredAuth;
        }

        public override void Initialize(IServiceCollection services, Type classType)
        {
            base.Initialize(services, classType);

            services.AddFilter(new NetAuthorizationFilter(this.RequiredAuth, classType));
        }
    }
}

using Guppy.ECS.Attributes;
using Guppy.Network.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Filters;

namespace VoidHuntersRevived.Library.Attributes
{
    public sealed class NetAuthorizationSystemAttribute : SystemFilterAttribute
    {
        public readonly NetAuthorization RequiredAuth;

        public NetAuthorizationSystemAttribute(NetAuthorization requiredAuth)
        {
            this.RequiredAuth = requiredAuth;
        }

        protected override object GetInstance(Type classType, Type returnType)
        {
            return NetAuthorizationSystemFilter.GetInstance(this.RequiredAuth);
        }
    }
}

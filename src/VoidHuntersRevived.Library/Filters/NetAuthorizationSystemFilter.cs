using Guppy.Common;
using Guppy.ECS.Definitions;
using Guppy.Network.Enums;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Library.Filters
{
    public sealed class NetAuthorizationSystemFilter : IFilter<ISystemDefinition>
    {
        private NetAuthorization _requiredAuth;

        private NetAuthorizationSystemFilter(NetAuthorization requiredAuth)
        {
            _requiredAuth = requiredAuth;
        }

        public bool Invoke(IServiceProvider provider, ISystemDefinition arg)
        {
            var currentAuth = provider.GetSetting<NetAuthorization>();

            var result = _requiredAuth == currentAuth.Value;

            return result;
        }

        private static Dictionary<NetAuthorization, NetAuthorizationSystemFilter> _cache = new Dictionary<NetAuthorization, NetAuthorizationSystemFilter>();
        
        public static NetAuthorizationSystemFilter GetInstance(NetAuthorization requiredAuth)
        {
            if (_cache.TryGetValue(requiredAuth, out var filter))
            {
                return filter;
            }

            return _cache[requiredAuth] = new NetAuthorizationSystemFilter(requiredAuth);
        }
    }
}

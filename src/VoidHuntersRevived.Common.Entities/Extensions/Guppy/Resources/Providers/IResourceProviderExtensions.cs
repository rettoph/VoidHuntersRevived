using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Components;

namespace Guppy.Resources.Providers
{
    public static class IResourceProviderExtensions
    {
        public static T? Get<T>(this IResourceProvider resources, ResourceId<T> id)
            where T : notnull
        {
            if(id.Value == Guid.Empty)
            {
                throw new ArgumentException();
            }

            var resource = (Resource<T>)Resource.Get(id.Value);

            return resources.Get(resource);
        }
    }
}

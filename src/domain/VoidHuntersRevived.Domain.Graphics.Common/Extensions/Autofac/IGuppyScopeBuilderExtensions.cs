using Guppy.Core.Common;
using Guppy.Core.Serialization.Common.Extensions;
using Microsoft.Xna.Framework.Graphics;

namespace VoidHuntersRevived.Domain.Graphics.Common.Extensions.Autofac
{
    public static class IGuppyScopeBuilderExtensions
    {
        public static IGuppyScopeBuilder RegisterPrimitiveType<TVertexInstance, TVertexStatic, TEffect>(this IGuppyScopeBuilder builder, string name)
            where TVertexInstance : unmanaged, IVertexType
            where TVertexStatic : unmanaged, IVertexType
            where TEffect : Effect
        {
            builder.RegisterPolymorphicJsonType<IPrimitiveType<TVertexInstance, TVertexStatic, TEffect>, IPrimitiveType>(name);

            return builder;
        }
    }
}
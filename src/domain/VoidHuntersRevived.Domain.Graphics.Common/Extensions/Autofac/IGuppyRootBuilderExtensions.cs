using Guppy.Core.Common.Builders;
using Guppy.Core.Serialization.Common.Extensions;
using Microsoft.Xna.Framework.Graphics;

namespace VoidHuntersRevived.Domain.Graphics.Common.Extensions.Autofac
{
    public static class IGuppyRootBuilderExtensions
    {
        public static IGuppyRootBuilder RegisterPrimitiveType<TVertexInstance, TVertexStatic, TEffect>(this IGuppyRootBuilder builder, string name)
            where TVertexInstance : unmanaged, IVertexType
            where TVertexStatic : unmanaged, IVertexType
            where TEffect : Effect
        {
            builder.RegisterPolymorphicJsonType<IPrimitiveType<TVertexInstance, TVertexStatic, TEffect>, IPrimitiveType>(name);

            return builder;
        }
    }
}
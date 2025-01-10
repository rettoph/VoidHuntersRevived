using Autofac;
using Guppy.Core.Serialization.Common.Extensions;
using Microsoft.Xna.Framework.Graphics;

namespace VoidHuntersRevived.Domain.Graphics.Common.Extensions.Autofac
{
    public static class ContainerBuilderExtensions
    {
        public static void RegisterPrimitiveType<TVertexInstance, TVertexStatic, TEffect>(this ContainerBuilder builder, string name)
            where TVertexInstance : unmanaged, IVertexType
            where TVertexStatic : unmanaged, IVertexType
            where TEffect : Effect
        {
            builder.RegisterPolymorphicJsonType<IPrimitiveType<TVertexInstance, TVertexStatic, TEffect>, IPrimitiveType>(name);
        }
    }
}
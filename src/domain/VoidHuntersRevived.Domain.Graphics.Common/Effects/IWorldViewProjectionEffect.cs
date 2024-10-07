using Microsoft.Xna.Framework;

namespace VoidHuntersRevived.Domain.Graphics.Common.Effects
{
    public interface IWorldViewProjectionEffect
    {
        Matrix WorldViewProjection { set; }
    }
}

using Microsoft.Xna.Framework;
using VoidHuntersRevived.Domain.Pieces.Common.Components.Static;

namespace VoidHuntersRevived.Domain.Client.Common.Services
{
    public interface IVisibleRenderingService
    {
        void Begin(Color primaryColor, Color secondaryColor);
        void End();

        void Draw(in Visible visible, ref Matrix transformation);
    }
}

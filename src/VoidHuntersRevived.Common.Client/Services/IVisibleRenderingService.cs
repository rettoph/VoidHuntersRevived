using Microsoft.Xna.Framework;
using VoidHuntersRevived.Common.Pieces.Components;

namespace VoidHuntersRevived.Common.Client.Services
{
    public interface IVisibleRenderingService
    {
        void Begin(Color primaryColor, Color secondaryColor);
        void End();

        void Draw(in Visible visible, ref Matrix transformation);
    }
}

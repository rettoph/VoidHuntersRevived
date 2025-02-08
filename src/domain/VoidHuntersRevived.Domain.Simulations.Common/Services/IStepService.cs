using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Domain.Simulations.Common.Services
{
    public interface IStepService
    {
        void Update(GameTime gameTime);
        bool TryGetNextStep([MaybeNullWhen(false)] out Step step);
    }
}

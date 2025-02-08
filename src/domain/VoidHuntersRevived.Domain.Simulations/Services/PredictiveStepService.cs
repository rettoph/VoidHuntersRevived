using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Domain.Simulations.Common.Services;

namespace VoidHuntersRevived.Domain.Simulations.Services
{
    public class PredictiveStepService : IStepService
    {
        private bool _dirty = false;
        private readonly Step _step = new();

        public void Update(GameTime gameTime)
        {
            this._step.ElapsedTime = (Fix64)gameTime.ElapsedGameTime.TotalSeconds;
            this._step.TotalTime += this._step.ElapsedTime;
            this._dirty = true;
        }

        public bool TryGetNextStep([MaybeNullWhen(false)] out Step step)
        {
            if (this._dirty == false)
            {
                step = default;
                return false;
            }

            this._dirty = false;
            step = this._step;

            return true;
        }
    }
}

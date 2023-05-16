using Guppy.Attributes;
using Guppy.Common.DependencyInjection;
using Guppy.Loaders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Enums;
using VoidHuntersRevived.Domain.Entities.Events;
using VoidHuntersRevived.Domain.Client.Constants;
using Guppy.Common;
using Guppy.Input.Enums;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Domain.Client.Loaders
{
    [AutoLoad]
    internal sealed class InputLoader : IServiceLoader
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddInput(Inputs.TractorBeam, CursorButtons.Right, new (bool, IMessage)[]
            {
                (true, new ActivateTractorBeamEmitter()
                {
                    TractorBeamEmitterKey = default!,
                }),
                (false, new DeactivateTractorBeamEmitter()
                {
                    TractorBeamEmitterKey = default!
                }),
            });

            InputLoader.AddSetDirectionInput(services, Inputs.SetDirectionForward, Keys.W, Direction.Forward);
            InputLoader.AddSetDirectionInput(services, Inputs.SetDirectionTurnRight, Keys.D, Direction.TurnRight);
            InputLoader.AddSetDirectionInput(services, Inputs.SetDirectionBackward, Keys.S, Direction.Backward);
            InputLoader.AddSetDirectionInput(services, Inputs.SetDirectionTurnLeft, Keys.A, Direction.TurnLeft);
            InputLoader.AddSetDirectionInput(services, Inputs.SetDirectionRight, Keys.E, Direction.Right);
            InputLoader.AddSetDirectionInput(services, Inputs.SetDirectionLeft, Keys.Q, Direction.Left);
        }

        private static void AddSetDirectionInput(IServiceCollection services, string key, Keys defaultSource, Direction direction)
        {
            services.AddInput(key, defaultSource, new[]
            {
                (true, new SetHelmDirection()
                {
                    HelmKey = ParallelKey.Empty,
                    Which = direction,
                    Value = true
                }),
                (false, new SetHelmDirection()
                {
                    HelmKey = ParallelKey.Empty,
                    Which = direction,
                    Value = false
                }),
            });
        }
    }
}

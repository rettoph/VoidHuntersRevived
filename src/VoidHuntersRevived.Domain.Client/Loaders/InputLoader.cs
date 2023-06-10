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
using VoidHuntersRevived.Domain.Client.Constants;
using Guppy.Common;
using Guppy.Input.Enums;
using VoidHuntersRevived.Common.Simulations;
using Guppy.MonoGame.Messages;
using VoidHuntersRevived.Common.Simulations.Components;
using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Domain.Client.Loaders
{
    [AutoLoad]
    internal sealed class InputLoader : IServiceLoader
    {
        public void ConfigureServices(IServiceCollection services)
        {
            InputLoader.AddSetDirectionInput(services, Inputs.SetDirectionForward, Keys.W, Direction.Forward);
            InputLoader.AddSetDirectionInput(services, Inputs.SetDirectionTurnRight, Keys.D, Direction.TurnRight);
            InputLoader.AddSetDirectionInput(services, Inputs.SetDirectionBackward, Keys.S, Direction.Backward);
            InputLoader.AddSetDirectionInput(services, Inputs.SetDirectionTurnLeft, Keys.A, Direction.TurnLeft);
            InputLoader.AddSetDirectionInput(services, Inputs.SetDirectionRight, Keys.E, Direction.Right);
            InputLoader.AddSetDirectionInput(services, Inputs.SetDirectionLeft, Keys.Q, Direction.Left);

            services.AddInput(Inputs.ToggleLockstep, Keys.F12, new[]
            {
                (false, Toggle<Lockstep>.Instance)
            });

            services.AddInput(Inputs.TogglePredictive, Keys.F11, new[]
            {
                (false, Toggle<Predictive>.Instance)
            });
        }

        private static void AddSetDirectionInput(IServiceCollection services, string key, Keys defaultSource, Direction direction)
        {
            // services.AddInput(key, defaultSource, new[]
            // {
            //     (true, new SetHelmDirection()
            //     {
            //         HelmKey = EventId.Empty,
            //         Which = direction,
            //         Value = true
            //     }),
            //     (false, new SetHelmDirection()
            //     {
            //         HelmKey = EventId.Empty,
            //         Which = direction,
            //         Value = false
            //     }),
            // });
        }
    }
}

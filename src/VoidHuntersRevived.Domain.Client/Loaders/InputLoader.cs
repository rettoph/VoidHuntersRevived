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
using VoidHuntersRevived.Domain.Entities.Enums;
using VoidHuntersRevived.Domain.Entities.Events;
using VoidHuntersRevived.Domain.Client.Constants;
using VoidHuntersRevived.Domain.Client.Systems;

namespace VoidHuntersRevived.Domain.Client.Loaders
{
    [AutoLoad]
    internal sealed class InputLoader : IServiceLoader
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureCollection(manager =>
            {
                manager.AddScoped<InputSystem>()
                    .AddInterfaceAliases();
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
                (ButtonState.Pressed, new SetPilotingDirection(default, direction, true)),
                (ButtonState.Released, new SetPilotingDirection(default, direction, false)),
            });
        }
    }
}

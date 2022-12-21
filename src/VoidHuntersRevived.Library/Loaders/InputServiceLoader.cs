using Guppy.Attributes;
using Guppy.Loaders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Constants;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Messages.Inputs;

namespace VoidHuntersRevived.Library.Loaders
{
    [AutoLoad(0)]
    internal sealed class InputServiceLoader : IServiceLoader
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddInput<DirectionInput>(InputConstants.SetDirectionForward, Keys.W,
                new[]
                {
                    (ButtonState.Pressed, new DirectionInput(Direction.Forward, true)),
                    (ButtonState.Released, new DirectionInput(Direction.Forward, false))
                });

            services.AddInput<DirectionInput>(InputConstants.SetDirectionTurnRight, Keys.D,
                new[]
                {
                    (ButtonState.Pressed, new DirectionInput(Direction.TurnRight, true)),
                    (ButtonState.Released, new DirectionInput(Direction.TurnRight, false))
                });

            services.AddInput<DirectionInput>(InputConstants.SetDirectionBackward, Keys.S,
                new[]
                {
                    (ButtonState.Pressed, new DirectionInput(Direction.Backward, true)),
                    (ButtonState.Released, new DirectionInput(Direction.Backward, false))
                });

            services.AddInput<DirectionInput>(InputConstants.SetDirectionTurnLeft, Keys.A,
                new[]
                {
                    (ButtonState.Pressed, new DirectionInput(Direction.TurnLeft, true)),
                    (ButtonState.Released, new DirectionInput(Direction.TurnLeft, false))
                });

            services.AddInput<DirectionInput>(InputConstants.SetDirectionRight, Keys.E,
                new[]
                {
                    (ButtonState.Pressed, new DirectionInput(Direction.Right, true)),
                    (ButtonState.Released, new DirectionInput(Direction.Right, false))
                });

            services.AddInput<DirectionInput>(InputConstants.SetDirectionLeft, Keys.Q,
                new[]
                {
                    (ButtonState.Pressed, new DirectionInput(Direction.Left, true)),
                    (ButtonState.Released, new DirectionInput(Direction.Left, false))
                });
        }
    }
}

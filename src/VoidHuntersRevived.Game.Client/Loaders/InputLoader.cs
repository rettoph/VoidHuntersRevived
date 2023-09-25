﻿using Autofac;
using Guppy.Attributes;
using Guppy.Common;
using Guppy.Input.Enums;
using Guppy.Loaders;
using Microsoft.Xna.Framework.Input;
using VoidHuntersRevived.Common.Pieces.Enums;
using VoidHuntersRevived.Game.Client.Constants;
using VoidHuntersRevived.Game.Client.Messages;
using VoidHuntersRevived.Game.Common.Events;

namespace VoidHuntersRevived.Game.Client.Loaders
{
    [AutoLoad]
    internal sealed class InputLoader : IServiceLoader
    {
        public void ConfigureServices(ContainerBuilder services)
        {
            InputLoader.AddSetDirectionInput(services, Inputs.SetDirectionForward, Keys.W, Direction.Forward);
            InputLoader.AddSetDirectionInput(services, Inputs.SetDirectionTurnRight, Keys.D, Direction.TurnRight);
            InputLoader.AddSetDirectionInput(services, Inputs.SetDirectionBackward, Keys.S, Direction.Backward);
            InputLoader.AddSetDirectionInput(services, Inputs.SetDirectionTurnLeft, Keys.A, Direction.TurnLeft);
            InputLoader.AddSetDirectionInput(services, Inputs.SetDirectionRight, Keys.E, Direction.Right);
            InputLoader.AddSetDirectionInput(services, Inputs.SetDirectionLeft, Keys.Q, Direction.Left);

            services.AddInput(Inputs.SetTractorBeamEmitterActive, CursorButtons.Right, new(ButtonState, IMessage)[]
{
                (ButtonState.Pressed, new Input_TractorBeamEmitter_SetActive(true)),
                (ButtonState.Released, new Input_TractorBeamEmitter_SetActive(false))
            });

            services.AddInput(Inputs.ToggleLockstepWireframe, Keys.F12, new (ButtonState, IMessage)[]
            {
                (ButtonState.Released, new Input_Toggle_LockstepWireframe())
            });

            services.AddInput(Inputs.ToggleFps, Keys.F11, new (ButtonState, IMessage)[]
            {
                            (ButtonState.Released, new Input_Toggle_FPS())
            });
        }

        private static void AddSetDirectionInput(ContainerBuilder services, string key, Keys defaultSource, Direction direction)
        {
            services.AddInput(key, defaultSource, new[]
            {
                (KeyState.Down, new Input_Helm_SetDirection()
                {
                    Which = direction,
                    Value = true
                }),
                (KeyState.Up, new Input_Helm_SetDirection()
                {
                    Which = direction,
                    Value = false
                }),
            });
        }
    }
}

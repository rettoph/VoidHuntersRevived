using Autofac;
using Guppy.Attributes;
using Guppy.Game.Input;
using Guppy.Game.Input.Enums;
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

            services.RegisterInput(Inputs.SetTractorBeamEmitterActive, CursorButtons.Right, new (ButtonState, IInput)[]
{
                (ButtonState.Pressed, new Input_TractorBeamEmitter_SetActive(true)),
                (ButtonState.Released, new Input_TractorBeamEmitter_SetActive(false))
            });

            services.RegisterInput(Inputs.ToggleLockstepWireframe, Keys.F12, new (ButtonState, IInput)[]
            {
                (ButtonState.Released, new Input_Toggle_LockstepWireframe())
            });

            services.RegisterInput(Inputs.ToggleFps, Keys.F11, new (ButtonState, IInput)[]
            {
                (ButtonState.Released, new Input_Toggle_FPS())
            });

            services.RegisterInput(Inputs.InvokeGarbageCollection, Keys.F10, new (ButtonState, IInput)[]
            {
                (ButtonState.Released, new Input_Invoke_Garbage_Collection())
            });
        }

        private static void AddSetDirectionInput(ContainerBuilder services, string key, Keys defaultSource, Direction direction)
        {
            services.RegisterInput(key, defaultSource, new[]
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

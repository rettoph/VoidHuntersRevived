using Autofac;
using Guppy.Core.Common.Attributes;
using Guppy.Game.Input.Common;
using Guppy.Game.Input.Common.Enums;
using Microsoft.Xna.Framework.Input;
using VoidHuntersRevived.Domain.Pieces.Common.Enums;
using VoidHuntersRevived.Game.Client.Constants;
using VoidHuntersRevived.Game.Client.Messages;
using VoidHuntersRevived.Game.Core.Events;

namespace VoidHuntersRevived.Game.Client.Modules
{
    [AutoLoad]
    internal sealed class InputModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            InputModule.AddSetDirectionInput(builder, Inputs.SetDirectionForward, Keys.W, Direction.Forward);
            InputModule.AddSetDirectionInput(builder, Inputs.SetDirectionTurnRight, Keys.D, Direction.TurnRight);
            InputModule.AddSetDirectionInput(builder, Inputs.SetDirectionBackward, Keys.S, Direction.Backward);
            InputModule.AddSetDirectionInput(builder, Inputs.SetDirectionTurnLeft, Keys.A, Direction.TurnLeft);
            InputModule.AddSetDirectionInput(builder, Inputs.SetDirectionRight, Keys.E, Direction.Right);
            InputModule.AddSetDirectionInput(builder, Inputs.SetDirectionLeft, Keys.Q, Direction.Left);

            builder.RegisterInput(Inputs.SetTractorBeamEmitterActive, CursorButtons.Right, new (ButtonState, IInput)[]
            {
                (ButtonState.Pressed, new Input_TractorBeamEmitter_SetActive(true)),
                (ButtonState.Released, new Input_TractorBeamEmitter_SetActive(false))
            });

            builder.RegisterInput(Inputs.ToggleLockstepWireframe, Keys.F12, new (ButtonState, IInput)[]
            {
                (ButtonState.Released, new Input_Toggle_LockstepWireframe())
            });

            builder.RegisterInput(Inputs.ToggleFps, Keys.F11, new (ButtonState, IInput)[]
            {
                (ButtonState.Released, new Input_Toggle_FPS())
            });

            builder.RegisterInput(Inputs.InvokeGarbageCollection, Keys.F10, new (ButtonState, IInput)[]
            {
                (ButtonState.Released, new Input_Invoke_Garbage_Collection())
            });

            builder.RegisterInput(Inputs.SpamClick, Keys.NumPad0, new (ButtonState, IInput)[]
            {
                (ButtonState.Pressed, Input_Spam_Click.True),
                (ButtonState.Released, Input_Spam_Click.False),
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

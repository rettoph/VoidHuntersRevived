using Autofac;
using Guppy.Core.Common.Extensions.Autofac;
using Guppy.Game;
using Guppy.Game.Input.Common;
using Guppy.Game.Input.Common.Enums;
using Guppy.Game.MonoGame.Common.Extensions;
using Microsoft.Xna.Framework.Input;
using VoidHuntersRevived.Domain.Pieces.Common.Enums;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Extensions;
using VoidHuntersRevived.Game.Client.Components.Scene;
using VoidHuntersRevived.Game.Client.Constants;
using VoidHuntersRevived.Game.Client.Engines;
using VoidHuntersRevived.Game.Client.Engines.Debugging;
using VoidHuntersRevived.Game.Client.Messages;
using VoidHuntersRevived.Game.Core.Events;

namespace VoidHuntersRevived.Game.Client.Extensions
{
    public static class ContainerBuilderExtensions
    {
        public static ContainerBuilder RegisterGameClientServices(this ContainerBuilder builder)
        {
            return builder.EnsureRegisteredOnce(nameof(RegisterGameClientServices), builder =>
            {
                builder.RegisterType<ClientPeerComponent>().AsImplementedInterfaces().InstancePerLifetimeScope();
                builder.RegisterType<ConfigureSimulationsComponent>().AsImplementedInterfaces().InstancePerLifetimeScope();
                builder.RegisterType<DebugEngineComponent>().AsImplementedInterfaces().InstancePerLifetimeScope();
                builder.RegisterType<ImGuiEngineComponent>().AsImplementedInterfaces().InstancePerLifetimeScope();
                builder.RegisterType<InvokeGarbageCollectionComponent>().AsImplementedInterfaces().InstancePerLifetimeScope();

                builder.RegisterEngine<CameraEngine>();
                builder.RegisterEngine<AetherDebugEngine>();
                builder.RegisterEngine<EntitiesDebugEngine>();
                builder.RegisterEngine<LockstepStrategy_ClientDebugEngine>();
                builder.RegisterEngine<LockstepStrategy_ServerDebugEngine>();
                builder.RegisterEngine<LockstepStrategyDebugEngine>();
                builder.RegisterEngine<StrategyDebugEngine>();
                builder.RegisterEngine<DrawVertexVisibleEngine>();
                builder.RegisterEngine<InputEngine>();
                builder.RegisterEngine<ShaderAntiAliasingEngine>();

                builder.Configure<ISceneConfiguration<IStrategy>>((scope, configuration) =>
                {
                    configuration.SetSceneHasDebugWindow(true).SetSceneHasTerminalWindow(true);
                });

                ContainerBuilderExtensions.RegisterInputs(builder);
            });
        }

        private static void RegisterInputs(ContainerBuilder builder)
        {
            ContainerBuilderExtensions.AddSetDirectionInput(builder, Inputs.SetDirectionForward, Keys.W, DirectionEnum.Forward);
            ContainerBuilderExtensions.AddSetDirectionInput(builder, Inputs.SetDirectionTurnRight, Keys.D, DirectionEnum.TurnRight);
            ContainerBuilderExtensions.AddSetDirectionInput(builder, Inputs.SetDirectionBackward, Keys.S, DirectionEnum.Backward);
            ContainerBuilderExtensions.AddSetDirectionInput(builder, Inputs.SetDirectionTurnLeft, Keys.A, DirectionEnum.TurnLeft);
            ContainerBuilderExtensions.AddSetDirectionInput(builder, Inputs.SetDirectionRight, Keys.E, DirectionEnum.Right);
            ContainerBuilderExtensions.AddSetDirectionInput(builder, Inputs.SetDirectionLeft, Keys.Q, DirectionEnum.Left);

            builder.RegisterInput(Inputs.SetTractorBeamEmitterActive, CursorButtonsEnum.Right, new (ButtonState, IInput)[]
            {
                (ButtonState.Pressed, new Input_TractorBeamEmitter_SetActive(true)),
                (ButtonState.Released, new Input_TractorBeamEmitter_SetActive(false))
            });


            builder.RegisterInput(Inputs.ToggleFps, Keys.F12, new (ButtonState, IInput)[]
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

        private static void AddSetDirectionInput(ContainerBuilder services, string key, Keys defaultSource, DirectionEnum direction)
        {
            services.RegisterInput(key, defaultSource,
            [
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
            ]);
        }
    }
}
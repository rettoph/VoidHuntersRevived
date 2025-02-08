using Guppy.Core.Commands.Common;
using Guppy.Core.Common;
using Guppy.Core.Common.Extensions;
using Guppy.Core.Network.Common.Enums;
using Guppy.Core.Network.Common.Extensions;
using Guppy.Game.Common;
using Guppy.Game.Common.Extensions;
using Guppy.Game.Graphics.Common.Extensions;
using Guppy.Game.Input.Common;
using Guppy.Game.Input.Common.Enums;
using Guppy.Game.MonoGame.Common.Extensions;
using Microsoft.Xna.Framework.Input;
using VoidHuntersRevived.Domain.Pieces.Common.Enums;
using VoidHuntersRevived.Domain.Simulations;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Lockstep;
using VoidHuntersRevived.Domain.Simulations.Common.Predictive;
using VoidHuntersRevived.Domain.Simulations.Lockstep;
using VoidHuntersRevived.Game.Client.Components.Scene;
using VoidHuntersRevived.Game.Client.Constants;
using VoidHuntersRevived.Game.Client.Messages;
using VoidHuntersRevived.Game.Client.Systems;
using VoidHuntersRevived.Game.Client.Systems.Debugging;
using VoidHuntersRevived.Game.Core.Events;

namespace VoidHuntersRevived.Game.Client.Extensions
{
    public static class IGuppyScopeBuilderExtensions
    {
        public static IGuppyScopeBuilder RegisterGameClientServices(this IGuppyScopeBuilder builder)
        {
            return builder.EnsureRegisteredOnce(nameof(RegisterGameClientServices), builder =>
            {
                builder.RegisterSceneFilter<IScene>(builder =>
                {
                    builder.RegisterSceneSystem<InvokeGarbageCollectionSystem>();
                });

                builder.RegisterSceneFilter<MultiplayerGameScene>(builder =>
                {
                    builder.RegisterSceneSystem<ClientPeerSystem>();
                });

                builder.RegisterSceneFilter<LocalGameScene>(builder =>
                {
                    builder.RegisterSceneSystem<ConfigureSimulationsSystem>();
                });

                builder.RegisterSceneFilter<IStrategy>(builder =>
                {
                    builder.AddSceneHasDebugWindow(true).AddSceneHasTerminalWindow(true);

                    builder.RegisterSceneSystem<DebugEngineSystem>();

                    builder.RegisterGraphicsEnabledFilter(true, builder =>
                    {
                        builder.RegisterSceneSystem<AetherDebugSystem>();
                        builder.RegisterSceneSystem<EntitiesDebugSystem>();
                        builder.RegisterSceneSystem<DrawVertexVisibleSystem>();
                        builder.RegisterSceneSystem<ShaderAntiAliasingSystem>();
                    });

                    builder.RegisterSceneFilter<Strategy>(builder =>
                    {
                        builder.RegisterSceneSystem<StrategyDebugSystem>();
                    });

                    builder.RegisterSceneFilter<LockstepStrategy_Client>(builder =>
                    {
                        builder.RegisterSceneSystem<LockstepStrategy_ClientDebugSystem>();
                    });

                    builder.RegisterSceneFilter<LockstepStrategy_Server>(builder =>
                    {
                        builder.RegisterSceneSystem<LockstepStrategy_ServerDebugSystem>();
                    });

                    builder.RegisterSceneFilter<ILockstepStrategy>(builder =>
                    {
                        builder.RegisterSceneSystem<LockstepStrategyDebugSystem>();
                    });

                    builder.Filter(
                        filter => filter.RequirePeerType(PeerTypeEnum.Client).RequireScene<ILockstepStrategy>(),
                        builder =>
                        {
                            builder.RegisterSceneSystem<InputSystem>();
                        });

                    builder.Filter(
                        filter => filter.RequirePeerType(PeerTypeEnum.Client).RequireScene<IPredictiveStrategy>(),
                        builder =>
                        {
                            builder.RegisterSceneSystem<CameraSystem>();
                        });
                });

                IGuppyScopeBuilderExtensions.RegisterInputs(builder);
            });
        }

        private static void RegisterInputs(IGuppyScopeBuilder builder)
        {
            IGuppyScopeBuilderExtensions.AddSetDirectionInput(builder, Inputs.SetDirectionForward, Keys.W, DirectionEnum.Forward);
            IGuppyScopeBuilderExtensions.AddSetDirectionInput(builder, Inputs.SetDirectionTurnRight, Keys.D, DirectionEnum.TurnRight);
            IGuppyScopeBuilderExtensions.AddSetDirectionInput(builder, Inputs.SetDirectionBackward, Keys.S, DirectionEnum.Backward);
            IGuppyScopeBuilderExtensions.AddSetDirectionInput(builder, Inputs.SetDirectionTurnLeft, Keys.A, DirectionEnum.TurnLeft);
            IGuppyScopeBuilderExtensions.AddSetDirectionInput(builder, Inputs.SetDirectionRight, Keys.E, DirectionEnum.Right);
            IGuppyScopeBuilderExtensions.AddSetDirectionInput(builder, Inputs.SetDirectionLeft, Keys.Q, DirectionEnum.Left);

            builder.RegisterInput(Inputs.SetTractorBeamEmitterActive, CursorButtonsEnum.Right, new (ButtonState, IInputMessage)[]
            {
                (ButtonState.Pressed, new Input_TractorBeamEmitter_SetActive(true)),
                (ButtonState.Released, new Input_TractorBeamEmitter_SetActive(false))
            });


            builder.RegisterInput(Inputs.ToggleFps, Keys.F12, new (ButtonState, IInputMessage)[]
            {
                (ButtonState.Released, new Input_Toggle_FPS())
            });

            builder.RegisterInput(Inputs.InvokeGarbageCollection, Keys.F10, new (ButtonState, IInputMessage)[]
            {
                (ButtonState.Released, new Input_Invoke_Garbage_Collection())
            });

            builder.RegisterInput(Inputs.SpamClick, Keys.NumPad0, new (ButtonState, IInputMessage)[]
            {
                (ButtonState.Pressed, Input_Spam_Click.True),
                (ButtonState.Released, Input_Spam_Click.False),
            });
        }

        private static void AddSetDirectionInput(IGuppyScopeBuilder services, string key, Keys defaultSource, DirectionEnum direction)
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
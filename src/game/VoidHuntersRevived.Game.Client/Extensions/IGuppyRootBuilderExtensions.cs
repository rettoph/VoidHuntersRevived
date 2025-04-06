using Guppy.Core.Common.Builders;
using Guppy.Core.Common.Extensions;
using Guppy.Core.Network.Common.Enums;
using Guppy.Core.Network.Common.Extensions;
using Guppy.Game.Common;
using Guppy.Game.Common.Extensions;
using Guppy.Game.Graphics.Common.Extensions;
using Guppy.Game.Input.Common;
using Guppy.Game.Input.Common.Enums;
using Guppy.Game.Input.Common.Extensions;
using Guppy.Game.MonoGame.Common.Extensions;
using Microsoft.Xna.Framework.Input;
using VoidHuntersRevived.Domain.Pieces.Common.Enums;
using VoidHuntersRevived.Domain.Simulations.Common.Strategies;
using VoidHuntersRevived.Game.Client.Components.Scene;
using VoidHuntersRevived.Game.Client.Constants;
using VoidHuntersRevived.Game.Client.Messages;
using VoidHuntersRevived.Game.Client.Systems;
using VoidHuntersRevived.Game.Client.Systems.Debugging;
using VoidHuntersRevived.Game.Core.Events;

namespace VoidHuntersRevived.Game.Client.Extensions
{
    public static class IGuppyRootBuilderExtensions
    {
        public static IGuppyRootBuilder RegisterGameClientServices(this IGuppyRootBuilder builder)
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
                    builder.Variables.AddSceneHasDebugWindow(true).AddSceneHasTerminalWindow(true);

                    builder.RegisterSceneSystem<DebugEngineSystem>();

                    builder.RegisterGraphicsEnabledFilter(true, builder =>
                    {
                        builder.RegisterSceneSystem<AetherDebugSystem>();
                        builder.RegisterSceneSystem<EntitiesDebugSystem>();
                        builder.RegisterSceneSystem<DrawVertexVisibleSystem>();
                        //builder.RegisterSceneSystem<ShaderAntiAliasingSystem>();
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

                IGuppyRootBuilderExtensions.RegisterInputs(builder);
            });
        }

        private static void RegisterInputs(IGuppyRootBuilder builder)
        {
            IGuppyRootBuilderExtensions.AddSetDirectionInput(builder, Inputs.SetDirectionForward, Keys.W, DirectionEnum.Forward);
            IGuppyRootBuilderExtensions.AddSetDirectionInput(builder, Inputs.SetDirectionTurnRight, Keys.D, DirectionEnum.TurnRight);
            IGuppyRootBuilderExtensions.AddSetDirectionInput(builder, Inputs.SetDirectionBackward, Keys.S, DirectionEnum.Backward);
            IGuppyRootBuilderExtensions.AddSetDirectionInput(builder, Inputs.SetDirectionTurnLeft, Keys.A, DirectionEnum.TurnLeft);
            IGuppyRootBuilderExtensions.AddSetDirectionInput(builder, Inputs.SetDirectionRight, Keys.E, DirectionEnum.Right);
            IGuppyRootBuilderExtensions.AddSetDirectionInput(builder, Inputs.SetDirectionLeft, Keys.Q, DirectionEnum.Left);

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

        private static void AddSetDirectionInput(IGuppyRootBuilder services, string key, Keys defaultSource, DirectionEnum direction)
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
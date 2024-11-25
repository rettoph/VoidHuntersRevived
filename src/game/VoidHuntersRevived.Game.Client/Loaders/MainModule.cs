using Autofac;
using Guppy.Core.Common.Attributes;
using Guppy.Core.Common.Extensions.Autofac;
using Guppy.Game;
using Guppy.Game.MonoGame.Common.Extensions;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Extensions;
using VoidHuntersRevived.Game.Client.Components.Scene;
using VoidHuntersRevived.Game.Client.Engines;
using VoidHuntersRevived.Game.Client.Engines.Debugging;

namespace VoidHuntersRevived.Game.Client.Modules
{
    [AutoLoad]
    internal sealed class MainModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

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
            builder.RegisterEngine<DrawActiveThrustableEngine>();
            builder.RegisterEngine<DrawLockstepWireframeEngine>();
            builder.RegisterEngine<DrawVertexVisibleEngine>();
            builder.RegisterEngine<InputEngine>();
            builder.RegisterEngine<ShaderAntiAliasingEngine>();
            builder.RegisterEngine<TractorBeamHighlightEngine>();

            builder.Configure<ISceneConfiguration<IStrategy>>((scope, configuration) =>
            {
                configuration.SetSceneHasDebugWindow(true).SetSceneHasTerminalWindow(true);
            });
        }
    }
}

using Autofac;
using Guppy.Core.Common.Builders;
using Guppy.Core.Common.Extensions;
using Guppy.Core.Files.Common;
using Guppy.Core.Assets.Common.Configuration;
using Guppy.Core.Assets.Common.Extensions;
using Guppy.Game.Common.Extensions;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Domain.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Enums;
using VoidHuntersRevived.Domain.Graphics.Common.Components;
using VoidHuntersRevived.Domain.Graphics.Common.Extensions.Autofac;
using VoidHuntersRevived.Domain.Physics.Common.Components;
using VoidHuntersRevived.Domain.Pieces.Common.Components;
using VoidHuntersRevived.Domain.Pieces.Common.Constants;
using VoidHuntersRevived.Domain.Pieces.Common.Graphics.Vertices;
using VoidHuntersRevived.Domain.Ships.Common.Components;
using VoidHuntersRevived.Domain.Simulations.Common.Strategies;
using VoidHuntersRevived.Domain.Teams.Common.Components;
using VoidHuntersRevived.Game.Core.Components.Scene;
using VoidHuntersRevived.Game.Core.Graphics.Effects;
using VoidHuntersRevived.Game.Core.Systems;

namespace VoidHuntersRevived.Game.Core.Extensions
{
    public static class IGuppyRootBuilderExtensions
    {
        public static IGuppyRootBuilder RegisterGameCoreServices(this IGuppyRootBuilder builder)
        {
            return builder.EnsureRegisteredOnce(nameof(RegisterGameCoreServices), builder =>
            {
                builder.RegisterSceneFilter<VoidHuntersGameScene>(builder =>
                {
                    builder.RegisterSceneSystem<SimulationFrameSystem>();
                });

                builder.RegisterSceneFilter<IStrategy>(builder =>
                {
                    builder.RegisterSceneSystem<SimulationSystem>();
                    builder.RegisterSceneSystem<UserSystem>();
                });

                builder.RegisterType<ShaderAntiAliasingEffect>().SingleInstance();
                builder.RegisterType<VisibleEffect>().AsImplementedInterfaces().AsSelf().SingleInstance();

                builder.RegisterPrimitiveType<VertexVisible, VertexStaticVisible, VisibleEffect>("PrimitiveType.Visible");

                builder.RegisterAssetPack(new AssetPackConfiguration()
                {
                    EntryDirectory = DirectoryPath.CurrentDirectory(VoidHuntersPack.Directory)
                });

                // Register core game resources
                builder.RegisterTeamEntityTemplates()
                    .RegisterPhysicsEntityTemplates()
                    .RegisterPiecesEntityTemplates()
                    .RegisterShipsEntityTemplates();
            });
        }

        private static IGuppyRootBuilder RegisterTeamEntityTemplates(this IGuppyRootBuilder builder)
        {
            builder.RegisterAsset(Assets.EntityTemplates.Team.TeamEntityTemplate.Name, new EntityTemplateFragment()
            {
                Key = Assets.EntityTemplates.Team.TeamEntityTemplate,
                Flags = EntityTemplateFlagsEnum.Partial,
                RequiredComponents = [
                    typeof(Team),
                    typeof(ColorScheme)
                ]
            });

            builder.RegisterAsset(Assets.EntityTemplates.Team.DefaultTeamEntityTemplate.Name, new EntityTemplateFragment()
            {
                Key = Assets.EntityTemplates.Team.DefaultTeamEntityTemplate,
                Flags = EntityTemplateFlagsEnum.Partial,
                Components = [
                    new DefaultTeam()
                ],
                RequiredComponents = [
                    typeof(Team)
                ]
            });

            builder.RegisterAsset(Assets.EntityTemplates.Team.TeamMemberEntityTemplate.Name, new EntityTemplateFragment()
            {
                Key = Assets.EntityTemplates.Team.TeamMemberEntityTemplate,
                Flags = EntityTemplateFlagsEnum.Partial,
                Components = [
                    new TeamMember()
                ]
            });

            return builder;
        }

        private static IGuppyRootBuilder RegisterPhysicsEntityTemplates(this IGuppyRootBuilder builder)
        {
            builder.RegisterAsset(Assets.EntityTemplates.Physics.BodyEntityTemplate.Name, new EntityTemplateFragment()
            {
                Key = Assets.EntityTemplates.Physics.BodyEntityTemplate,
                Flags = EntityTemplateFlagsEnum.Partial,
                Inherit = Assets.EntityTemplates.Team.TeamMemberEntityTemplate,
                Components = [
                    Body.Default,
                    new Enabled(),
                    new Awake(true)
                ],
                RequiredComponents = [
                    typeof(Collision)
                ]
            });

            builder.RegisterAsset(Assets.EntityTemplates.Physics.FixtureEntityTemplate.Name, new EntityTemplateFragment()
            {
                Key = Assets.EntityTemplates.Physics.FixtureEntityTemplate,
                Flags = EntityTemplateFlagsEnum.Partial,
                Inherit = Assets.EntityTemplates.Team.TeamMemberEntityTemplate,
                Components = [
                    new Fixture()
                ]
            });

            return builder;
        }

        private static IGuppyRootBuilder RegisterPiecesEntityTemplates(this IGuppyRootBuilder builder)
        {
            builder.RegisterAsset(Assets.EntityTemplates.Piece.TreeEntityTemplate.Name, new EntityTemplateFragment()
            {
                Key = Assets.EntityTemplates.Piece.TreeEntityTemplate,
                Flags = EntityTemplateFlagsEnum.Partial,
                Inherit = Assets.EntityTemplates.Physics.BodyEntityTemplate,
                Components = [
                    new Tree(),
                ]
            });

            builder.RegisterAsset(Assets.EntityTemplates.Piece.PieceEntityTemplate.Name, new EntityTemplateFragment()
            {
                Key = Assets.EntityTemplates.Piece.PieceEntityTemplate,
                Flags = EntityTemplateFlagsEnum.Partial,
                Inherit = Assets.EntityTemplates.Physics.FixtureEntityTemplate,
                Components = [
                    Plug.Default,
                    new Coupling(),
                    new Node(),
                    new VertexVisible()
                ],
                RequiredComponents = [
                    typeof(Rigid),
                    typeof(ColorScheme),
                    typeof(Primitive<VertexVisible>)
                ]
            });

            builder.RegisterAsset(Assets.EntityTemplates.Piece.ThrusterEntityTemplate.Name, new EntityTemplateFragment()
            {
                Key = Assets.EntityTemplates.Piece.ThrusterEntityTemplate,
                Flags = EntityTemplateFlagsEnum.Partial,
                Inherit = Assets.EntityTemplates.Piece.PieceEntityTemplate,
                Components = [
                    new Thrustable()
                ]
            });

            builder.RegisterAsset(Assets.EntityTemplates.Piece.HullEntityTemplate.Name, new EntityTemplateFragment()
            {
                Key = Assets.EntityTemplates.Piece.HullEntityTemplate,
                Flags = EntityTemplateFlagsEnum.Partial,
                Inherit = Assets.EntityTemplates.Piece.PieceEntityTemplate,
                RequiredComponents = [
                    typeof(Sockets)
                ]
            });

            return builder;
        }

        private static IGuppyRootBuilder RegisterShipsEntityTemplates(this IGuppyRootBuilder builder)
        {
            builder.RegisterAsset(Assets.EntityTemplates.Ship.ChainEntityTemplate.Name, new EntityTemplateFragment()
            {
                Key = Assets.EntityTemplates.Ship.ChainEntityTemplate,
                Inherit = Assets.EntityTemplates.Piece.TreeEntityTemplate,
                Components = [
                    new Tractorable(),
                    new Collision()
                    {
                        Categories = CollisionGroups.FreeFloatingCategories,
                        CollidesWith = CollisionGroups.FreeFloatingCollidesWith
                    }
                ]
            });

            builder.RegisterAsset(Assets.EntityTemplates.Ship.ShipEntityTemplate.Name, new EntityTemplateFragment()
            {
                Key = Assets.EntityTemplates.Ship.ShipEntityTemplate,
                Flags = EntityTemplateFlagsEnum.Partial,
                Inherit = Assets.EntityTemplates.Piece.TreeEntityTemplate,
                Components = [
                    new PhysicsBubble() {
                        Enabled = false,
                        Radius = default
                    },
                    new Helm(),
                    new Tactical(),
                    new TractorBeamEmitter(default)
                ]
            });

            builder.RegisterAsset(Assets.EntityTemplates.Ship.UserShipEntityTemplate.Name, new EntityTemplateFragment()
            {
                Key = Assets.EntityTemplates.Ship.UserShipEntityTemplate,
                Inherit = Assets.EntityTemplates.Ship.ShipEntityTemplate,
                Components = [
                    new UserId(),
                    new Awake(sleepingAllowed: false),
                    new Collision()
                    {
                        Categories = CollisionGroups.ShipCategories,
                        CollidesWith = CollisionGroups.ShipCollidesWith
                    },
                    new PhysicsBubble()
                    {
                        Enabled = true,
                        Radius = (Fix64)25
                    }
                ]
            });

            return builder;
        }
    }
}
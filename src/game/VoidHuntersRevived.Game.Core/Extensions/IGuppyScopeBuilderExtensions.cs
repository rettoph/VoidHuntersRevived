using Autofac;
using Guppy.Core.Common;
using Guppy.Core.Common.Extensions;
using Guppy.Core.Files.Common;
using Guppy.Core.Resources.Common.Configuration;
using Guppy.Core.Resources.Common.Extensions;
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
    public static class IGuppyScopeBuilderExtensions
    {
        public static IGuppyScopeBuilder RegisterGameCoreServices(this IGuppyScopeBuilder builder)
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

                builder.RegisterResourcePack(new ResourcePackConfiguration()
                {
                    EntryDirectory = DirectoryLocation.CurrentDirectory(VoidHuntersPack.Directory)
                });

                // Register core game resources
                builder.RegisterTeamEntityTemplates()
                    .RegisterPhysicsEntityTemplates()
                    .RegisterPiecesEntityTemplates()
                    .RegisterShipsEntityTemplates();
            });
        }

        private static IGuppyScopeBuilder RegisterTeamEntityTemplates(this IGuppyScopeBuilder builder)
        {
            builder.RegisterResource(Resources.EntityTemplates.Team.TeamEntityTemplate.Name, new EntityTemplateFragment()
            {
                Key = Resources.EntityTemplates.Team.TeamEntityTemplate,
                Flags = EntityTemplateFlagsEnum.Partial,
                RequiredComponents = [
                    typeof(Team),
                    typeof(ColorScheme)
                ]
            });

            builder.RegisterResource(Resources.EntityTemplates.Team.DefaultTeamEntityTemplate.Name, new EntityTemplateFragment()
            {
                Key = Resources.EntityTemplates.Team.DefaultTeamEntityTemplate,
                Flags = EntityTemplateFlagsEnum.Partial,
                Components = [
                    new DefaultTeam()
                ],
                RequiredComponents = [
                    typeof(Team)
                ]
            });

            builder.RegisterResource(Resources.EntityTemplates.Team.TeamMemberEntityTemplate.Name, new EntityTemplateFragment()
            {
                Key = Resources.EntityTemplates.Team.TeamMemberEntityTemplate,
                Flags = EntityTemplateFlagsEnum.Partial,
                Components = [
                    new TeamMember()
                ]
            });

            return builder;
        }

        private static IGuppyScopeBuilder RegisterPhysicsEntityTemplates(this IGuppyScopeBuilder builder)
        {
            builder.RegisterResource(Resources.EntityTemplates.Physics.BodyEntityTemplate.Name, new EntityTemplateFragment()
            {
                Key = Resources.EntityTemplates.Physics.BodyEntityTemplate,
                Flags = EntityTemplateFlagsEnum.Partial,
                Inherit = Resources.EntityTemplates.Team.TeamMemberEntityTemplate,
                Components = [
                    Body.Default,
                    new Enabled(),
                    new Awake(true)
                ],
                RequiredComponents = [
                    typeof(Collision)
                ]
            });

            builder.RegisterResource(Resources.EntityTemplates.Physics.FixtureEntityTemplate.Name, new EntityTemplateFragment()
            {
                Key = Resources.EntityTemplates.Physics.FixtureEntityTemplate,
                Flags = EntityTemplateFlagsEnum.Partial,
                Inherit = Resources.EntityTemplates.Team.TeamMemberEntityTemplate,
                Components = [
                    new Fixture()
                ]
            });

            return builder;
        }

        private static IGuppyScopeBuilder RegisterPiecesEntityTemplates(this IGuppyScopeBuilder builder)
        {
            builder.RegisterResource(Resources.EntityTemplates.Piece.TreeEntityTemplate.Name, new EntityTemplateFragment()
            {
                Key = Resources.EntityTemplates.Piece.TreeEntityTemplate,
                Flags = EntityTemplateFlagsEnum.Partial,
                Inherit = Resources.EntityTemplates.Physics.BodyEntityTemplate,
                Components = [
                    new Tree(),
                ]
            });

            builder.RegisterResource(Resources.EntityTemplates.Piece.PieceEntityTemplate.Name, new EntityTemplateFragment()
            {
                Key = Resources.EntityTemplates.Piece.PieceEntityTemplate,
                Flags = EntityTemplateFlagsEnum.Partial,
                Inherit = Resources.EntityTemplates.Physics.FixtureEntityTemplate,
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

            builder.RegisterResource(Resources.EntityTemplates.Piece.ThrusterEntityTemplate.Name, new EntityTemplateFragment()
            {
                Key = Resources.EntityTemplates.Piece.ThrusterEntityTemplate,
                Flags = EntityTemplateFlagsEnum.Partial,
                Inherit = Resources.EntityTemplates.Piece.PieceEntityTemplate,
                Components = [
                    new Thrustable()
                ]
            });

            builder.RegisterResource(Resources.EntityTemplates.Piece.HullEntityTemplate.Name, new EntityTemplateFragment()
            {
                Key = Resources.EntityTemplates.Piece.HullEntityTemplate,
                Flags = EntityTemplateFlagsEnum.Partial,
                Inherit = Resources.EntityTemplates.Piece.PieceEntityTemplate,
                RequiredComponents = [
                    typeof(Sockets)
                ]
            });

            return builder;
        }

        private static IGuppyScopeBuilder RegisterShipsEntityTemplates(this IGuppyScopeBuilder builder)
        {
            builder.RegisterResource(Resources.EntityTemplates.Ship.ChainEntityTemplate.Name, new EntityTemplateFragment()
            {
                Key = Resources.EntityTemplates.Ship.ChainEntityTemplate,
                Inherit = Resources.EntityTemplates.Piece.TreeEntityTemplate,
                Components = [
                    new Tractorable(),
                    new Collision()
                    {
                        Categories = CollisionGroups.FreeFloatingCategories,
                        CollidesWith = CollisionGroups.FreeFloatingCollidesWith
                    }
                ]
            });

            builder.RegisterResource(Resources.EntityTemplates.Ship.ShipEntityTemplate.Name, new EntityTemplateFragment()
            {
                Key = Resources.EntityTemplates.Ship.ShipEntityTemplate,
                Flags = EntityTemplateFlagsEnum.Partial,
                Inherit = Resources.EntityTemplates.Piece.TreeEntityTemplate,
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

            builder.RegisterResource(Resources.EntityTemplates.Ship.UserShipEntityTemplate.Name, new EntityTemplateFragment()
            {
                Key = Resources.EntityTemplates.Ship.UserShipEntityTemplate,
                Inherit = Resources.EntityTemplates.Ship.ShipEntityTemplate,
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
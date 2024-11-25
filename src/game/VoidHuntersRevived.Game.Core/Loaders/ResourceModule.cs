using Autofac;
using Guppy.Core.Common.Attributes;
using Guppy.Core.Resources.Common.Extensions.Autofac;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Domain.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Enums;
using VoidHuntersRevived.Domain.Graphics.Common.Components;
using VoidHuntersRevived.Domain.Physics.Common.Components;
using VoidHuntersRevived.Domain.Pieces.Common.Components;
using VoidHuntersRevived.Domain.Pieces.Common.Constants;
using VoidHuntersRevived.Domain.Pieces.Common.Graphics.Vertices;
using VoidHuntersRevived.Domain.Ships.Common.Components;
using VoidHuntersRevived.Domain.Teams.Common.Components;

namespace VoidHuntersRevived.Game.Core.Modules
{
    [AutoLoad]
    public class ResourceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            this.RegisterTeamEntityTemplates(builder);
            this.RegisterPhysicsEntityTemplates(builder);
            this.RegisterPiecesEntityTemplates(builder);
            this.RegisterShipsEntityTemplates(builder);
        }

        private void RegisterTeamEntityTemplates(ContainerBuilder builder)
        {
            builder.RegisterResource(Resources.EntityTemplates.Team.TeamEntityTemplate.Name, new EntityTemplateFragment()
            {
                Key = Resources.EntityTemplates.Team.TeamEntityTemplate,
                Flags = EntityTemplateFlags.Partial,
                RequiredComponents = [
                    typeof(Team),
                    typeof(ColorScheme)
                ]
            });

            builder.RegisterResource(Resources.EntityTemplates.Team.DefaultTeamEntityTemplate.Name, new EntityTemplateFragment()
            {
                Key = Resources.EntityTemplates.Team.DefaultTeamEntityTemplate,
                Flags = EntityTemplateFlags.Partial,
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
                Flags = EntityTemplateFlags.Partial,
                Components = [
                    new TeamMember()
                ]
            });
        }

        private void RegisterPhysicsEntityTemplates(ContainerBuilder builder)
        {
            builder.RegisterResource(Resources.EntityTemplates.Physics.BodyEntityTemplate.Name, new EntityTemplateFragment()
            {
                Key = Resources.EntityTemplates.Physics.BodyEntityTemplate,
                Flags = EntityTemplateFlags.Partial,
                Inherit = Resources.EntityTemplates.Team.TeamMemberEntityTemplate,
                Components = [
                    new Location(),
                    new Enabled(),
                    new Awake(true)
                ],
                RequiredComponents = [
                    typeof(Collision)
                ]
            });
        }

        private void RegisterPiecesEntityTemplates(ContainerBuilder builder)
        {
            builder.RegisterResource(Resources.EntityTemplates.Piece.TreeEntityTemplate.Name, new EntityTemplateFragment()
            {
                Key = Resources.EntityTemplates.Piece.TreeEntityTemplate,
                Flags = EntityTemplateFlags.Partial,
                Inherit = Resources.EntityTemplates.Physics.BodyEntityTemplate,
                Components = [
                    new Tree(),
                ]
            });

            builder.RegisterResource(Resources.EntityTemplates.Piece.PieceEntityTemplate.Name, new EntityTemplateFragment()
            {
                Key = Resources.EntityTemplates.Piece.PieceEntityTemplate,
                Flags = EntityTemplateFlags.Partial,
                Inherit = Resources.EntityTemplates.Team.TeamMemberEntityTemplate,
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
                Flags = EntityTemplateFlags.Partial,
                Inherit = Resources.EntityTemplates.Piece.PieceEntityTemplate,
                Components = [
                    new Thrustable()
                ]
            });

            builder.RegisterResource(Resources.EntityTemplates.Piece.HullEntityTemplate.Name, new EntityTemplateFragment()
            {
                Key = Resources.EntityTemplates.Piece.HullEntityTemplate,
                Flags = EntityTemplateFlags.Partial,
                Inherit = Resources.EntityTemplates.Piece.PieceEntityTemplate,
                RequiredComponents = [
                    typeof(Sockets)
                ]
            });
        }

        private void RegisterShipsEntityTemplates(ContainerBuilder builder)
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
                Flags = EntityTemplateFlags.Partial,
                Inherit = Resources.EntityTemplates.Piece.TreeEntityTemplate,
                Components = [
                    new PhysicsBubble() {
                        Enabled = false,
                        Radius = default
                    },
                    new Helm(),
                    new Tactical(),
                    new TractorBeamEmitter()
                    {
                        Active = false
                    }
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
        }
    }
}

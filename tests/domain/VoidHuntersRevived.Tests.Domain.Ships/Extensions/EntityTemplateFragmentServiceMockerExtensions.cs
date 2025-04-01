using Guppy.Core.Resources.Common;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Domain.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Physics.Common.Components;
using VoidHuntersRevived.Domain.Pieces.Common;
using VoidHuntersRevived.Domain.Pieces.Common.Components;
using VoidHuntersRevived.Domain.Pieces.Common.Constants;
using VoidHuntersRevived.Domain.Ships.Common.Components;
using VoidHuntersRevived.Domain.Teams.Common.Components;
using VoidHuntersRevived.Tests.Common.Entities.Mockers;

namespace VoidHuntersRevived.Tests.Domain.Ships.Extensions
{
    public static class EntityTemplateFragmentServiceMockerExtensions
    {
        public static void RegisterTestShipEntityTemplateFragments(this EntityTemplateFragmentServiceMocker entityTemplateFragmentServiceMocker)
        {
            entityTemplateFragmentServiceMocker.EntityTemplateFragments.AddRange([
                new EntityTemplateFragment()
                {
                    Key = Key<IEntityTemplate>.GetByName("Test.DefaultTeam"),
                    Components = [
                        new DefaultTeam(),
                        new Team(ResourceKey<string>.Get("Test.DefaultTeam"))
                    ]
                },
                new EntityTemplateFragment()
                {
                    Key = Key<IEntityTemplate>.GetByName("Test.Team"),
                    Components = [
                        new Team(ResourceKey<string>.Get("Test.Team")),
                        new ColorScheme(TestResources.Colors.TestColor, TestResources.Colors.TestColor)
                    ]
                },
                new EntityTemplateFragment()
                {
                    Key = Resources.EntityTemplates.Ship.ChainEntityTemplate,
                    Components = [
                        new TeamMember(),
                        new Body(),
                        new Enabled(),
                        new Awake(true),
                        new Collision()
                        {
                            Categories = CollisionGroups.FreeFloatingCategories,
                            CollidesWith = CollisionGroups.FreeFloatingCollidesWith
                        },
                        new Tree(),
                        new Tractorable(),
                    ]
                },
                new EntityTemplateFragment()
                {
                    Key = TestResources.TestSquareEntityTemplateKey,
                    Components = [
                        new TeamMember(),
                        Plug.Default,
                        new Coupling(),
                        new Node(),
                        new Fixture(),
                        new ColorScheme(TestResources.Colors.TestColor, TestResources.Colors.TestColor),
                        new Rigid(TestResources.BodyTemplates.TestSquareBodyTemplate),
                        new Sockets()
                        {
                            Items = new Socket[] {
                                new(new FixTransform2D(new FixVector2(1, 0.5), Fix64.Zero))
                            }.ToNativeDynamicArray()
                        }
                    ]
                },
                new EntityTemplateFragment()
                {
                    Key = Resources.EntityTemplates.Ship.UserShipEntityTemplate,
                    Components = [
                        new TeamMember(),
                        new Body(),
                        new Enabled(),
                        new Tree(),
                        new Helm(),
                        new Tactical(),
                        new TractorBeamEmitter(default),
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
                }
            ]);
        }
    }
}

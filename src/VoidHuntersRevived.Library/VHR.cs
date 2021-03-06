﻿using Guppy.Contexts;
using Guppy.DependencyInjection;
using Guppy.Extensions.System;
using Guppy.LayerGroups;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;
using tainicom.Aether.Physics2D.Dynamics;
using VoidHuntersRevived.Library.Services.SpellCasts;
using VoidHuntersRevived.Library.Services.SpellCasts.AmmunitionSpellCasts;

namespace VoidHuntersRevived.Library
{
    public static class VHR
    {
        public static class Entities
        {
            public static readonly String Hull = "entity:ship-part:hull";
            public static readonly String Thruster = "entity:ship-part:thruster";
            public static readonly String Gun = "entity:ship-part:weapon:gun";
            public static readonly String Laser = "entity:ship-part:weapon:laser";
            public static readonly String Armor = "entity:ship-part:armor";
            public static readonly String DroneBay = "entity:ship-part:drone-bay";
            public static readonly String ShieldGenerator = "entity:ship-part:shield-generator";
            public static readonly String PowerCell = "entity:ship-part:power-cell";
        }
        
        public static class Actions
        {
            public static class Ship
            {
                public static readonly UInt32 TryLaunchDrones = "action:ship:try-launch-drones".xxHash();
                public static readonly UInt32 OnLaunchDrones = "action:ship:on-launch-drones".xxHash();

                public static readonly UInt32 TryToggleEnergyShields = "action:ship:try-toggle-energy-shields".xxHash();
                public static readonly UInt32 OnToggleEnergyShields = "action:ship:on-toggle-energy-shields".xxHash();
            }
        }

        public static class Network
        {
            public static class MessageData
            {
                public static class DefaultDirtyUpdate
                {
                    public static readonly Int32 SequenceChannel = 0;
                    public static readonly NetDeliveryMethod NetDeliveryMethod = NetDeliveryMethod.ReliableUnordered;
                }

                public static class ShipPart
                {
                    public static class UpdateHealthPing
                    {
                        public static readonly Int32 SequenceChannel = 15;
                        public static readonly NetDeliveryMethod NetDeliveryMethod = NetDeliveryMethod.ReliableOrdered;
                    }
                }

                public static class Ship
                {
                    public static class DirtyUpdate
                    {
                        public static readonly Int32 SequenceChannel = 10;
                        public static readonly NetDeliveryMethod NetDeliveryMethod = NetDeliveryMethod.ReliableUnordered;
                    }

                    public static class ActionPing
                    {
                        public static readonly Int32 SequenceChannel = 0;
                        public static readonly NetDeliveryMethod NetDeliveryMethod = NetDeliveryMethod.ReliableUnordered;
                    }
                }
            }

            public static class Pings
            {
                public static class Scene
                {
                    public static readonly UInt32 Setup = "scene:setup".xxHash();
                    public static readonly UInt32 Entity = "scene:entity".xxHash();
                }

                public static class World
                {
                    public static readonly UInt32 UpdateSize = "world:update:size".xxHash();
                }

                public static class ShipPart
                {
                    public static readonly UInt32 UpdateHealth = "ship-part:update:health".xxHash();
                }

                public static class SpellCasterPart
                {
                    public static readonly UInt32 Cast = "spell-caster-part:cast".xxHash();
                }

                public static class Ship
                {
                    public static readonly UInt32 Action = "ship:action".xxHash();

                    public static readonly UInt32 UpdateTargetRequest = "ship:update:target:request".xxHash();

                    public static readonly UInt32 UpdateDirection = "ship:update:direction".xxHash();
                    public static readonly UInt32 UpdateDirectionRequest = "ship:update:direction:request".xxHash();

                    public static readonly UInt32 UpdateFiring = "ship:update:firing".xxHash();
                    public static readonly UInt32 UpdateFiringRequest = "ship:update:firing:request".xxHash();

                    public static readonly UInt32 LaunchDrones = "ship:launch-drones".xxHash();
                    public static readonly UInt32 LaunchDronesRequest = "ship:launch-drones:request".xxHash();

                    public static readonly UInt32 ToggleEnergyShields = "ship:toggle-energy-shields".xxHash();
                    public static readonly UInt32 ToggleEnergyShieldsRequest = "ship:toggle-energy-shields:request".xxHash();

                    public static readonly UInt32 UpdateBridge = "ship:update:bridge".xxHash();

                    public static readonly UInt32 SpawnRequest = "ship:spawn:request".xxHash();

                    public static readonly UInt32 SpawnAiRequest = "ship:spawn:ai:request".xxHash();

                    public static readonly UInt32 SelfDestructRequest = "ship:self-destruct:request".xxHash();

                    public static class TractorBeam
                    {
                        public static readonly UInt32 Action = "ship:tractor-beam:action".xxHash();
                        public static readonly UInt32 ActionRequest = "ship:tractor-beam:action:request".xxHash();
                    }
                }
            }
        }

        public static class Categories
        {
            public static readonly Category BorderCollisionCategories = Category.Cat1;
            public static readonly Category PassiveCollisionCategories = Category.Cat2;
            public static readonly Category ActiveCollisionCategories = Category.Cat3;
            public static readonly Category FighterCollisionCategories = Category.Cat4;


            public static readonly Category BorderCollidesWith = Categories.PassiveCollisionCategories | Categories.ActiveCollisionCategories | Categories.FighterCollisionCategories;
            public static readonly Category PassiveCollidesWith = Categories.BorderCollisionCategories;
            public static readonly Category ActiveCollidesWith = Categories.BorderCollisionCategories | Categories.ActiveCollisionCategories;
            public static readonly Category FighterCollidesWith = Categories.BorderCollisionCategories | Categories.FighterCollisionCategories;
        }

        public static class Directories
        {
            public static class Resources
            {
                public static readonly String ShipParts = "Resources/ShipParts";
                public static readonly String Ships = "Resources/Ships";
            }
        }

        public static class Utilities
        {
            public static readonly Single SlaveLerpPerSecond = 1f;
        }

        public static class LayersContexts
        {
            public static readonly LayerContext World = new LayerContext()
            {
                DrawOrder = 00,
                Group = new SingleLayerGroup(1)
            };

            public static readonly LayerContext Player = new LayerContext()
            {
                DrawOrder = 10,
                Group = new SingleLayerGroup(2)
            };

            public static readonly LayerContext Chunk = new LayerContext()
            {
                DrawOrder = 20,
                Group = new SingleLayerGroup(3)
            };

            public static readonly LayerContext Trail = new LayerContext()
            {
                DrawOrder = 30,
                Group = new SingleLayerGroup(4)
            };

            public static readonly LayerContext Ship = new LayerContext()
            {
                DrawOrder = 40,
                Group = new SingleLayerGroup(5)
            };

            public static readonly LayerContext TractorBeam = new LayerContext()
            {
                DrawOrder = 50,
                Group = new SingleLayerGroup(6)
            };

            public static readonly LayerContext Spell = new LayerContext()
            {
                DrawOrder = 60,
                Group = new SingleLayerGroup(7)
            };

            public static readonly LayerContext Explosion = new LayerContext()
            {
                DrawOrder = 70,
                Group = new SingleLayerGroup(8)
            };

            public static readonly LayerContext HeadsUpDisplay = new LayerContext()
            {
                DrawOrder = 80,
                Group = new SingleLayerGroup(9)
            };

            public static readonly LayerContext UI = new LayerContext()
            {
                DrawOrder = 90,
                Group = new SingleLayerGroup(10)
            };
        }

        public static class SpellCasts
        {
            public static readonly UInt32 LaunchDronesSpellCast = ServiceConfiguration.GetId<LaunchDroneSpellCast>();
            public static readonly UInt32 BulletSpellCast = ServiceConfiguration.GetId<BulletSpellCast>();
            public static readonly UInt32 LaserBeamSpellCast = ServiceConfiguration.GetId<LaserBeamSpellCast>();
            public static readonly UInt32 EnergyShieldSpellCast = ServiceConfiguration.GetId<EnergyShieldSpellCast>();
        }
    }
}

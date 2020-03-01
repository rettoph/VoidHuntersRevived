using Guppy.Attributes;
using Guppy.Collections;
using Guppy.Extensions.DependencyInjection;
using Guppy.Factories;
using Guppy.Interfaces;
using Guppy.Loaders;
using Guppy.UI.Entities;
using Guppy.UI.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Client.Library.Drivers.Entities.Controllers;
using VoidHuntersRevived.Client.Library.Entities;
using VoidHuntersRevived.Client.Library.Entities.UI;
using VoidHuntersRevived.Client.Library.Utilities;
using VoidHuntersRevived.Client.Library.Utilities.Cameras;

namespace VoidHuntersRevived.Client.Library
{
    [IsServiceLoader]
    public class ClientServiceLoader : IServiceLoader
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<SpriteManager>();
            services.AddScoped<ServerShadow>();
            services.AddScoped<DebugOverlay, InitializableFactory<DebugOverlay>>();
            services.AddScoped<Sensor>("entity:sensor");
            services.AddScoped<TrailManager>("entity:trail-manager");
            services.AddScoped<PopupManager>("entity:popup-manager");
            services.AddScoped<Hud>(p => p.GetRequiredService<EntityCollection>().Create<Hud>());
        }

        public void ConfigureProvider(IServiceProvider provider)
        {
            // Run custom global setup methods
            ChunkTextureDriver.Setup(provider);

            var content = provider.GetRequiredService<ContentLoader>();

            content.TryRegister("font", "Font");
            content.TryRegister("font:ui:title", "Fonts/BiomeBold-Big");
            content.TryRegister("font:ui:title-light", "Fonts/BiomeLight-Big");
            content.TryRegister("font:ui:label", "Fonts/Biome-Normal");
            content.TryRegister("font:ui:input", "Fonts/BiomeLight-Normal");
            content.TryRegister("font:small", "Fonts/BiomeLight-Small");

            content.TryRegister("sprite:background:1", "Sprites/background-1");
            content.TryRegister("sprite:background:2", "Sprites/background-2");
            content.TryRegister("sprite:background:3", "Sprites/background-3");

            content.TryRegister("sprite:logo", "Sprites/icon2alpha");

            content.TryRegister("icon:save", "Sprites/icon_save");

            #region Register ShipPart Textures
            content.TryRegister("texture:entity:ship-part:hull:triangle", "Sprites/entity_ship-part_hull_triangle");
            content.TryRegister("texture:entity:ship-part:hull:square", "Sprites/entity_ship-part_hull_square");
            content.TryRegister("texture:entity:ship-part:hull:pentagon", "Sprites/entity_ship-part_hull_pentagon");
            content.TryRegister("texture:entity:ship-part:hull:hexagon", "Sprites/entity_ship-part_hull_hexagon");

            content.TryRegister("texture:entity:ship-part:chassis:mosquito", "Sprites/entity_ship-part_chassis_mosquito");

            content.TryRegister("texture:entity:ship-part:thruster:small", "Sprites/entity_ship-part_thruster_small");

            content.TryRegister("texture:entity:ship-part:weapon:mass-driver", "Sprites/entity_ship-part_weapon_mass-driver");
            content.TryRegister("texture:entity:ammunition:projectile:mass-driver", "Sprites/entity_ammunition_projectile_mass-driver");
            #endregion

            #region Register UI Elements
            var configurations = provider.GetRequiredService<ConfigurationLoader>();

            configurations.TryRegister<Button>("hud:button", b =>
            {
                b.BorderColor = new Color(0, 143, 241);
                b.BorderSize = 1;
                b.BackgroundTransform = Color.White;
                b.BackgroundColor = new Color(0, 0, 0, 50);
                b.Color = Color.White;
                b.Font = content.TryGet<SpriteFont>("font:ui:input");

                b.OnHoveredChanged += (s, hovered) =>
                {
                    if(!b.Buttons.HasFlag(Pointer.Button.Left))
                        b.BackgroundColor = hovered ? new Color(25, 25, 40, 50) : new Color(0, 0, 0, 50);
                };
                b.OnButtonPressed += (s, buttons) =>
                {
                    if (buttons == Pointer.Button.Left)
                        b.BackgroundColor = new Color(30, 30, 50, 50);
                };
                b.OnButtonReleased += (s, buttons) =>
                {
                    if (buttons == Pointer.Button.Left)
                        b.BackgroundColor = b.Hovered ? new Color(25, 25, 40, 50) : new Color(0, 0, 0, 50);
                };
            });
            #endregion
        }
    }
}

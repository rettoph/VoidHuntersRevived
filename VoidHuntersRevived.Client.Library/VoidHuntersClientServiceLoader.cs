using Guppy.Collections;
using Guppy.Extensions.DependencyInjection;
using Guppy.Interfaces;
using Guppy.Loaders;
using Guppy.UI.Elements;
using Guppy.UI.Enums;
using Guppy.UI.Loaders;
using Guppy.UI.Styles;
using Guppy.UI.Utilities.Units;
using Guppy.UI.Utilities.Units.UnitValues;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Client.Library.Drivers;
using VoidHuntersRevived.Client.Library.Entities;
using VoidHuntersRevived.Client.Library.Layers;
using VoidHuntersRevived.Client.Library.Scenes;
using VoidHuntersRevived.Client.Library.UI;
using VoidHuntersRevived.Client.Library.Utilities;
using VoidHuntersRevived.Client.Library.Utilities.Cameras;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Entities.ShipParts;

namespace VoidHuntersRevived.Client.Library
{
    public class VoidHuntersClientServiceLoader : IServiceLoader
    {
        public void ConfigureServiceCollection(IServiceCollection services)
        {
            services.AddScoped<FarseerCamera2D>();

            services.AddGame<VoidHuntersClientGame>();
            services.AddScene<VoidHuntersClientWorldScene>();
            services.AddScene<LobbyScene>();
            services.AddLayer<CameraLayer>();
            services.AddLayer<HudLayer>();

            services.AddDriver<UserPlayer, ClientUserPlayerDriver>();
            services.AddDriver<Pointer, MousePointerDriver>();
            services.AddDriver<VoidHuntersClientWorldScene, VoidHuntersClientWorldSceneDriver>(96);
            services.AddDriver<Ship, ClientShipDriver>();
            services.AddDriver<FarseerEntity, ClientFarseerEntityDriver>();
            services.AddDriver<ShipPart, ClientShipPartDriver>();

            services.AddScoped<ServerRender>();
            services.AddScoped<Pointer>(p => {
                var pointer = p.GetRequiredService<EntityCollection>().Create<Pointer>("entity:pointer");
                pointer.SetUpdateOrder(0);
                pointer.SetLayerDepth(0);

                return pointer;
            });
        }

        public void Boot(IServiceProvider provider)
        {
            var contentLoader = provider.GetLoader<ContentLoader>();
            contentLoader.Register("font:ui", "font");
            contentLoader.Register("texture:ui:text-area", "Sprites/text-area");
            contentLoader.Register("texture:connection-node:male", "Sprites/male-connection-node");
            contentLoader.Register("texture:connection-node:female", "Sprites/female-connection-node");

            var styleLoader = provider.GetLoader<StyleLoader>();
            styleLoader.Register(typeof(TextInput).FullName, new Style());
            styleLoader.Register(typeof(ChatItem).FullName, new Style());

            var entityLoader = provider.GetLoader<EntityLoader>();

            entityLoader.Register<Pointer>("entity:pointer", "name:entity:pointer", "description:entity:pointer");
        }

        public void PreInitialize(IServiceProvider provider)
        {
            var contentLoader = provider.GetLoader<ContentLoader>();
            var styleLoader = provider.GetLoader<StyleLoader>();

            var textElementStyle = styleLoader.GetValue(typeof(TextInput).FullName);
            textElementStyle.Set<SpriteFont>(ElementState.Normal, StateProperty.Font, contentLoader.Get<SpriteFont>("font:ui"));
            textElementStyle.Set<Color>(StateProperty.TextColor, Color.Black);
            textElementStyle.Set<Texture2D>(StateProperty.Background, contentLoader.Get<Texture2D>("texture:ui:text-area"));
            textElementStyle.Set<Alignment>(StateProperty.TextAlignment, Alignment.CenterLeft);
            textElementStyle.Set<UnitValue>(GlobalProperty.PaddingLeft, 7);
            textElementStyle.Set<UnitValue>(GlobalProperty.PaddingRight, 7);

            var chatItemStyle = styleLoader.GetValue(typeof(ChatItem).FullName);
            chatItemStyle.Set<SpriteFont>(ElementState.Normal, StateProperty.Font, contentLoader.Get<SpriteFont>("font:ui"));
            chatItemStyle.Set<Alignment>(StateProperty.TextAlignment, Alignment.CenterLeft);
            chatItemStyle.Set<UnitValue>(GlobalProperty.PaddingTop, 3);
            chatItemStyle.Set<UnitValue>(GlobalProperty.PaddingLeft, 7);
            chatItemStyle.Set<UnitValue>(GlobalProperty.PaddingBottom, 3);
            chatItemStyle.Set<UnitValue>(GlobalProperty.PaddingRight, 7);
        }

        public void Initialize(IServiceProvider provider)
        {
            // throw new NotImplementedException();
        }

        public void PostInitialize(IServiceProvider provider)
        {
            // throw new NotImplementedException();
        }
    }
}

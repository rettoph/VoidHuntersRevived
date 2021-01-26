using Guppy.Attributes;
using Guppy.DependencyInjection;
using Guppy.Interfaces;
using Guppy.Services;
using Guppy.UI.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Builder.UI.Pages;
using Guppy.Extensions.DependencyInjection;
using Guppy.UI.Enums;
using VoidHuntersRevived.Builder.UI;
using VoidHuntersRevived.Builder.UI.Inputs;

namespace VoidHuntersRevived.Builder.ServiceLoaders
{
    [AutoLoad]
    class UIServiceLoader : IServiceLoader
    {
        public void RegisterServices(ServiceCollection services)
        {
            services.AddFactory<ShipPartContextTypeSelectorPage>(p => new ShipPartContextTypeSelectorPage());
            services.AddFactory<ShipPartContextBuilderPage>(p => new ShipPartContextBuilderPage());
            services.AddFactory<ShipPartShapesBuilderPage>(p => new ShipPartShapesBuilderPage());
            services.AddFactory<ShipPartPropertiesEditorPage>(p => new ShipPartPropertiesEditorPage());
            services.AddFactory<ContextTypeButton>(p => new ContextTypeButton());
            services.AddFactory<ShapeEditorMenu>(p => new ShapeEditorMenu());
            services.AddFactory<ConnectionNodeEditorMenu>(p => new ConnectionNodeEditorMenu());
            services.AddFactory<SideContextInput>(p => new SideContextInput());
            services.AddFactory<ShapeTransformationsInput>(p => new ShapeTransformationsInput());
            services.AddFactory<ContextPropertyInput>(p => new ContextPropertyInput());
            services.AddFactory<StringInput>(p => new StringInput());
            services.AddFactory<SingleInput>(p => new SingleInput());
            services.AddFactory<RadianInput>(p => new RadianInput());
            services.AddFactory<ColorInput>(p => new ColorInput());
            services.AddFactory<BooleanInput>(p => new BooleanInput());
            services.AddFactory<Vector2Input>(p => new Vector2Input());

            services.AddScoped<ShipPartContextTypeSelectorPage>();
            services.AddScoped<ShipPartContextBuilderPage>();
            services.AddTransient<ShipPartShapesBuilderPage>();
            services.AddTransient<ShipPartPropertiesEditorPage>();
            services.AddTransient<ContextTypeButton>();
            services.AddTransient<ShapeEditorMenu>();
            services.AddTransient<ConnectionNodeEditorMenu>();
            services.AddTransient<SideContextInput>();
            services.AddTransient<ShapeTransformationsInput>();
            services.AddTransient<ContextPropertyInput>();
            services.AddTransient<StringInput>();
            services.AddTransient<SingleInput>();
            services.AddTransient<RadianInput>();
            services.AddTransient<ColorInput>();
            services.AddTransient<BooleanInput>();
            services.AddTransient<Vector2Input>();

            services.AddSetup<ColorService>((colors, p, c) =>
            {
                var opacity = 0.5f;
                colors.TryRegister("ui:color:0", new Color(Color.Black, opacity));
                colors.TryRegister("ui:color:1", new Color(Color.White, opacity));
                colors.TryRegister("ui:color:2", new Color(Color.Gray, opacity));
                colors.TryRegister("ui:color:3", new Color(Color.Lerp(Color.Gray, Color.Black, 0.2f), opacity));
                colors.TryRegister("ui:color:4", new Color(Color.Lerp(Color.Gray, Color.Black, 0.3f), opacity));
                colors.TryRegister("ui:color:5", new Color(Color.Lerp(Color.Gray, Color.Black, 0.4f), opacity));
            });

            services.AddSetup<TextElement>((button, p, c) =>
            {
                button.Font = p.GetContent<SpriteFont>("debug:font");
            });

            services.AddSetup<TextElement>("ui:button", (button, p, c) =>
            {
                button.Inline = InlineType.None;
                button.Alignment = Alignment.CenterCenter;
                button.Bounds.Width = 200;
                button.Bounds.Height = 35;
            });

            services.AddTransient<TextElement>("ui:button:0");
            services.AddSetup<TextElement>("ui:button:0", (button, p, c) =>
            {
                button.Color[ElementState.Default] = p.GetColor("ui:color:1");
                button.BorderWidth[ElementState.Default] = 1;
                button.BorderColor[ElementState.Default] = p.GetColor("ui:color:1");
                button.BackgroundColor[ElementState.Default] = p.GetColor("ui:color:2");
                button.BackgroundColor[ElementState.Hovered] = p.GetColor("ui:color:3");
                button.BackgroundColor[ElementState.Pressed] = p.GetColor("ui:color:4");
            });
        }

        public void ConfigureProvider(ServiceProvider provider)
        {
            // throw new NotImplementedException();
        }
    }
}

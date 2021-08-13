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
            services.RegisterTypeFactory<ShipPartContextSelectorPage>(p => new ShipPartContextSelectorPage());
            services.RegisterTypeFactory<ShipPartContextBuilderPage>(p => new ShipPartContextBuilderPage());
            services.RegisterTypeFactory<ShipPartShapesBuilderPage>(p => new ShipPartShapesBuilderPage());
            services.RegisterTypeFactory<ShipPartPropertiesEditorPage>(p => new ShipPartPropertiesEditorPage());
            services.RegisterTypeFactory<ContextTypeButton>(p => new ContextTypeButton());
            services.RegisterTypeFactory<ShapeEditorMenu>(p => new ShapeEditorMenu());
            services.RegisterTypeFactory<ConnectionNodeEditorMenu>(p => new ConnectionNodeEditorMenu());
            services.RegisterTypeFactory<SideContextInput>(p => new SideContextInput());
            services.RegisterTypeFactory<ShapeTransformationsInput>(p => new ShapeTransformationsInput());
            services.RegisterTypeFactory<ContextPropertyInput>(p => new ContextPropertyInput());
            services.RegisterTypeFactory<StringInput>(p => new StringInput());
            services.RegisterTypeFactory<SingleInput>(p => new SingleInput());
            services.RegisterTypeFactory<RadianInput>(p => new RadianInput());
            services.RegisterTypeFactory<ColorInput>(p => new ColorInput());
            services.RegisterTypeFactory<BooleanInput>(p => new BooleanInput());
            services.RegisterTypeFactory<Vector2Input>(p => new Vector2Input());

            services.RegisterScoped<ShipPartContextSelectorPage>();
            services.RegisterScoped<ShipPartContextBuilderPage>();
            services.RegisterTransient<ShipPartShapesBuilderPage>();
            services.RegisterTransient<ShipPartPropertiesEditorPage>();
            services.RegisterTransient<ContextTypeButton>();
            services.RegisterTransient<ShapeEditorMenu>();
            services.RegisterTransient<ConnectionNodeEditorMenu>();
            services.RegisterTransient<SideContextInput>();
            services.RegisterTransient<ShapeTransformationsInput>();
            services.RegisterTransient<ContextPropertyInput>();
            services.RegisterTransient<StringInput>();
            services.RegisterTransient<SingleInput>();
            services.RegisterTransient<RadianInput>();
            services.RegisterTransient<ColorInput>();
            services.RegisterTransient<BooleanInput>();
            services.RegisterTransient<Vector2Input>();

            services.RegisterSetup<ColorService>((colors, p, c) =>
            {
                var opacity = 0.5f;
                colors.TryRegister("ui:color:0", new Color(Color.Black, opacity));
                colors.TryRegister("ui:color:1", new Color(Color.White, opacity));
                colors.TryRegister("ui:color:2", new Color(Color.Gray, opacity));
                colors.TryRegister("ui:color:3", new Color(Color.Lerp(Color.Gray, Color.Black, 0.2f), opacity));
                colors.TryRegister("ui:color:4", new Color(Color.Lerp(Color.Gray, Color.Black, 0.3f), opacity));
                colors.TryRegister("ui:color:5", new Color(Color.Lerp(Color.Gray, Color.Black, 0.4f), opacity));
            });

            services.RegisterSetup<TextElement>((button, p, c) =>
            {
                button.Font = p.GetContent<SpriteFont>("debug:font");
            });

            services.RegisterSetup<TextElement>("ui:button", (button, p, c) =>
            {
                button.Inline = InlineType.None;
                button.Alignment = Alignment.CenterCenter;
                button.Bounds.Width = 200;
                button.Bounds.Height = 35;
            });

            services.RegisterTransient<TextElement>("ui:button:0");
            services.RegisterSetup<TextElement>("ui:button:0", (button, p, c) =>
            {
                button.Color[ElementState.Default] = p.GetColor("ui:color:1");
                button.BorderWidth[ElementState.Default] = 1;
                button.BorderColor[ElementState.Default] = p.GetColor("ui:color:1");
                button.BackgroundColor[ElementState.Default] = p.GetColor("ui:color:2");
                button.BackgroundColor[ElementState.Hovered] = p.GetColor("ui:color:3");
                button.BackgroundColor[ElementState.Pressed] = p.GetColor("ui:color:4");
            });
        }

        public void ConfigureProvider(GuppyServiceProvider provider)
        {
            // throw new NotImplementedException();
        }
    }
}

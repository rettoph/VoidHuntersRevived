using Guppy;
using Guppy.DependencyInjection;
using Guppy.UI.Elements;
using Guppy.UI.Entities;
using Guppy.UI.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using VoidHuntersRevived.Builder.UI;
using VoidHuntersRevived.Client.Library.Scenes;
using VoidHuntersRevived.Client.Library.UI;
using VoidHuntersRevived.Library.Scenes;
using Guppy.Extensions.DependencyInjection;
using tainicom.Aether.Physics2D;
using VoidHuntersRevived.Library.Contexts;
using VoidHuntersRevived.Library.Entities.ShipParts;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Library.Entities;
using Guppy.UI.Interfaces;
using Guppy.UI.Utilities.Units;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace VoidHuntersRevived.Builder.Scenes
{
    public class ShipPartBuilderScene : GraphicsGameScene
    {
        struct ShapeDefinition
        {
            public SideInput[] Sides;
            public ShapeTranslation Translation;
        }

        #region Private Fields
        private List<ShapeDefinition> _shapes;
        public ShipPart _demo;
        private WorldEntity _world;
        private StackContainer _container;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            provider.Service(out _world);

            this.camera.ZoomTo(100);
            this.camera.MoveTo(_world.Size / 2);
            _shapes = new List<ShapeDefinition>();

            _container = this.stage.Content.Children.Create<StackContainer>((container, p, c) =>
            {
                container.Bounds.X = 0;
                container.Bounds.Y = 0;
                container.Bounds.Width = 1f;
                container.Bounds.Height = 1f;
                container.Alignment = StackAlignment.Horizontal;
            });

            this.stage.Content.Children.Create<TextElement>((button, p, c) =>
            {
                button.Color[ElementState.Default] = Color.White;
                button.BackgroundColor[ElementState.Default] = p.GetColor("ui:input:color:2");
                button.BackgroundColor[ElementState.Hovered] = Color.Lerp(p.GetColor("ui:input:color:2"), Color.Black, 0.25f);
                button.BackgroundColor[ElementState.Pressed] = Color.Lerp(p.GetColor("ui:input:color:2"), Color.Black, 0.5f);
                button.Inline = InlineType.None;
                button.Bounds.Width = 150;
                button.Bounds.Height = 45;
                button.Bounds.X = new CustomUnit(c => (c - button.Bounds.Width.ToPixel(c)));
                button.Bounds.Y = new CustomUnit(c => (c - button.Bounds.Height.ToPixel(c)));
                button.Alignment = Alignment.CenterCenter;
                button.Font = p.GetContent<SpriteFont>("font:ui:normal");
                button.Value = "Add Shape";
                button.OnClicked += (s) => this.AddShape();
            });

            this.stage.Content.Children.Create<TextElement>((button, p, c) =>
            {
                button.Color[ElementState.Default] = Color.White;
                button.BackgroundColor[ElementState.Default] = p.GetColor("ui:input:color:2");
                button.BackgroundColor[ElementState.Hovered] = Color.Lerp(p.GetColor("ui:input:color:2"), Color.Black, 0.25f);
                button.BackgroundColor[ElementState.Pressed] = Color.Lerp(p.GetColor("ui:input:color:2"), Color.Black, 0.5f);
                button.Inline = InlineType.None;
                button.Bounds.Width = 150;
                button.Bounds.Height = 45;
                button.Bounds.X = new CustomUnit(c => (c - button.Bounds.Width.ToPixel(c)));
                button.Bounds.Y = new CustomUnit(c => (c - button.Bounds.Height.ToPixel(c) - button.Bounds.Height.ToPixel(c) - 10));
                button.Alignment = Alignment.CenterCenter;
                button.Font = p.GetContent<SpriteFont>("font:ui:normal");
                button.Value = "Export";
                button.OnClicked += (s) =>
                {
                    String output = "";
                    output += "var config = new ShipPartConfiguration();\n";
                    foreach (ShapeDefinition shape in _shapes)
                    {
                        for (var i = 0; i < shape.Sides.Length; i++)
                        {
                            if (shape.Sides[i].Angle.Input.Value != "")
                            {
                                output += $"config.AddSide("
                                    + $"MathHelper.ToRadians({Single.Parse(shape.Sides[i].Angle.Input.Value)}), "
                                    + $"ShipPartConfiguration.NodeType.None, "
                                    + $"{(shape.Sides[i].Length.Input.Value == String.Empty ? 0 : Single.Parse(shape.Sides[i].Length.Input.Value))});\n";
                            }
                        }

                        output += $"config.Rotate(MathHelper.ToRadians({Single.Parse(shape.Translation.R.Input.Value)}));\n";
                        output += $"config.Transform(Matrix.CreateTranslation(new Vector3({Single.Parse(shape.Translation.X.Input.Value)}, {Single.Parse(shape.Translation.Y.Input.Value)}, 0)));\n";
                        output += $"config.Flush();\n";

                        Directory.CreateDirectory("Exports");

                        using(var file = File.Open($"Exports/export_{_shapes.Count}-shapes_{DateTime.Now.ToString("yyyyMM-dd_hh-mm-ss-tt")}.txt", FileMode.OpenOrCreate))
                        {
                            using(StreamWriter writer = new StreamWriter(file))
                            {
                                writer.Write(output);
                            }
                        }

                        this.logger.Info(output);
                    }

                };
            });
        }

        private void AddShape()
        {
            var sides = new SideInput[Settings.MaxPolygonVertices - 1];
            ShapeTranslation translation = null;
            _container.Children.Create<StackContainer>((container, p, c) =>
            {
                container.Bounds.X = 0;
                container.Bounds.Y = 0;
                container.Bounds.Width = 250;
                container.Bounds.Height = 1f;
                container.Alignment = StackAlignment.Vertical;
                container.Inline = InlineType.Vertical;
                container.BorderColor[ElementState.Default] = p.GetColor("ui:input:color:2");
                container.BorderWidth[ElementState.Default] = 1;

                translation = container.Children.Create<ShapeTranslation>();
                translation.X.Input.OnValueChanged += this.HandleValueChanged;
                translation.Y.Input.OnValueChanged += this.HandleValueChanged;
                translation.R.Input.OnValueChanged += this.HandleValueChanged;

                for (Int32 i = 0; i < sides.Length; i++)
                {
                    sides[i] = container.Children.Create<SideInput>();
                    sides[i].Angle.Input.OnValueChanged += this.HandleValueChanged;
                    sides[i].Length.Input.OnValueChanged += this.HandleValueChanged;
                }
            });


            _shapes.Add(new ShapeDefinition()
            {
                Sides = sides,
                Translation = translation
            });

            this.Render();
        }

        private void Render()
        {
            _demo?.TryRelease();

            var config = new ShipPartContext();
            foreach (ShapeDefinition shape in _shapes)
            {
                try
                {
                    for (var i = 0; i < shape.Sides.Length; i++)
                    {
                        if (shape.Sides[i].Angle.Input.Value != "")
                        {
                            config.AddSide(
                                MathHelper.ToRadians(Single.Parse(shape.Sides[i].Angle.Input.Value)),
                                ShipPartShapeContext.NodeType.None,
                                shape.Sides[i].Length.Input.Value == "" ? 0 : Single.Parse(shape.Sides[i].Length.Input.Value));
                        }
                    }

                    config.Rotate(MathHelper.ToRadians(Single.Parse(shape.Translation.R.Input.Value)));
                    config.Transform(Matrix.CreateTranslation(new Vector3(Single.Parse(shape.Translation.X.Input.Value), Single.Parse(shape.Translation.Y.Input.Value), 0)));
                    config.Flush();
                }
                catch (Exception e)
                {
                    this.logger.Error(e.Message);
                    config.ClearBuffer();
                }
            }

            config.AddNode(Vector2.Zero, 0, ShipPartShapeContext.NodeType.Male);
            config.Flush();


            if (config.Vertices.Count == 0)
                return;

            _demo = this.Entities.Create<ShipPart>("entity:ship-part:dynamic", (demo, p, c) =>
            {
                demo.Context = config;
                demo.Position = _world.Size / 2 - config.Centeroid;
            });
        }

        private void HandleValueChanged(TextElement sender, string args)
            => Render();
        #endregion
    }
}

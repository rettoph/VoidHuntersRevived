using Guppy.Loaders;
using Guppy.UI.Entities.UI;
using Guppy.UI.Utilities.Units;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using Guppy.UI.Enums;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Library.Entities.Players;
using System.IO;
using System.Linq;

namespace VoidHuntersRevived.Client.Library.Entities.UI
{
    /// <summary>
    /// The main game HUD, providing info 
    /// and interfaces to interact with the game.
    /// </summary>
    public class Hud : Stage
    {
        #region Private Fields
        private ContentLoader _content;

        private Button _saveTrigger;
        private Button _saveButton;
        private FormComponent _saveInput;
        #endregion

        #region Public Attributes
        public Boolean Focused { get; private set; }

        public UserPlayer Client { get; internal set; }
        #endregion

        #region Lifecycle Methods
        protected override void Create(IServiceProvider provider)
        {
            base.Create(provider);

            _content = provider.GetRequiredService<ContentLoader>();
        }

        protected override void PreInitialize()
        {
            base.PreInitialize();

            this.SetLayerDepth(4);
            this.SetDrawOrder(10000);
        }

        protected override void Initialize()
        {
            base.Initialize();

            _saveTrigger = this.Add<Button>("hud:button", b =>
            {
                b.Bounds.Set(Unit.Get(1f, -75), 25, 50, 50);
                b.BackgroundImage = _content.TryGet<Texture2D>("icon:save");
                b.BackgroundStyle = BackgroundStyle.Fill;
                b.OnClicked += this.HandleSaveTriggerClicked;
            });

            _saveButton = this.Add<Button>("hud:button", b =>
            {
                b.Bounds.Set(Unit.Get(0.5f, -200), Unit.Get(0.25f, 80), 400, 30);
                b.OnClicked += this.HandleSaveClicked;
                b.Text = "Save";
                b.Hidden = true;
            });

            _saveInput = this.Add<FormComponent>(fc =>
            {
                fc.Label = "Ship Title";
                fc.Hidden = true;
                fc.Bounds.Set(Unit.Get(0.5f, -200), 0.25f, 400, 80);
            });
        }
        #endregion

        #region Event Handlers
        private void HandleSaveTriggerClicked(object sender, EventArgs e)
        {
            this.Focused = true;
            this.BackgroundColor = new Color(0, 0, 0, 100);

            _saveInput.Hidden = false;
            _saveButton.Hidden = false;
        }

        private void HandleSaveClicked(object sender, EventArgs e)
        {
            if (!Directory.Exists("Ships"))
                Directory.CreateDirectory("Ships");

            this.Client.Ship.Title = _saveInput.Value;

            using (FileStream output = File.OpenWrite($"Ships/{new String(this.Client.Ship.Title.Select(ch => Path.GetInvalidFileNameChars().Contains(ch) ? '_' : ch).ToArray())}_{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}.vh"))
            {
                output.Position = 0;
                output.SetLength(0);
                this.Client.Ship.Export().WriteTo(output);
            }

            this.Focused = false;
            this.BackgroundColor = null;

            _saveInput.Hidden = true;
            _saveButton.Hidden = true;
        }
        #endregion
    }
}

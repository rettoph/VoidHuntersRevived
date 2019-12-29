using Guppy;
using Guppy.Extensions.Collection;
using Guppy.Loaders;
using Guppy.UI.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Client.Library.Utilities.Cameras;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.ShipParts;

namespace VoidHuntersRevived.Client.Library.Entities
{
    internal sealed class PopupManager : Entity
    {
        #region Private Fields
        private PrimitiveBatch _primitiveBatch;
        private Dictionary<ShipPart, Popup> _popups;
        private SpriteBatch _spriteBatch;
        private SpriteFont _font;
        private FarseerCamera2D _camera;
        private GraphicsDevice _graphics;
        private Queue<Popup> _closed;
        #endregion

        #region Constructor
        public PopupManager(GraphicsDevice graphics, FarseerCamera2D camera, SpriteBatch spriteBatch, ContentLoader content, PrimitiveBatch primitiveBatch)
        {
            _spriteBatch = spriteBatch;
            _font = content.TryGet<SpriteFont>("font");
            _primitiveBatch = primitiveBatch;
            _popups = new Dictionary<ShipPart, Popup>();
            _camera = camera;
            _graphics = graphics;
            _closed = new Queue<Popup>();
        }
        #endregion

        #region Lifecycle Methods
        protected override void Create(IServiceProvider provider)
        {
            base.Create(provider);

            this.SetLayerDepth(3);
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _popups.Values.ForEach(p => p.Update(gameTime));
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            var origin = new Vector2(_graphics.Viewport.Width / 2, _graphics.Viewport.Height / 2);
            _popups.Values.ForEach(p =>
            {
                p.Draw(origin, _camera, _font, _spriteBatch, _primitiveBatch, gameTime);
                if(p.State < 0.001f)
                { // If popup is gone...
                    _closed.Enqueue(p);
                }
            });

            while(_closed.Any())
                this.Remove(_closed.Dequeue());
        }
        #endregion

        #region Helper Methods
        public void SetHovered(ShipPart target)
        {
            if(_popups.ContainsKey(target.Root))
            { // Set hover to true...
                _popups[target.Root].Hovered = true;
            }
            else
            { // Create a new popup for the requested target...
                _popups.Add(target.Root, Popup.Build(target.Root));
            }
        }

        private void Remove(Popup popup)
        {
            popup.Dispose();
            _popups.Remove(popup.Target);
        }
        #endregion
    }

    internal class Popup : IDisposable
    {
        #region Static Fields
        private static Queue<Popup> Queue = new Queue<Popup>();
        #endregion

        #region Public Attributes
        public ShipPart Target { get; private set; }
        public Single State { get; private set; }
        public Boolean Hovered { get; set; }
        #endregion

        #region Lifecycle Methods
        private Popup Initialize(ShipPart target)
        {
            this.Target = target;
            this.State = 0.002f;

            return this;
        }

        public void Dispose()
        {
            Popup.Queue.Enqueue(this);
        }
        #endregion

        #region Frame Methods
        public void Draw(Vector2 origin, FarseerCamera2D camera, SpriteFont font, SpriteBatch spriteBatch, PrimitiveBatch batch, GameTime gameTime)
        {
            this.State = MathHelper.Lerp(this.State, this.Hovered ? 1 : 0, this.Hovered ? 0.01f : 0.02f);

            String title;
            String description;
            String advanced;

            this.Target.GetInfo(out title, out description, out advanced);
            Vector2 offset = new Vector2(0, 45);
            Vector2 padding = new Vector2(5, 5);
            Vector3 rawPixelPos = camera.Project(new Vector3(this.Target.Root.WorldCenter, 0));
            Vector2 pixelPos = new Vector2(rawPixelPos.X, rawPixelPos.Y) - origin + offset;
            Vector2 bounds = font.MeasureString($"{title}\n{description}\n{advanced}") + padding + padding;
            Color fcolor = Color.Lerp(Color.Transparent, Color.White, this.State);
            Color fcolor2 = Color.Lerp(Color.Transparent, this.Target.Color, this.State);
            Color bColor = Color.Lerp(Color.Transparent, new Color(50, 50, 50, 200), this.State);

            spriteBatch.DrawString(
                spriteFont: font, 
                text: title, 
                position: new Vector2(pixelPos.X, pixelPos.Y) + padding, 
                color: fcolor2,
                rotation: 0,
                origin: Vector2.Zero,
                scale: 1,
                effects: SpriteEffects.None,
                layerDepth: 0);

            spriteBatch.DrawString(
                spriteFont: font,
                text: description,
                position: new Vector2(pixelPos.X, pixelPos.Y + font.LineSpacing) + padding,
                color: fcolor,
                rotation: 0,
                origin: Vector2.Zero,
                scale: 1,
                effects: SpriteEffects.None,
                layerDepth: 0);

            spriteBatch.DrawString(
                spriteFont: font,
                text: $"{advanced}",
                position: new Vector2(pixelPos.X, pixelPos.Y + font.LineSpacing + font.LineSpacing) + padding,
                color: fcolor,
                rotation: 0,
                origin: Vector2.Zero,
                scale: 1,
                effects: SpriteEffects.None,
                layerDepth: 0);

            batch.DrawTriangle(pixelPos, bColor, pixelPos + new Vector2(bounds.X, 0), bColor, pixelPos + bounds, bColor);
            batch.DrawTriangle(pixelPos, bColor, pixelPos + bounds, bColor, pixelPos + new Vector2(0, bounds.Y), bColor);
            batch.DrawLine(pixelPos + new Vector2(0, 0), Color.White, pixelPos + (new Vector2(bounds.X, 0) * this.State), Color.White);
            batch.DrawLine(pixelPos + new Vector2(0, 0), Color.White, pixelPos + (new Vector2(0, bounds.Y) * this.State), Color.White);
            batch.DrawLine(pixelPos + bounds, Color.White, pixelPos + bounds- (new Vector2(bounds.X, 0) * this.State), Color.White);
            batch.DrawLine(pixelPos + bounds, Color.White, pixelPos + bounds - (new Vector2(0, bounds.Y) * this.State), Color.White);
            batch.DrawLine(pixelPos, Color.White, pixelPos - (offset * this.State), Color.White);
        }

        public void Update(GameTime gameTime)
        {
            this.Hovered = false;
        }
        #endregion

        #region Static Methods
        public static Popup Build(ShipPart target)
        {
            return (Popup.Queue.Any() ? Popup.Queue.Dequeue() : new Popup()).Initialize(target);
        }
        #endregion
    }
}

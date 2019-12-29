using Guppy;
using Guppy.Extensions.Collection;
using Guppy.Loaders;
using Guppy.UI.Entities;
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
using VoidHuntersRevived.Library.Extensions.Microsoft.Xna;

namespace VoidHuntersRevived.Client.Library.Entities
{
    internal sealed class PopupManager : Entity
    {
        #region Static Fields
        public static Double PopupDelay { get; private set; } = 500;
        #endregion

        #region Private Fields
        private Dictionary<ShipPart, Popup> _popups;
        private Queue<Popup> _closed;
        private ShipPart _target;
        private DateTime _targetChanged;
        #endregion

        #region Internal Attributes
        internal PrimitiveBatch primitiveBatch { get; private set; }
        internal SpriteBatch spriteBatch { get; private set; }
        internal SpriteFont font { get; private set; }
        internal FarseerCamera2D camera { get; private set; }
        internal GraphicsDevice graphics { get; private set; }
        internal Pointer pointer { get; private set; }
        #endregion

        #region Constructor
        public PopupManager(Pointer pointer, GraphicsDevice graphics, FarseerCamera2D camera, SpriteBatch spriteBatch, ContentLoader content, PrimitiveBatch primitiveBatch)
        {
            
            _popups = new Dictionary<ShipPart, Popup>();
            _closed = new Queue<Popup>();
            this.camera = camera;
            this.graphics = graphics;
            this.spriteBatch = spriteBatch;
            this.font = content.TryGet<SpriteFont>("font");
            this.primitiveBatch = primitiveBatch;
            this.pointer = pointer;
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

            if (_target != default(ShipPart) && DateTime.Now.Subtract(_targetChanged).TotalMilliseconds >= PopupManager.PopupDelay)
            { // If the client has been hovering a specific part for the amount of time required...
                if (_popups.ContainsKey(_target.Root))
                { // Set hover to true...
                    _popups[_target.Root].Hovered = true;
                }
                else
                { // Create a new popup for the requested target...
                    _popups.Add(_target.Root, Popup.Build(this, _target.Root));
                }
            }

            _popups.Values.ForEach(p => p.Update(gameTime));
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            var origin = new Vector2(this.graphics.Viewport.Width / 2, this.graphics.Viewport.Height / 2);
            _popups.Values.ForEach(p =>
            {
                p.Draw(origin, gameTime);
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
            if (_target != target?.Root)
            {
                if (_target != default(ShipPart) && _popups.ContainsKey(_target.Root))
                    _popups[_target.Root].Hovered = false;

                _target = target?.Root;
                _targetChanged = DateTime.Now;
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
        public static Vector2 Padding { get; private set; } = new Vector2(5, 5);
        public static Vector2 Offset { get; private set; } = new Vector2(0, 35);
        #endregion

        #region Private Fields
        private PopupManager _manager;
        #endregion

        #region Public Attributes
        public ShipPart Target { get; private set; }
        public Single State { get; private set; }
        public Boolean Hovered { get; set; }
        #endregion

        #region Lifecycle Methods
        private Popup Initialize(PopupManager manager, ShipPart target)
        {
            _manager = manager;
            this.Target = target;
            this.State = 0.002f;
            this.Hovered = true;

            return this;
        }

        public void Dispose()
        {
            Popup.Queue.Enqueue(this);
        }
        #endregion

        #region Frame Methods
        public void Draw(Vector2 origin, GameTime gameTime)
        {
            String title;
            String description;
            String advanced;

            this.Target.GetInfo(out title, out description, out advanced);
            Vector2 targetPixelPos = (_manager.camera.Project(this.Target.Root.WorldCenter.ToVector3(0)).ToVector2() - origin).Round() + new Vector2(0.5f, 0f);
            Vector2 pixelPos = targetPixelPos + Popup.Offset;
            Vector2 offset = pixelPos - targetPixelPos; 
            Vector2 bounds = _manager.font.MeasureString($"{title}\n{description}\n{advanced}") + Popup.Padding + Popup.Padding;
            Color fcolor = Color.Lerp(Color.Transparent, Color.White, this.State);
            Color fcolor2 = Color.Lerp(Color.Transparent, this.Target.Color, this.State);
            Color bColor = Color.Lerp(Color.Transparent, new Color(50, 50, 50, 200), this.State);

            _manager.spriteBatch.DrawString(
                spriteFont: _manager.font, 
                text: title, 
                position: new Vector2(pixelPos.X, pixelPos.Y) + Popup.Padding, 
                color: fcolor2,
                rotation: 0,
                origin: Vector2.Zero,
                scale: 1,
                effects: SpriteEffects.None,
                layerDepth: 0);

            _manager.spriteBatch.DrawString(
                spriteFont: _manager.font,
                text: description,
                position: new Vector2(pixelPos.X, pixelPos.Y + _manager.font.LineSpacing) + Popup.Padding,
                color: fcolor,
                rotation: 0,
                origin: Vector2.Zero,
                scale: 1,
                effects: SpriteEffects.None,
                layerDepth: 0);

            _manager.spriteBatch.DrawString(
                spriteFont: _manager.font,
                text: $"{advanced}",
                position: new Vector2(pixelPos.X, pixelPos.Y + _manager.font.LineSpacing + _manager.font.LineSpacing) + Popup.Padding,
                color: fcolor,
                rotation: 0,
                origin: Vector2.Zero,
                scale: 1,
                effects: SpriteEffects.None,
                layerDepth: 0);

            _manager.primitiveBatch.DrawTriangle(pixelPos, bColor, pixelPos + new Vector2(bounds.X, 0), bColor, pixelPos + bounds, bColor);
            _manager.primitiveBatch.DrawTriangle(pixelPos, bColor, pixelPos + bounds, bColor, pixelPos + new Vector2(0, bounds.Y), bColor);
            _manager.primitiveBatch.DrawLine(pixelPos + new Vector2(0, 0), Color.White, pixelPos + (new Vector2(bounds.X, 0) * this.State), Color.White);
            _manager.primitiveBatch.DrawLine(pixelPos + new Vector2(0, 0), Color.White, pixelPos + (new Vector2(0, bounds.Y) * this.State), Color.White);
            _manager.primitiveBatch.DrawLine(pixelPos + bounds, Color.White, pixelPos + bounds- (new Vector2(bounds.X, 0) * this.State), Color.White);
            _manager.primitiveBatch.DrawLine(pixelPos + bounds, Color.White, pixelPos + bounds - (new Vector2(0, bounds.Y) * this.State), Color.White);
            _manager.primitiveBatch.DrawLine(pixelPos, Color.White, pixelPos - (offset * this.State), Color.White);
        }

        public void Update(GameTime gameTime)
        {
            this.State = MathHelper.Lerp(this.State, this.Hovered ? 1 : 0, (this.Hovered ? 0.00625f : 0.0125f) * (Single)gameTime.ElapsedGameTime.TotalMilliseconds);
        }
        #endregion

        #region Static Methods
        public static Popup Build(PopupManager manager, ShipPart target)
        {
            return (Popup.Queue.Any() ? Popup.Queue.Dequeue() : new Popup()).Initialize(manager, target);
        }
        #endregion
    }
}

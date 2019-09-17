using GalacticFighters.Client.Library.Scenes;
using Guppy;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace GalacticFighters.Client.Library.Entities
{
    public sealed class Pointer : Entity
    {
        #region Enums
        [Flags]
        public enum Button
        {
            Left = 1,
            Middle = 2,
            Right = 4
        }
        #endregion

        #region Private Fields
        private SpriteFont _font;
        private SpriteBatch _spriteBatch;
        #endregion

        #region Public Attributes
        public Vector2 Position { get; private set; }
        public Button Buttons { get; private set; }

        public Single Scroll { get; private set; }
        #endregion

        #region Constructor
        public Pointer(SpriteBatch spriteBatch, ContentManager content)
        {
            _font = content.Load<SpriteFont>("font");
            _spriteBatch = spriteBatch;
        }
        #endregion

        #region Lifecycle Methods
        protected override void Create(IServiceProvider provider)
        {
            base.Create(provider);

            this.Events.Register<Vector2>("moved");
            this.Events.Register<Button>("pressed");
            this.Events.Register<Button>("released");
            this.Events.Register<Single>("scrolled");
        }
        #endregion

        #region Frame Methods
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            _spriteBatch.DrawString(_font, $"Pointer: {this.Position.X}, {this.Position.Y}", new Vector2(15, 15), Color.White);
        }
        #endregion

        #region Utility Methods
        public void MoveTo(Vector2 position)
        {
            this.Position = position;

            this.Events.TryInvoke<Vector2>(this, "moved", this.Position);
        }

        public void MoveBy(Vector2 delta)
        {
            this.MoveTo(this.Position + delta);
        }

        public void ScrollTo(Single scroll)
        {
            this.Scroll = scroll;

            this.Events.TryInvoke<Single>(this, "scrolled", this.Scroll);
        }

        public void ScrollBy(Single delta)
        {
            this.ScrollTo(this.Scroll + delta);
        }

        public void SetButton(Button button, Boolean value)
        {
            if (value)
            {
                this.Buttons |= button;
                this.Events.TryInvoke<Button>(this, "pressed", button);
            }
            else
            {
                this.Buttons &= ~button;
                this.Events.TryInvoke<Button>(this, "released", button);
            }
        }
        #endregion
    }
}

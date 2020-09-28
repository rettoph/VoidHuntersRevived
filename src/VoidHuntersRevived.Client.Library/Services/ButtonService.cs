using Guppy;
using Guppy.DependencyInjection;
using Guppy.Extensions.Collections;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace VoidHuntersRevived.Client.Library.Services
{
    /// <summary>
    /// Simple class used to track specific button actions.
    /// 
    /// Toggles, presses, and lifts can be managed here.
    /// 
    /// Tracks keyboard & mouse buttons
    /// </summary>
    public sealed class ButtonService : Asyncable
    {
        #region Classes, Structs, & Enums
        public sealed class ButtonManager
        {
            #region Private Fields
            private Dictionary<Microsoft.Xna.Framework.Input.ButtonState, ButtonValue> _args;
            #endregion

            #region Public Fields
            public readonly Button Which;
            public Microsoft.Xna.Framework.Input.ButtonState State;
            #endregion

            #region Events
            public delegate void OnKeyDelegate(ButtonManager sender, ButtonValue key);
            public event OnKeyDelegate OnKeyPressed;
            public event OnKeyDelegate OnKeyReleased;
            #endregion

            #region Constructor
            public ButtonManager(Button which)
            {
                this.Which = which;

                _args = ((ButtonState[])Enum.GetValues(typeof(ButtonState))).ToDictionary(
                    keySelector: bs => bs,
                    elementSelector: bs => new ButtonValue(this.Which, bs));
            }
            #endregion

            #region Frame Methods
            public void TrySetState(ButtonState state)
            {
                if (this.State != state)
                {
                    this.State = state;

                    if (this.State == ButtonState.Pressed)
                    { // Pressed...
                        this.OnKeyPressed?.Invoke(this, _args[this.State]);
                    }
                    else if (this.State == ButtonState.Released)
                    { // Released...
                        this.OnKeyReleased?.Invoke(this, _args[this.State]);
                    }
                }
            }
            #endregion
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct Button {
            [FieldOffset(0)]
            public readonly ButtonType Type;

            [FieldOffset(1)]
            public readonly Keys KeyboardKey;

            public Button(ButtonType type, Keys keyboardKey)
            {
                this.Type = type;
                this.KeyboardKey = keyboardKey;
            }
        }

        public enum ButtonType : Byte
        {
            Cursor,
            Keyboard
        }

        public struct ButtonValue
        {
            public readonly Button Which;
            public readonly ButtonState State;

            public ButtonValue(Button which, ButtonState state)
            {
                this.Which = which;
                this.State = state;
            }
        }
        #endregion

        #region Private Fields
        private Dictionary<Keys, ButtonManager> _keys;
        #endregion

        #region Public Attributes
        public ButtonManager this[Keys key]
        {
            get
            {
                if (!_keys.ContainsKey(key))
                    _keys[key] = new ButtonManager(new Button(ButtonType.Keyboard, key));

                return _keys[key];
            }
        }
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            _keys = new Dictionary<Keys, ButtonManager>();
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            var kState = Keyboard.GetState();
            _keys.ForEach(kvp => kvp.Value.TrySetState(kState.IsKeyDown(kvp.Key) ? ButtonState.Pressed : ButtonState.Released));
        }
        #endregion
    }
}

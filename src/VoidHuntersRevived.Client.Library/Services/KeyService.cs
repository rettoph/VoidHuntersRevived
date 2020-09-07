using Guppy;
using Guppy.DependencyInjection;
using Guppy.Extensions.Collections;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace VoidHuntersRevived.Client.Library.Services
{
    /// <summary>
    /// Simple class used to track specific key actions.
    /// 
    /// Toggles, presses, and lifts can be managed here.
    /// </summary>
    public sealed class KeyService : Asyncable
    {
        #region Classes
        public sealed class KeyManager
        {
            #region Private Fields
            private Boolean _oldPressed;
            #endregion

            #region Public Fields
            public readonly Keys Key;
            public Boolean Pressed { get; private set; }
            #endregion

            #region Events
            public delegate void OnKeyDelegate(KeyManager key);
            public event OnKeyDelegate OnKeyPressed;
            public event OnKeyDelegate OnKeyReleased;
            #endregion

            #region Constructor
            public KeyManager(Keys key)
            {
                this.Key = key;
            }
            #endregion

            #region Frame Methods
            public void Update(Boolean pressed)
            {
                this.Pressed = pressed;

                if(this.Pressed && !_oldPressed)
                { // Pressed...
                    this.OnKeyPressed?.Invoke(this);
                }
                else if(!this.Pressed && _oldPressed)
                { // Released...
                    this.OnKeyReleased?.Invoke(this);
                }

                // Update the old state...
                _oldPressed = this.Pressed;
            }
            #endregion
        }
        #endregion

        #region Private Fields
        private Dictionary<Keys, KeyManager> _keys;
        #endregion

        #region Public Attributes
        public KeyManager this[Keys key]
        {
            get
            {
                if (!_keys.ContainsKey(key))
                    _keys[key] = new KeyManager(key);

                return _keys[key];
            }
        }
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            _keys = new Dictionary<Keys, KeyManager>();
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            var kState = Keyboard.GetState();
            _keys.ForEach(kvp => kvp.Value.Update(kState.IsKeyDown(kvp.Key)));
        }
        #endregion
    }
}

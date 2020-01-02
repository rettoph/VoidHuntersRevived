using System;
using System.Collections.Generic;
using System.Text;
using FarseerPhysics.Dynamics;
using Guppy.Extensions.Collection;
using Microsoft.Xna.Framework;

namespace VoidHuntersRevived.Library.Entities.Controllers
{
    /// <summary>
    /// Represents a special controller owned & maintained by another
    /// object. Custom Setup & Update methods can be added by setting
    /// public properties.
    /// 
    /// By default, this is not enabled or visible.
    /// It is expected that a managing entity will manually call
    /// the TryUpdate & TryDraw methods.
    /// </summary>
    public class CustomController : Controller
    {
        #region Public Properties
        public delegate void BodyDelegate(FarseerEntity component, Body body);

        public BodyDelegate OnSetupBody { get; set; }
        public BodyDelegate OnUpdateBody { get; set; }
        public BodyDelegate OnSetdownBody { get; set; }
        #endregion

        #region Lifecycle Methods
        protected override void Initialize()
        {
            base.Initialize();

            this.SetEnabled(false);
            this.SetVisible(false);
        }
        #endregion

        #region Helper Methods
        public override void SetupBody(FarseerEntity component, Body body)
        {
            base.SetupBody(component, body);

            // Invoke all custom setup methods
            this.OnSetupBody?.Invoke(component, body);
        }

        public override void UpdateBody(FarseerEntity component, Body body)
        {
            base.UpdateBody(component, body);

            // Invoke all custom update methods
            this.OnUpdateBody?.Invoke(component, body);
        }
        #endregion

        #region Set Methods
        public void SetLocked(Boolean value)
        {
            this.Locked = value;
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            this.Components.ForEach(fe => fe.TryUpdate(gameTime));
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            // Draw all internal components
            this.Components.ForEach(fe => fe.TryDraw(gameTime));
        }
        #endregion
    }
}

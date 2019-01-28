using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.Interfaces
{
    public interface IFarseerEntityDriver
    {
        Vector2 Position { get; set; }
        Vector2 LinearVelocity { get; set; }

        Single Rotation { get; set; }
        Single AngularVelocity { get; set; }

        void Update(GameTime gameTime);
    }
}

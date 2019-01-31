using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Core.Interfaces;

namespace VoidHuntersRevived.Library.Interfaces
{
    public interface IFarseerEntityDriver : IEntity
    {
        Vector2 Position { get; set; }
        Vector2 LinearVelocity { get; set; }

        Single Rotation { get; set; }
        Single AngularVelocity { get; set; }
    }
}

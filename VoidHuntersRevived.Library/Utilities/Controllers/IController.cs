using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;

namespace VoidHuntersRevived.Library.Utilities.Controllers
{
    public interface IController
    {
        Color Color { get; }
        Category CollidesWith { get; }
        Category CollisionCategories { get; }
        Category IgnoreCCDWith { get; }

        Boolean Add(Object entity);

        Boolean Remove(Object entity);
    }
}

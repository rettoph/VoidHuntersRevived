using FarseerPhysics.Dynamics;
using Guppy;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;

namespace VoidHuntersRevived.Library.Utilities.Controllers
{
    public abstract class Controller : Driven
    {
        public Color? Color { get; set; }
        public Category CollidesWith { get; set; } = Categories.ActiveCollidesWith;
        public Category CollisionCategories { get; set; } = Categories.ActiveCollisionCategories;
        public Category IgnoreCCDWith { get; set; } = Categories.ActiveIgnoreCCDWith;

        protected internal abstract void Remove(Object entity);
    }
}

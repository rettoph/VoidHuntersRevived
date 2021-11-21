using System;
using System.Collections.Generic;
using System.Text;
using tainicom.Aether.Physics2D.Dynamics;

namespace VoidHuntersRevived.Library.Globals.Constants
{
    public static class CollisionCategories
    {
        public const Category ImpenetrableCollidesWith = Category.All;
        public const Category ImpenetrableCollisionCategories = Category.Cat1;
        public const Int16 ImpenetrableCollisionGroup = 0;

        public const Category CorporealCollidesWith = CollisionCategories.ImpenetrableCollisionCategories | CollisionCategories.CorporealCollisionCategories;
        public const Category CorporealCollisionCategories = Category.Cat2;
        public const Int16 CorporealCollisionGroup = 0;

        public const Category NonCorporealCollidesWith = CollisionCategories.ImpenetrableCollisionCategories;
        public const Category NonCorporealCollisionCategories = Category.Cat3;
        public const Int16 NonCorporealCollisionGroup = -1;
    }
}

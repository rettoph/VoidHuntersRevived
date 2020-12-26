using tainicom.Aether.Physics2D.Dynamics;

namespace VoidHuntersRevived.Library.Utilities
{
    /// <summary>
    /// Default collision categories used within the game
    /// </summary>
    public static class Categories
    {
        public static readonly Category BorderCollisionCategories = Category.Cat1;
        public static readonly Category PassiveCollisionCategories = Category.Cat2;
        public static readonly Category ActiveCollisionCategories = Category.Cat3;

        public static readonly Category BorderCollidesWith = Categories.PassiveCollisionCategories | Categories.ActiveCollisionCategories;
        public static readonly Category ActiveCollidesWith = Categories.BorderCollisionCategories | Categories.ActiveCollisionCategories;
        public static readonly Category PassiveCollidesWith = Categories.BorderCollisionCategories;

    }
}

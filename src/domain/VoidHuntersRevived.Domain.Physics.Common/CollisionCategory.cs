namespace VoidHuntersRevived.Domain.Physics.Common
{
    public struct CollisionCategory
    {
        private readonly byte _bit;
        internal readonly int _mask => 0x1 << this._bit;

        public readonly string Name => _names[this._bit];

        public CollisionCategory()
        {
            throw new NotImplementedException();

        }
        internal CollisionCategory(byte bit)
        {
            if (bit > 31)
            {
                throw new ArgumentOutOfRangeException();
            }

            this._bit = bit;
        }

        public override string ToString()
        {
            return this.Name;
        }

        private static byte _currentBit;
        private static readonly string[] _names = new string[32];
        public static CollisionCategory Create(string name)
        {
            _names[_currentBit] = name;
            return new CollisionCategory(_currentBit++);
        }
    }
}
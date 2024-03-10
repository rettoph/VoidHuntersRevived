namespace VoidHuntersRevived.Domain.Physics.Common
{
    public struct CollisionCategory
    {
        private byte _bit;
        internal int _mask => 0x1 << _bit;

        public string Name => _names[_bit];

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

            _bit = bit;
        }

        public override string ToString()
        {
            return this.Name;
        }

        private static byte _currentBit;
        private static string[] _names = new string[32];
        public static CollisionCategory Create(string name)
        {
            _names[_currentBit] = name;
            return new CollisionCategory(_currentBit++);
        }
    }
}

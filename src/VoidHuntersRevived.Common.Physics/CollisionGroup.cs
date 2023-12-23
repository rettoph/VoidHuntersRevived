using System.Collections.Specialized;

namespace VoidHuntersRevived.Common.Physics
{
    public struct CollisionGroup
    {
        private BitVector32 _flags;
        private byte _nameIndex;

        public string Name => _names[_nameIndex];

        public IEnumerable<CollisionCategory> Categories
        {
            get
            {
                for (byte i = 0; i < 32; i++)
                {
                    if (_flags[0x1 << i])
                    {
                        yield return new CollisionCategory(i);
                    }
                }
            }
        }

        public int Flags => _flags.Data;

        public CollisionGroup()
        {
            throw new InvalidOperationException();
        }
        private CollisionGroup(byte nameIndex, params CollisionCategory[] categories)
        {
            _nameIndex = nameIndex;
            _flags = new BitVector32();

            this.Append(categories);
        }

        public void Append(params CollisionCategory[] categories)
        {
            foreach (CollisionCategory category in categories)
            {
                _flags[category._mask] = true;
            }
        }

        public void Truncate(params CollisionCategory[] categories)
        {
            foreach (CollisionCategory category in categories)
            {
                _flags[category._mask] = false;
            }
        }

        private static List<string> _names = new List<string>();
        private static Dictionary<string, CollisionGroup> _dict = new Dictionary<string, CollisionGroup>();
        public static CollisionGroup Create(string name, params CollisionCategory[] categories)
        {
            var group = new CollisionGroup((byte)_names.Count, categories);

            _dict.Add(name, group);
            _names.Add(name);

            return group;
        }

        public static CollisionGroup GetByName(string name)
        {
            return _dict[name];
        }
    }
}

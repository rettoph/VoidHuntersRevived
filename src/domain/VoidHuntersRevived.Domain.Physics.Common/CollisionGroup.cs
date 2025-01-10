using System.Collections.Specialized;

namespace VoidHuntersRevived.Domain.Physics.Common
{
    public struct CollisionGroup
    {
        private BitVector32 _flags;
        private readonly byte _nameIndex;

        public readonly string Name => _names[this._nameIndex];

        public IEnumerable<CollisionCategory> Categories
        {
            get
            {
                for (byte i = 0; i < 32; i++)
                {
                    if (this._flags[0x1 << i])
                    {
                        yield return new CollisionCategory(i);
                    }
                }
            }
        }

        public int Flags => this._flags.Data;

        public CollisionGroup()
        {
            throw new InvalidOperationException();
        }
        private CollisionGroup(byte nameIndex, params CollisionCategory[] categories)
        {
            this._nameIndex = nameIndex;
            this._flags = new BitVector32();

            this.Append(categories);
        }

        public void Append(params CollisionCategory[] categories)
        {
            foreach (CollisionCategory category in categories)
            {
                this._flags[category.mask] = true;
            }
        }

        public void Truncate(params CollisionCategory[] categories)
        {
            foreach (CollisionCategory category in categories)
            {
                this._flags[category.mask] = false;
            }
        }

        private static readonly List<string> _names = [];
        private static readonly Dictionary<string, CollisionGroup> _dict = [];
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
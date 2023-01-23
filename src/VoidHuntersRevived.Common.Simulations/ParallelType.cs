using Guppy.Common.Collections;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Simulations
{
    public readonly struct ParallelType
    {
        private static byte _id;
        private static DoubleDictionary<int, string, ParallelType> _types;

        public readonly byte Value;
        public readonly string Name;

        private ParallelType(byte value, string name)
        {
            this.Value = value;
            this.Name = name;
        }

        static ParallelType()
        {
            _types = new DoubleDictionary<int, string, ParallelType>();
        }
        public ParallelKey Create(int noise)
        {
            return ParallelKey.From(this, noise);
        }

        public static ParallelType Get(byte value)
        {
            return _types[value];
        }

        public static ParallelType Get(string name)
        {
            return _types[name];
        }

        public static ParallelType GetOrRegister(string name)
        {
            if(_types.TryGet(name, out var type))
            {
                return type;
            }

            type = new ParallelType(_id++, name);
            if(_types.TryAdd(type.Value, type.Name, type))
            {
                return type;
            }

            throw new NotImplementedException();
        }
    }
}

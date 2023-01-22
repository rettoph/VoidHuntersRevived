using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Common.Simulations
{
    public readonly struct ParallelKey
    {
        public readonly ParallelType Type;
        public readonly int Value;
        public readonly int Index;

        private ParallelKey(ParallelType type, int value, int index)
        {
            this.Type = type;
            this.Value = value;
            this.Index = index;
        }

        public ParallelKey Create(ParallelType type)
        {
            return ParallelKey.From(type, this.Value);
        }

        public ParallelKey Create(ParallelType type, int index)
        {
            return ParallelKey.From(type, this.GetHashCode(), index);
        }

        public void WriteAll(NetDataWriter writer)
        {
            writer.Put(this.Type.Value);
            writer.Put(this.Value);
            writer.Put(this.Index);
        }

        public void Write(NetDataWriter writer)
        {
            writer.Put(this.Value);
            writer.Put(this.Index);
        }

        public void WriteValue(NetDataWriter writer)
        {
            writer.Put(this.Value);
        }

        public static ParallelKey From(ParallelType type, int value)
        {
            return new ParallelKey(type, value, 0);
        }

        public static ParallelKey From(ParallelType type, int value, int index)
        {
            return new ParallelKey(type, value, index);
        }

        public static ParallelKey ReadAll(NetDataReader reader)
        {
            return ParallelKey.From(
                type: ParallelType.Get(reader.GetByte()),
                value: reader.GetInt(),
                index: reader.GetInt());
        }

        public static ParallelKey Read(NetDataReader reader, ParallelType type)
        {
            return ParallelKey.From(
                type: type,
                value: reader.GetInt(),
                index: reader.GetInt());
        }

        public static ParallelKey ReadValue(NetDataReader reader, ParallelType type)
        {
            return ParallelKey.From(
                type: type,
                value: reader.GetInt());
        }
    }
}

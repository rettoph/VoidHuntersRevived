using Guppy.Attributes;
using Guppy.Common;
using Guppy.Common.Collections;
using Guppy.MonoGame;
using Guppy.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Messages;

namespace VoidHuntersRevived.Client.Library.Services
{
    public sealed class AetherBodyPositionDebugService
    {
        private readonly struct Key
        {
            public readonly bool Local;
            public readonly int Id;

            public Key(bool local, int id)
            {
                this.Local = local;
                this.Id = id;
            }

            public override bool Equals(object? obj)
            {
                return obj is Key key &&
                       Local == key.Local &&
                       Id == key.Id;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Local, Id);
            }
        }
        private readonly IDictionary<Key, Buffer<Vector2>> _data;

        public AetherBodyPositionDebugService()
        {
            _data = new Dictionary<Key, Buffer<Vector2>>();
        }

        public void Draw(PrimitiveBatch<VertexPositionColor> primitiveBatch)
        {
            foreach (var kvp in _data)
            {
                if(kvp.Value.Position < 2)
                {
                    continue;
                }

                Vector2? p1 = default;
                Vector2? p2 = default;

                foreach(Vector2 position in kvp.Value)
                {
                    p2 = p1;
                    p1 = position;

                    if(p1 is not null && p2 is not null)
                    {
                        var color = kvp.Key.Local ? Color.Red : Color.Green;
                        primitiveBatch.DrawLine(color, p1.Value, p2.Value);
                    }
                }
            }
        }

        public void AddPosition(bool local, int id, Vector2 position)
        {
            var key = new Key(local, id);

            if(!_data.TryGetValue(key, out var dataPoints))
            {
                _data.Add(key, dataPoints = new Buffer<Vector2>(250));
            }

            dataPoints.Add(position);
        }
    }
}

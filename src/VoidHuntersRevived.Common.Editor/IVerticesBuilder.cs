using Guppy.MonoGame.Primitives;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Editor
{
    public interface IVerticesBuilder
    {
        bool Building { get; }
        bool Closed { get; }
        ref bool Snap { get; }
        ref float Degrees { get; }
        ref float Length { get; }
        float Radians { get; }

        void Start(bool closed);

        IEnumerable<Vector2> Build();

        void Add(Vector2? vertex = null);

        bool Remove(int? index = null);

        void Draw();
    }
}

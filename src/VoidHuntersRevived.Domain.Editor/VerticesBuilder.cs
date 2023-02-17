using Guppy.Common;
using Guppy.MonoGame.Primitives;
using Guppy.MonoGame.Utilities.Cameras;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Common;
using VoidHuntersRevived.Common.Editor;
using VoidHuntersRevived.Common.Helpers;

namespace VoidHuntersRevived.Domain.Editor.Services
{
    internal sealed partial class VerticesBuilder : IVerticesBuilder
    {
        private static readonly PrimitiveShape VertexShape = new PrimitiveShape(Vector2Helper.CreateCircle(0.25f, 16));

        private bool _snap = true;
        private float _degrees = 5f;
        private float _length = 1f;

        private List<Vector2> _vertices;
        private Camera2D _camera;
        private PrimitiveBatch<VertexPositionColor> _primitiveBatch;
        private Outline _outline;
        private ReadOnlyCollection<Vector2> _verticesReadonly;

        public bool Building { get; private set; }
        public bool Closed { get; private set; }

        public ref bool Snap => ref _snap;

        public ref float Degrees => ref _degrees;

        public ref float Length => ref _length;

        public float Radians => MathHelper.ToRadians(_degrees);

        public VerticesBuilder(
            Camera2D camera,
            PrimitiveBatch<VertexPositionColor> primitiveBatch)
        {
            _camera = camera;
            _vertices = new List<Vector2>();
            _primitiveBatch = primitiveBatch;
            _outline = new Outline();
            _verticesReadonly = new ReadOnlyCollection<Vector2>(_vertices);

            this.Snap = true;
        }

        public void Start(bool closed)
        {
            if(this.Building)
            {
                throw new NotImplementedException();
            }

            _vertices.Clear();

            this.Closed = closed;
            this.Building = true;
        }

        public IEnumerable<Vector2> Build()
        {
            var vertices = this.GetVertices(false).ToList();
            this.Building = false;

            return vertices;
        }

        public void Add(Vector2? vertex = null)
        {
            vertex ??= this.GetInputVertex();

            _vertices.Add(vertex.Value);
        }

        public bool Remove(int? index = null)
        {
            index ??= _vertices.Count - 1;

            if(_vertices.Count <= index.Value || index < 0)
            {
                return false;
            }

            _vertices.RemoveAt(index.Value);

            return true;
        }

        public void Draw()
        {
            if(!this.Building)
            {
                return;
            }

            _outline.Clean(this.GetVertices(true));
            _primitiveBatch.Trace(_outline, Color.Green, Matrix.Identity);

            foreach (Vector2 vertex in this.GetVertices(false))
            {
                _primitiveBatch.Trace(VertexShape, Color.Red, vertex.ToTranslation());
            }

            _primitiveBatch.Trace(VertexShape, Color.Yellow, this.GetInputVertex().ToTranslation());
        }

        private IEnumerable<Vector2> GetVertices(bool includeInput)
        {
            foreach(Vector2 vertex in _vertices)
            {
                yield return vertex;
            }

            if(includeInput)
            {
                yield return this.GetInputVertex();
            }
            

            if(this.Closed && _vertices.Count > 0)
            {
                yield return _vertices.First();
            }
        }

        private Vector2 GetInputVertex()
        {
            var mouse = Mouse.GetState();
            var input = _camera.Unproject(mouse.Position.ToVector2());

            if(!this.Snap)
            {
                return input;
            }

            if (_vertices.Count == 0)
            {
                return new Vector2()
                {
                    X = input.X.RoundNearest(1f),
                    Y = input.Y.RoundNearest(1f)
                };
            }

            int lastIndex = _vertices.Count - 1;
            Vector2 last = _vertices[lastIndex];
            Vector2 worldPoint = last + Vector2.UnitX;
            Vector2 localPoint = _vertices.Count == 1 ? worldPoint : _vertices[lastIndex - 1];

            float worldAngle = last.Angle(worldPoint, input);
            float localAngle = last.Angle(localPoint, input);

            // Zero degrees
            float angle = worldAngle - localAngle;

            if (localAngle < -MathHelper.PiOver2)
            {
                angle += MathHelper.Pi;
            }
            else if (localAngle >= 0)
            {
                angle += localAngle.RoundNearest(this.Radians);
            }

            float distance = Vector2.Distance(last, input);
            distance = distance.RoundNearest(_length);

            // distance = MathF.Max(this.Snap.Length, distance);

            input = last + new Vector2()
            {
                X = MathF.Cos(angle) * distance,
                Y = MathF.Sin(angle) * distance,
            };

            return input;
        }
    }
}

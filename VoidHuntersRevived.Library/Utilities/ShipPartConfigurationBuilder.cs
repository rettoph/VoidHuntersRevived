using FarseerPhysics;
using FarseerPhysics.Common;
using Guppy.Loaders;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Library.Configurations;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Utilities.ConnectionNodes;

namespace VoidHuntersRevived.Library.Utilities
{
    /// <summary>
    /// Simple static class used to auto generate 
    /// ship part data on initialize time.
    /// An entity loader, description, name,
    /// and side count must be inserted to use.
    /// </summary>
    public class ShipPartConfigurationBuilder
    {
        public enum NodeType {
            None,
            Male,
            Female
        }
        public static Vector2 Side = new Vector2(1, 0);

        private List<List<Vector2>> _savedVertices;
        public readonly Vector2 Start;
        private List<Vector2> _vertices;
        private List<Vector3> _femaleNodes;
        private Vector3 _maleNode;
        private Vector2 _lastPoint;
        private Single _lastRadians;

        public ShipPartConfigurationBuilder(Vector2 start = default(Vector2))
        {
            _savedVertices = new List<List<Vector2>>();
            _vertices = new List<Vector2>();
            _femaleNodes = new List<Vector3>();

            this.Start = start;
            _vertices.Add(start);
        }

        public void AddVertice(Vector2 vertice)
        {
            this._vertices.Add(vertice);
        }

        public void FlushVertices(Vector2 nextStart = default(Vector2))
        {
            _savedVertices.Add(_vertices.Select(v => v).ToList());
            _vertices.Clear();

            _vertices.Add(nextStart);
            _lastRadians = 0;
        }

        public void AddSide(Single radians, NodeType nodeType = NodeType.None)
        {
            _lastPoint = _vertices.Last();
            _lastRadians += radians;

            var rotationMatrix = Matrix.CreateRotationZ(_lastRadians);
            _vertices.Add(_lastPoint + Vector2.Transform(ShipPartConfigurationBuilder.Side, rotationMatrix));

            if(nodeType == NodeType.Female)
            {
                var coords = _lastPoint + Vector2.Transform(ShipPartConfigurationBuilder.Side / 2, rotationMatrix);
                _femaleNodes.Add(new Vector3(coords, _lastRadians + MathHelper.PiOver2));
            }
            else if (nodeType == NodeType.Male)
            {
                var coords = _lastPoint + Vector2.Transform(ShipPartConfigurationBuilder.Side / 2, rotationMatrix);
                _maleNode = new Vector3(coords, _lastRadians - MathHelper.PiOver2);
            }
        }

        public void TrimLast()
        {
            _vertices.RemoveAt(_vertices.Count() - 1);
        }

        public void Rotate(Single rads)
        {
            foreach (List<Vector2> vertices in _savedVertices)
                for (Int32 i = 0; i < vertices.Count; i++)
                    vertices[i] = Vector2.Transform(vertices[i], Matrix.CreateRotationZ(rads));

            for (Int32 i = 0; i < _femaleNodes.Count; i++)
            {
                _femaleNodes[i] = Vector3.Transform(_femaleNodes[i], Matrix.CreateRotationZ(rads));
                _femaleNodes[i] = new Vector3(_femaleNodes[i].X, _femaleNodes[i].Y, _femaleNodes[i].Z + rads);
            }

            _maleNode = Vector3.Transform(_maleNode, Matrix.CreateRotationZ(rads));
            _maleNode = new Vector3(_maleNode.X, _maleNode.Y, _maleNode.Z + rads);
        }

        public ShipPartConfiguration Build()
        {
            if (_vertices.Count > 1)
                this.FlushVertices();

            return new ShipPartConfiguration(
                vertices: _savedVertices,
                maleConnectionNode: _maleNode,
                femaleConnectionNodes: _femaleNodes.ToArray());
        }

        public void SetMale(Vector3 male)
        {
            _maleNode = male;
        }

        public static ShipPartConfiguration BuildPolygon(Int32 sides, Boolean includeFemales = false)
        {
            if (sides < 3)
                throw new Exception("Unable to create polygon with less than three sides!");

            var builder = new ShipPartConfigurationBuilder();

            builder.AddSide(MathHelper.PiOver2, NodeType.Male);

            Single targetAngle = (sides == 3 ? MathHelper.TwoPi : MathHelper.TwoPi);
            Single stepAngle = targetAngle / sides;

            for (Int32 i = 0; i < sides - 1; i++)
                builder.AddSide(stepAngle, NodeType.Female);

            builder.TrimLast();
            return builder.Build();
        }
    }
}

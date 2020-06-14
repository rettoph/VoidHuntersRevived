using FarseerPhysics.Common;
using FarseerPhysics.Common.ConvexHull;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoidHuntersRevived.Library.Configurations
{
    /// <summary>
    /// The main configuration class for ship part
    /// instances. This defines the ShipPart vertices,
    /// & nodes
    /// </summary>
    public class ShipPartConfiguration
    {
        #region Flags
        [Flags]
        public enum NodeType
        {
            None = 1,
            Male = 2,
            Female = 4,
            Both = Male | Female
        }
        #endregion

        #region Buffers
        private Vertices _allVertices;
        private List<Vector2> _verticeBuffer;
        private List<ConnectionNodeConfiguration> _femaleBuffer;
        private ConnectionNodeConfiguration _maleBuffer;
        private Boolean _setHull;
        #endregion

        #region Values
        private List<Vertices> _vertices;
        private List<ConnectionNodeConfiguration> _females;
        private ConnectionNodeConfiguration _male;
        #endregion

        #region Public Values
        public IReadOnlyCollection<Vertices> Vertices { get => _vertices.AsReadOnly(); }
        public ConnectionNodeConfiguration MaleConnectionNode { get => _male; }
        public IReadOnlyCollection<ConnectionNodeConfiguration> FemaleConnectionNodes { get => _females.AsReadOnly(); }
        public Vector2 Centeroid { get; private set; }

        public Color DefaultColor { get; set; } = Color.Orange;

        public Vertices Hull { get; set; }
        #endregion

        #region Helper Fields
        public Single _rotation;
        #endregion

        #region Constructors
        public ShipPartConfiguration()
        {
            _allVertices = new Vertices();
            _verticeBuffer = new List<Vector2>();
            _femaleBuffer = new List<ConnectionNodeConfiguration>();

            _vertices = new List<Vertices>();
            _females = new List<ConnectionNodeConfiguration>();
            _male = new ConnectionNodeConfiguration()
            {
                Position = Vector2.Zero,
                Rotation = 0
            };
        }
        #endregion

        #region Raw Methods
        public void AddVertice(Vector2 vertice)
        {
            _verticeBuffer.Add(vertice);
        }
        public void AddVertice(Single x, Single y)
        {
            this.AddVertice(new Vector2(x, y));
        }

        /// <summary>
        /// Remove the most recent vertice, and optionally the most recent 
        /// female node too
        /// </summary>
        /// <param name="includeNode"></param>
        public void Remove(Boolean includeNode = false)
        {
            if (_verticeBuffer.Any())
                _verticeBuffer.RemoveAt(_verticeBuffer.Count() - 1);

            if (includeNode && _femaleBuffer.Any())
                _femaleBuffer.RemoveAt(_femaleBuffer.Count - 1);
        }

        public void AddNode(Vector2 position, Single rotation, NodeType type)
        {
            if (!type.HasFlag(NodeType.None))
            {
                var configuration = new ConnectionNodeConfiguration()
                {
                    Position = position,
                    Rotation = rotation
                };

                if (type.HasFlag(NodeType.Male))
                    _maleBuffer = configuration;
                if (type.HasFlag(NodeType.Female))
                    _femaleBuffer.Add(configuration);
            }
        }
        public void AddNode(Single x, Single y, Single rotation, NodeType type)
        {
            this.AddNode(new Vector2(x, y), rotation, type);
        }

        public ShipPartConfiguration Flush()
        {
            if (_verticeBuffer.Any())
            { // Flush the vertices...
                _vertices.Add(new Vertices(_verticeBuffer.ToArray()));
                _allVertices.AddRange(_verticeBuffer);
                _verticeBuffer.Clear();
            }

            if (_femaleBuffer.Any())
            { // Flush the females
                _females.AddRange(_femaleBuffer);
                _femaleBuffer.Clear();
            }

            if (_maleBuffer != null)
            { // Flush the male
                _male = _maleBuffer;
                _maleBuffer = null;
            }

            // Reset helper values...
            _rotation = 0;

            // Update internal values
            this.Centeroid = this.Vertices.SelectMany(v => v).Aggregate(Vector2.Zero, (s, v) => s + v) / this.Vertices.SelectMany(v => v).Count();

            // Update output hull...
            if(!_setHull)
                this.Hull = GiftWrap.GetConvexHull(_allVertices);

            return this;
        }

        public void Transform(Matrix tranformation)
        {
            // Update the stored vertices...
            _verticeBuffer = _verticeBuffer.Select(v => Vector2.Transform(v, tranformation)).ToList();

            // Update the stored female nodes...
            _femaleBuffer.ForEach(node => ConnectionNodeConfiguration.Transform(ref node, tranformation));

            // Update the stored male node if any
            if (_maleBuffer != null)
                ConnectionNodeConfiguration.Transform(ref _maleBuffer, tranformation);
        }

        public void Rotate(Single rotation)
        {
            this.Transform(Matrix.CreateRotationZ(rotation));
        }

        public void Clear()
        {
            _verticeBuffer.Clear();
            _femaleBuffer.Clear();
            _maleBuffer = null;

            _vertices = new List<Vertices>();
            _females = new List<ConnectionNodeConfiguration>();
            _male = null;
        }

        public void SetHull(Vertices vertices)
        {
            this.Hull = vertices;
            _setHull = true;
        }
        public void SetHull(Vector2[] vertices)
            => this.SetHull(new Vertices(vertices));
        #endregion

        #region Helper Methods
        /// <summary>
        /// Add a side with a length of 1, rotate relative to
        /// the last node's rotation.
        /// </summary>
        /// <param name="relativeRotation"></param>
        /// <param name="node"></param>
        public void AddSide(Single offsetRotation, NodeType node = NodeType.None)
        {
            // Ensure that at least one vertice is defined...
            if (_verticeBuffer.Count == 0)
                this.AddVertice(Vector2.Zero);

            _rotation = (_rotation + MathHelper.Pi) - offsetRotation;
            var matrix = Matrix.CreateRotationZ(_rotation);
            var last = _verticeBuffer.Last();

            // Add a new vertice
            this.AddVertice(last + Vector2.Transform(Vector2.UnitX, matrix));

            // Add the requested node types
            if (!node.HasFlag(NodeType.None))
            {
                var nPoint = last + Vector2.Transform(Vector2.UnitX / 2, matrix);
                if (node.HasFlag(NodeType.Male))
                    this.AddNode(nPoint, _rotation - MathHelper.PiOver2, NodeType.Male);
                if (node.HasFlag(NodeType.Female))
                    this.AddNode(nPoint, _rotation + MathHelper.PiOver2, NodeType.Female);
            }
        }

        public void AddPolygon(Single sides, NodeType nodeTypes = NodeType.Both)
        {
            if (sides < 3)
                throw new Exception("Unable to generate polygon with less than 3 sides!");

            var stepAngle = MathHelper.Pi - (MathHelper.TwoPi / sides);

            this.AddSide(0, nodeTypes.HasFlag(NodeType.Male) ? NodeType.Male : NodeType.None);

            for (Int32 i = 0; i < sides - 1; i++)
                this.AddSide(stepAngle, nodeTypes.HasFlag(NodeType.Female) ? NodeType.Female : NodeType.None);

            // Remove the last vertice
            this.Remove();
        }
        #endregion
    }
}

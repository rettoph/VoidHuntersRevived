using Guppy.Utilities.Primitives;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using tainicom.Aether.Physics2D.Common;
using tainicom.Aether.Physics2D.Common.ConvexHull;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.Controllers;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Utilities;

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
        /// <summary>
        /// A collection of all <see cref="Flush"/>'ed <see cref="Vertices"/>.
        /// </summary>
        public IReadOnlyCollection<Vertices> Vertices { get => _vertices.AsReadOnly(); }

        /// <summary>
        /// The primary *male* <see cref="ConnectionNode"/> of the current
        /// <see cref="ShipPart"/>. This is the node used when attaching to 
        /// another part.
        /// </summary>
        public ConnectionNodeConfiguration MaleConnectionNode { get => _male; }

        /// <summary>
        /// A collection of all *female* <see cref="ConnectionNode"/>s to be created
        /// within the <see cref="ShipPart"/>.
        /// </summary>
        public IReadOnlyCollection<ConnectionNodeConfiguration> FemaleConnectionNodes { get => _females.AsReadOnly(); }

        /// <summary>
        /// A custom defined centeroid for the current <see cref="Vertices"/>.
        /// </summary>
        public Vector2 Centeroid { get; private set; }

        /// <summary>
        /// The default <see cref="Color"/> to render the 
        /// <see cref="ShipPart"/> when not attached to any
        /// <see cref="Ship"/> or <see cref="Chain"/>
        /// </summary>
        public Color DefaultColor { get; set; } = Color.Orange;

        /// <summary>
        /// A traced outline of the defined <see cref="Vertices"/>.
        /// This is primarily used when multiple shapes reside within
        /// a single <see cref="ShipPart"/>.
        /// </summary>
        public Vertices Hull { get; private set; }

        /// <summary>
        /// The default density to use when creating a 
        /// new <see cref="Fixture"/>.
        /// </summary>
        public Single Density { get; set; } = 0.5f;

        /// <summary>
        /// The maximum <see cref="ShipPart.Health"/> value
        /// for the defined <see cref="ShipPart"/>.
        /// </summary>
        public Single MaxHealth { get; set; } = 100f;
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
        /// <summary>
        /// Add a single vertex to the current <see cref="Vertices"/>
        /// buffer.
        /// </summary>
        /// <param name="vertex"></param>
        public void AddVertex(Vector2 vertex)
        {
            _verticeBuffer.Add(vertex);
        }
        /// <summary>
        /// Add a single vertex to the current <see cref="Vertices"/>
        /// buffer.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void AddVertex(Single x, Single y)
            => this.AddVertex(new Vector2(x, y));

        /// <summary>
        /// Remove the most recent vertex, and optionally the most recent 
        /// female <see cref="ConnectionNode"/> too
        /// </summary>
        /// <param name="removeNode"></param>
        public void Remove(Boolean removeNode = false)
        {
            if (_verticeBuffer.Any())
                _verticeBuffer.RemoveAt(_verticeBuffer.Count() - 1);

            if (removeNode && _femaleBuffer.Any())
                _femaleBuffer.RemoveAt(_femaleBuffer.Count - 1);
        }

        /// <summary>
        /// Add a brand new <see cref="ConnectionNode"/>.
        /// If <paramref name="type"/> is <see cref="NodeType.Male"/>
        /// then the <see cref="MaleConnectionNode"/> value will be
        /// overwritten.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="type"></param>
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
        /// <summary>
        /// Add a brand new <see cref="ConnectionNode"/>.
        /// If <paramref name="type"/> is <see cref="NodeType.Male"/>
        /// then the <see cref="MaleConnectionNode"/> value will be
        /// overwritten.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="rotation"></param>
        /// <param name="type"></param>
        public void AddNode(Single x, Single y, Single rotation, NodeType type)
        {
            this.AddNode(new Vector2(x, y), rotation, type);
        }

        /// <summary>
        /// Flush all buffered <see cref="Vertices"/> & <see cref="ConnectionNode"/>s.
        /// Effectively locking in any changes made since the last <see cref="Flush"/>
        /// invocation.
        /// </summary>
        /// <returns></returns>
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
            if (!_setHull)
                this.SetHull(GiftWrap.GetConvexHull(_allVertices));

            return this;
        }

        /// <summary>
        /// Apply a transformation to the current buffer values.
        /// This includes <see cref="ConnectionNode"/>s &
        /// <see cref="Vertices"/>.
        /// </summary>
        /// <param name="tranformation"></param>
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

        /// <summary>
        /// Rotate the current buffer values.
        /// This includes <see cref="ConnectionNode"/>s &
        /// <see cref="Vertices"/>.
        /// </summary>
        /// <param name="rotation"></param>
        public void Rotate(Single rotation)
            => this.Transform(Matrix.CreateRotationZ(rotation));

        /// <summary>
        /// Clear the entire configuration.
        /// </summary>
        public void Clear()
        {
            this.ClearBuffer();

            _vertices = new List<Vertices>();
            _females = new List<ConnectionNodeConfiguration>();
            _male = null;
        }

        /// <summary>
        /// Clear only the values buffered so far
        /// </summary>
        public void ClearBuffer()
        {
            _verticeBuffer.Clear();
            _femaleBuffer.Clear();
            _maleBuffer = null;
        }

        /// <summary>
        /// Manually define the <see cref="Hull"/> value.
        /// If called an automated <see cref="Hull"/> will
        /// no longer be developed.
        /// </summary>
        /// <param name="vertices"></param>
        public void SetHull(Vertices vertices)
        {
            this.Hull = vertices;
            _setHull = true;
        }

        /// <summary>
        /// Manually define the <see cref="Hull"/> value.
        /// If called an automated <see cref="Hull"/> will
        /// no longer be developed.
        /// </summary>
        /// <param name="vertices"></param>
        public void SetHull(Vector2[] vertices)
            => this.SetHull(new Vertices(vertices));
        #endregion

        #region Helper Methods
        /// <summary>
        /// Add a side with a length of <paramref name="length"/>, rotate relative to
        /// the last side's rotation.
        /// </summary>
        /// <param name="relativeRotation"></param>
        /// <param name="node"></param>
        public void AddSide(Single offsetRotation, NodeType node = NodeType.None, Single length = 1)
        {
            // Ensure that at least one vertice is defined...
            if (_verticeBuffer.Count == 0)
                this.AddVertex(Vector2.Zero);

            _rotation = (_rotation + MathHelper.Pi) - offsetRotation;
            var matrix = Matrix.CreateRotationZ(_rotation);
            var last = _verticeBuffer.Last();

            // Add a new vertice
            var side = Vector2.UnitX * length;
            this.AddVertex(last + Vector2.Transform(side, matrix));

            // Add the requested node types
            if (!node.HasFlag(NodeType.None))
            {
                var nPoint = last + Vector2.Transform(side / 2, matrix);
                if (node.HasFlag(NodeType.Male))
                    this.AddNode(nPoint, _rotation - MathHelper.PiOver2, NodeType.Male);
                if (node.HasFlag(NodeType.Female))
                    this.AddNode(nPoint, _rotation + MathHelper.PiOver2, NodeType.Female);
            }
        }

        /// <summary>
        /// Create a regular polygon.
        /// </summary>
        /// <param name="sides">The number of sides to add.</param>
        /// <param name="nodeTypes">The nodes to include.</param>
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

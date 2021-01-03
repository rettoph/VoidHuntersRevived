using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using tainicom.Aether.Physics2D.Common;
using tainicom.Aether.Physics2D.Common.ConvexHull;
using VoidHuntersRevived.Library.Utilities;
using static VoidHuntersRevived.Library.Contexts.ShipPartShapeContext;

namespace VoidHuntersRevived.Library.Contexts
{
    public class ShipPartShapesContext : IEnumerable<ShipPartShapeContext>
    {
        #region Private Fields
        private List<ShipPartShapeContext> _shapes;
        private Boolean _customHull;
        #endregion

        #region Public Properties
        /// <summary>
        /// A custom defined centeroid for the current <see cref="Vertices"/>.
        /// </summary>
        public Vector2 Centeroid { get; private set; }

        /// <summary>
        /// The *lastmode* defined male connection node.
        /// Note, male connection nodes can be manually overwritten
        /// in newer shapes. This is bad practice, but for now it
        /// works. Just make sure you define the male connection
        /// node in the last shape to be sure it never gets overwritten.
        /// </summary>
        public ConnectionNodeContext MaleConnectionNode { get; private set; }

        /// <summary>
        /// A traced outline of the defined <see cref="Vertices"/>.
        /// This is primarily used when multiple shapes reside within
        /// a single <see cref="ShipPart"/>.
        /// </summary>
        public Vertices Hull { get; private set; }
        #endregion

        #region Constructors
        internal ShipPartShapesContext()
        {
            _shapes = new List<ShipPartShapeContext>();
        }
        #endregion

        #region API Methods
        /// <summary>
        /// Attempt to build & add a new shape
        /// </summary>
        /// <param name="build"></param>
        public Boolean TryAdd(Action<ShipPartShapeContextBuilder> build)
        {
            var builder = new ShipPartShapeContextBuilder();
            build(builder);

            _shapes.Add(builder.Build());

            // Update internal values
            this.Centeroid = _shapes.SelectMany(s => s.Vertices).Aggregate(Vector2.Zero, (s, v) => s + v) / _shapes.SelectMany(s => s.Vertices).Count();
            this.MaleConnectionNode = new ConnectionNodeContext()
            {
                Position = Vector2.UnitX * -0.5f,
                Rotation = -MathHelper.PiOver2
            };

            // Update output hull...
            if (!_customHull)
                this.Hull = GiftWrap.GetConvexHull(new Vertices(_shapes.SelectMany(s => s.Vertices)));

            return true;
        }


        /// <summary>
        /// Create a regular polygon.
        /// </summary>
        /// <param name="sides"></param>
        /// <param name="withFemales"></param>
        /// <param name="build"></param>
        public void AddPolygon(Single sides, Boolean withFemales = true, Action<ShipPartShapeContextBuilder> build = null)
        {
            if (sides < 3)
                throw new Exception("Unable to generate polygon with less than 3 sides!");

            var stepAngle = MathHelper.Pi - (MathHelper.TwoPi / sides);

            this.TryAdd(builder =>
            {
                for (Int32 i = 0; i < sides; i++)
                {
                    builder.AddSide(stepAngle, withFemales);
                }

                build?.Invoke(builder);
            });
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
            _customHull = true;
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

        #region Serialization Methods
        internal void Read(BinaryReader reader)
        {
            this.MaleConnectionNode = new ConnectionNodeContext()
            {
                Position = new Vector2(reader.ReadSingle(), reader.ReadSingle()),
                Rotation = reader.ReadSingle()
            };

            var shapes = reader.ReadInt32();
            for (Int32 i = 0; i < shapes; i++)
            {
                this.TryAdd(builder =>
                {
                    builder.Scale = reader.ReadSingle();
                    builder.Translation = new Vector2(reader.ReadSingle(), reader.ReadSingle());
                    builder.Rotation = reader.ReadSingle();

                    var sides = reader.ReadInt32();

                    for(Int32 ii = 0; ii < sides; ii++)
                    {
                        builder.AddSide(
                            rotation: reader.ReadSingle(),
                            includeFemales: reader.ReadBoolean(),
                            length: reader.ReadSingle());
                    }
                });
            }
        }

        internal void Write(BinaryWriter writer)
        {
            writer.Write(this.MaleConnectionNode.Position.X);
            writer.Write(this.MaleConnectionNode.Position.Y);
            writer.Write(this.MaleConnectionNode.Rotation);

            writer.Write(_shapes.Count);
            foreach (ShipPartShapeContext shape in this)
            {
                writer.Write(shape.Scale);
                writer.Write(shape.Translation.X);
                writer.Write(shape.Translation.Y);
                writer.Write(shape.Rotation);

                writer.Write(shape.Sides.Length);
                foreach(SideContext side in shape.Sides)
                {
                    writer.Write(side.Rotation);
                    writer.Write(side.IncludeFemales);
                    writer.Write(side.Length);
                }
            }
        }
        #endregion

        #region IEnumerable<ShipPartShapeContext> Implementation
        public IEnumerator<ShipPartShapeContext> GetEnumerator()
            => _shapes.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => _shapes.GetEnumerator();
        #endregion
    }
}

using Guppy.Extensions.Microsoft.Xna.Framework;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using tainicom.Aether.Physics2D.Common;
using VoidHuntersRevived.Library.Contexts;
using VoidHuntersRevived.Library.Extensions.Microsoft.Xna;
using static VoidHuntersRevived.Library.Contexts.ShipPartShapeContext;

namespace VoidHuntersRevived.Library.Utilities
{
    /// <summary>
    /// A simple utility that will allow us to easily create <see cref="ShipPartShape"/>
    /// definitions without needing to recalculate the vertece data each update.
    /// 
    /// This should only be used to manually create Shape data, as most other methods are automated
    /// at this point.
    /// </summary>
    public class ShipPartShapeContextBuilder
    {
        #region Private Fields
        private List<SideContext> _sides;
        #endregion

        #region Public Properties
        /// <summary>
        /// The scale transformation applied to the defined sides.
        /// </summary>
        public Single Scale { get; set; }

        /// <summary>
        /// The translation transformation applied to the defined sides.
        /// </summary>
        public Vector2 Translation { get; set; }

        /// <summary>
        /// The rotation transformation applied to the defined sides.
        /// </summary>
        public Single Rotation { get; set; }

        /// <summary>
        /// The default density to use when creating a 
        /// new <see cref="Fixture"/>.
        /// </summary>
        public Single Density { get; set; }
        #endregion

        #region Constructors
        internal ShipPartShapeContextBuilder()
        {
            _sides = new List<SideContext>();
            this.Reset();
        }
        #endregion

        #region API Methods
        /// <summary>
        /// Add a new side into the shape output relative
        /// to the previously added side.
        /// </summary>
        /// <param name="rotation"></param>
        /// <param name="includeFemales"></param>
        /// <param name="length"></param>
        public void AddSide(Single rotation, Boolean includeFemales = true, Single length = 1)
        {
            _sides.Add(new SideContext()
            {
                Rotation = rotation,
                Length = length,
                IncludeFemales = includeFemales
            });
        }

        /// <summary>
        /// Take the internal <see cref="SideContext"/> data
        /// and attempt to construct a vaild ship part shape.
        /// </summary>
        /// <returns></returns>
        public ShipPartShapeContext Build()
        {
            if (_sides.Count < 3)
                throw new Exception("Unable to create shape, not enough sides defined.");

            Single currentRotation = 0f;
            Vector2 currentPosition = Vector2.Zero;
            Vector2 lastPosition = Vector2.Zero;
            List<ConnectionNodeContext> females = new List<ConnectionNodeContext>();
            List<Vector2> vertices = new List<Vector2>();
            vertices.Add(Vector2.Zero);

            foreach(SideContext side in _sides)
            { // Iterate through each side defined & attempt to calculate the vertice data.
                currentRotation = (MathHelper.Pi + currentRotation) - side.Rotation;
                currentPosition = lastPosition + (Vector2.UnitX * side.Length).RotateTo(currentRotation);

                if(side.IncludeFemales)
                { // Calculate the female node positions now.
                    var count = (Single)Math.Ceiling(side.Length) + 1; // The number of female nodes to be added.
                    var spacing = side.Length / count; // The distance beween each female node on this side.

                    for(var i=1; i<count; i++)
                    { // Calculate each node & add it in.
                        females.Add(new ConnectionNodeContext()
                        {
                            Position = lastPosition + (Vector2.UnitX * (spacing * i)).RotateTo(currentRotation),
                            Rotation = currentRotation + MathHelper.PiOver2
                        });
                    }
                }

                if(Vector2.Distance(vertices.First(), currentPosition) > 0.0001f)
                { // Only bother adding the side if its not "closing" the shape.
                    vertices.Add(currentPosition);
                    lastPosition = currentPosition; // Update the last position.
                }
            }

            Matrix translation = Matrix.CreateScale(this.Scale) 
                * Matrix.CreateRotationZ(this.Rotation) 
                * Matrix.CreateTranslation(this.Translation.ToVector3());

            return new ShipPartShapeContext(
                vertices: new Vertices(vertices.Select(v => Vector2.Transform(v, translation)).ToArray()),
                females: females.Select(f => ConnectionNodeContext.Transform(f, translation)).ToArray(),
                sides: _sides.ToArray(),
                scale: this.Scale,
                translation: this.Translation,
                rotation: this.Rotation,
                density: this.Density);
        }

        public void Reset()
        {
            _sides.Clear();

            this.Scale = 1f;
            this.Translation = Vector2.Zero;
            this.Rotation = 0f;
            this.Density = 0.5f;
        }
        #endregion
    }
}

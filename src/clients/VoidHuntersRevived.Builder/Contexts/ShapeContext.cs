using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Guppy.Extensions.System.Collections;
using Guppy.Extensions.Microsoft.Xna.Framework;
using System.Linq;
using tainicom.Aether.Physics2D;
using Guppy.Events.Delegates;
using tainicom.Aether.Physics2D.Common;

namespace VoidHuntersRevived.Builder.Contexts
{
    public class ShapeContext
    {
        #region Private Fields
        private List<SideContext> _sides = new List<SideContext>();
        #endregion

        #region Public Fields

        public Vector2 Translation;
        public Single Rotation;
        public Single Scale = 1;
        #endregion

        #region Public Properties
        public Matrix TransformationMatrix => Matrix.CreateTranslation(this.Translation.X, this.Translation.Y, 0);

        public IReadOnlyList<SideContext> Sides => _sides;

        /// <summary>
        /// Returns the world rotation of the
        /// last side within the current shape.
        /// </summary>
        public Single LastWorldRotation => this.GetWorldRotation(_sides.Count);
        #endregion

        #region Methods
        /// <summary>
        /// Calculate the world coordinates of the internally
        /// defined transformations & sides.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Vector2> GetVertices()
        {
            var transformation = this.TransformationMatrix;
            var rotation = 0f;
            Vector2 lastVertex = Vector2.Zero;

            yield return Vector2.Transform(
                position: lastVertex,
                matrix: transformation);

            foreach (SideContext side in this.Sides)
            {
                rotation += MathHelper.Pi - side.Rotation;
                lastVertex += side.Length.ToVector2().RotateTo(rotation);

                yield return Vector2.Transform(
                    position: lastVertex,
                    matrix: transformation);
            }
        }

        /// <summary>
        /// Attempt to add a new side into the shape.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Boolean TryAddSide(SideContext context)
        {
            if (context.Rotation < 0 || context.Rotation > MathHelper.Pi)
                return false; // All rotation inputs must be between 0 & 180 degrees to ensure a convex shape.
            if (context.Length < Settings.Epsilon)
                return false; // There must be some length to the side...
            if (this.GetVertices().Count() >= Settings.MaxPolygonVertices)
                return false;

            // Add the side into the shape...
            _sides.Add(context); 

            // Check if the side is valid...
            var vertices = this.GetVertices().ToList(); // Calculate the "would be" vertices if the side is added.

            // Ensure the side is not manually closed.
            if(Vector2.Distance(vertices.First(), vertices.Last()) < 0.001f)
            { // The newest side "closes" the shape manually? This should never happen.
                _sides.RemoveAt(_sides.Count() - 1); // Remove the last side

                return false;
            }

            // If we make it this far the side is valid. Just return true.
            return true;
        }
        /// <summary>
        /// Attempt to add a new side into the shape.
        /// </summary>
        /// <param name="length"></param>
        /// <param name="rotation"></param>
        public Boolean TryAddSide(Single length, Single rotation)
            => this.TryAddSide(new SideContext()
            {
                Length = length,
                Rotation = rotation
            });

        /// <summary>
        /// Attempt to remove the most recently added side.
        /// </summary>
        /// <returns></returns>
        public Boolean TryRemoveSide()
        {
            if (!_sides.Any())
                return false;

            _sides.RemoveAt(_sides.Count() - 1); // Remove the last side
            return true;
        }

        public Single GetWorldRotation(Int32 index)
            => MathHelper.WrapAngle(_sides.SkipLast(_sides.Count - index).Select(s => MathHelper.Pi - s.Rotation).TryAggregate((world, side) => world + side, 0));

        /// <summary>
        /// Returns all interest points reguarding the current shape.
        /// Interest points represent points where we believe you may want
        /// to attach nodes or take edges.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Vector2> GetInterestPoints()
        {
            if (this.Sides.Any())
            {
                var verts = this.GetVertices();
                var count = verts.Count();
                Vector2 start;
                Vector2 end;
                Single length;
                Single spacing;

                yield return verts.TryAggregate((t, v) => t + v) / count;

                for (var i = 0; i < count; i++)
                {
                    start = verts.ElementAt(i);
                    end = verts.ElementAt((i + 1) % count);
                    length = (Single)Math.Ceiling(Vector2.Distance(start, end));
                    spacing = Vector2.Distance(start, end) / (length * 2);

                    yield return start;

                    // Calculate all interst ponts along the side edge.
                    for (var ii = 1; ii < length * 2; ii++)
                        yield return start + (Vector2.UnitX * spacing * ii).RotateTo(end.Angle(start));
                }
            }
        }
        #endregion
    }
}

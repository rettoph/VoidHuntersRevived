// MIT License - Copyright (C) The Mono.Xna Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Runtime.Serialization;

namespace VoidHuntersRevived.Common.FixedPoint.FixedPoint
{
    /// <summary>
    /// Describes a 2D-FixRectangle. 
    /// </summary>
    public struct FixRectangle
    {
        private static FixRectangle emptyFixRectangle = new FixRectangle();


        /// <summary>
        /// The x coordinate of the top-left corner of this <see cref="FixRectangle"/>.
        /// </summary>
        [DataMember]
        public Fix64 X;

        /// <summary>
        /// The y coordinate of the top-left corner of this <see cref="FixRectangle"/>.
        /// </summary>
        [DataMember]
        public Fix64 Y;

        /// <summary>
        /// The width of this <see cref="FixRectangle"/>.
        /// </summary>
        [DataMember]
        public Fix64 Width;

        /// <summary>
        /// The height of this <see cref="FixRectangle"/>.
        /// </summary>
        [DataMember]
        public Fix64 Height;

        /// <summary>
        /// Returns a <see cref="FixRectangle"/> with X=0, Y=0, Width=0, Height=0.
        /// </summary>
        public static FixRectangle Empty
        {
            get { return emptyFixRectangle; }
        }

        /// <summary>
        /// Returns the x coordinate of the left edge of this <see cref="FixRectangle"/>.
        /// </summary>
        public Fix64 Left
        {
            get { return this.X; }
        }

        /// <summary>
        /// Returns the x coordinate of the right edge of this <see cref="FixRectangle"/>.
        /// </summary>
        public Fix64 Right
        {
            get { return (this.X + this.Width); }
        }

        /// <summary>
        /// Returns the y coordinate of the top edge of this <see cref="FixRectangle"/>.
        /// </summary>
        public Fix64 Top
        {
            get { return this.Y; }
        }

        /// <summary>
        /// Returns the y coordinate of the bottom edge of this <see cref="FixRectangle"/>.
        /// </summary>
        public Fix64 Bottom
        {
            get { return (this.Y + this.Height); }
        }

        /// <summary>
        /// Creates a new instance of <see cref="FixRectangle"/> struct, with the specified
        /// position, width, and height.
        /// </summary>
        /// <param name="x">The x coordinate of the top-left corner of the created <see cref="FixRectangle"/>.</param>
        /// <param name="y">The y coordinate of the top-left corner of the created <see cref="FixRectangle"/>.</param>
        /// <param name="width">The width of the created <see cref="FixRectangle"/>.</param>
        /// <param name="height">The height of the created <see cref="FixRectangle"/>.</param>
        public FixRectangle(Fix64 x, Fix64 y, Fix64 width, Fix64 height)
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
        }

        public bool Contains(FixMatrix transformation)
        {
            return this.Left < transformation.M41 && transformation.M41 < this.Right
                && this.Top < transformation.M42 && transformation.M42 < this.Bottom;
        }

        public bool Contains(FixVector2 position)
        {
            return this.Left < position.X && position.X < this.Right
                && this.Top < position.Y && position.Y < this.Bottom;
        }
    }
}

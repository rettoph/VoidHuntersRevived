using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.Utilities
{
    /// <summary>
    /// Stolen from:
    /// https://github.com/47th/Monogame/blob/master/Engine/Util/RectangleF.cs
    /// 
    /// Rectangle that supports float values
    /// </summary>
    public struct RectangleF
    {
        public Vector2 Min;
        public Vector2 Max;

        public float Left { get { return this.Min.X; } }
        public float Right { get { return this.Max.X; } }
        public float Top { get { return this.Max.Y; } }
        public float Bottom { get { return this.Min.Y; } }

        public float Width { get { return this.Max.X - this.Min.X; } }
        public float Height { get { return this.Max.Y - this.Min.Y; } }

        private static readonly RectangleF _empty;
        private static readonly RectangleF _minMax;

        static RectangleF()
        {
            _empty = new RectangleF();
            _minMax = new RectangleF(Vector2.One * float.MinValue, Vector2.One * float.MaxValue);
        }

        public Vector2 Center
        {
            get { return (Min + Max) / 2; }
        }

        public static RectangleF Empty
        {
            get { return _empty; }
        }


        public static RectangleF MinMax
        {
            get { return _minMax; }
        }

        public bool IsZero
        {
            get
            {
                return
                    (this.Min.X == 0) &&
                    (this.Min.Y == 0) &&
                    (this.Max.X == 0) &&
                    (this.Max.Y == 0);
            }
        }

        public RectangleF(float x, float y, float width, float height)
        {
            Min.X = x;
            Min.Y = y;
            Max.X = x + width;
            Max.Y = y + height;
        }

        public RectangleF(Vector2 min, Vector2 max)
        {
            Min = min;
            Max = max;
        }

        public bool Contains(float x, float y)
        {
            return
                (Min.X <= x) &&
                (Min.Y <= y) &&
                (Max.X >= x) &&
                (Max.Y >= y);
        }

        public bool Contains(Vector2 vector)
        {
            return
                (Min.X <= vector.X) &&
                (Min.Y <= vector.Y) &&
                (Max.X >= vector.X) &&
                (Max.Y >= vector.Y);
        }

        public void Contains(ref Vector2 rect, out bool result)
        {
            result =
                (Min.X <= rect.X) &&
                (Min.Y <= rect.Y) &&
                (Max.X >= rect.X) &&
                (Max.Y >= rect.Y);
        }

        public bool Contains(RectangleF rect)
        {
            return
                (Min.X <= rect.Min.X) &&
                (Min.Y <= rect.Min.Y) &&
                (Max.X >= rect.Max.X) &&
                (Max.Y >= rect.Max.Y);
        }

        public void Contains(ref RectangleF rect, out bool result)
        {
            result =
                (Min.X <= rect.Min.X) &&
                (Min.Y <= rect.Min.Y) &&
                (Max.X >= rect.Max.X) &&
                (Max.Y >= rect.Max.Y);
        }

        public bool Intersects(RectangleF rect)
        {
            return
                (Min.X < rect.Max.X) &&
                (Min.Y < rect.Max.Y) &&
                (Max.X > rect.Min.X) &&
                (Max.Y > rect.Min.Y);
        }

        public bool Intersects(Vector2 position)
        {
            return
                (Min.X < position.X) &&
                (Min.Y < position.Y) &&
                (Max.X > position.X) &&
                (Max.Y > position.Y);
        }

        public void Intersects(ref RectangleF rect, out bool result)
        {
            result =
                (Min.X < rect.Max.X) &&
                (Min.Y < rect.Max.Y) &&
                (Max.X > rect.Min.X) &&
                (Max.Y > rect.Min.Y);
        }

        public static RectangleF Intersect(RectangleF rect1, RectangleF rect2)
        {
            RectangleF result;

            float num8 = rect1.Max.X;
            float num7 = rect2.Max.X;
            float num6 = rect1.Max.Y;
            float num5 = rect2.Max.Y;
            float num2 = (rect1.Min.X > rect2.Min.X) ? rect1.Min.X : rect2.Min.X;
            float num = (rect1.Min.Y > rect2.Min.Y) ? rect1.Min.Y : rect2.Min.Y;
            float num4 = (num8 < num7) ? num8 : num7;
            float num3 = (num6 < num5) ? num6 : num5;

            if ((num4 > num2) && (num3 > num))
            {
                result.Min.X = num2;
                result.Min.Y = num;
                result.Max.X = num4;
                result.Max.Y = num3;

                return result;
            }

            result.Min.X = 0;
            result.Min.Y = 0;
            result.Max.X = 0;
            result.Max.Y = 0;

            return result;
        }

        public static void Intersect(ref RectangleF rect1, ref RectangleF rect2, out RectangleF result)
        {
            float num8 = rect1.Max.X;
            float num7 = rect2.Max.X;
            float num6 = rect1.Max.Y;
            float num5 = rect2.Max.Y;
            float num2 = (rect1.Min.X > rect2.Min.X) ? rect1.Min.X : rect2.Min.X;
            float num = (rect1.Min.Y > rect2.Min.Y) ? rect1.Min.Y : rect2.Min.Y;
            float num4 = (num8 < num7) ? num8 : num7;
            float num3 = (num6 < num5) ? num6 : num5;

            if ((num4 > num2) && (num3 > num))
            {
                result.Min.X = num2;
                result.Min.Y = num;
                result.Max.X = num4;
                result.Max.Y = num3;
            }

            result.Min.X = 0;
            result.Min.Y = 0;
            result.Max.X = 0;
            result.Max.Y = 0;
        }

        public static RectangleF Union(RectangleF rect1, RectangleF rect2)
        {
            RectangleF result;

            float num6 = rect1.Max.X;
            float num5 = rect2.Max.X;
            float num4 = rect1.Max.Y;
            float num3 = rect2.Max.Y;
            float num2 = (rect1.Min.X < rect2.Min.X) ? rect1.Min.X : rect2.Min.X;
            float num = (rect1.Min.Y < rect2.Min.Y) ? rect1.Min.Y : rect2.Min.Y;
            float num8 = (num6 > num5) ? num6 : num5;
            float num7 = (num4 > num3) ? num4 : num3;

            result.Min.X = num2;
            result.Min.Y = num;
            result.Max.X = num8;
            result.Max.Y = num7;

            return result;
        }

        public static void Union(ref RectangleF rect1, ref RectangleF rect2, out RectangleF result)
        {
            float num6 = rect1.Max.X;
            float num5 = rect2.Max.X;
            float num4 = rect1.Max.Y;
            float num3 = rect2.Max.Y;
            float num2 = (rect1.Min.X < rect2.Min.X) ? rect1.Min.X : rect2.Min.X;
            float num = (rect1.Min.Y < rect2.Min.Y) ? rect1.Min.Y : rect2.Min.Y;
            float num8 = (num6 > num5) ? num6 : num5;
            float num7 = (num4 > num3) ? num4 : num3;

            result.Min.X = num2;
            result.Min.Y = num;
            result.Max.X = num8;
            result.Max.Y = num7;
        }

        public bool Equals(RectangleF other)
        {
            return
                (Min.X == other.Min.X) &&
                (Min.Y == other.Min.Y) &&
                (Max.X == other.Max.X) &&
                (Max.Y == other.Max.Y);
        }

        public override int GetHashCode()
        {
            return Min.GetHashCode() + Max.GetHashCode();
        }

        public static bool operator ==(RectangleF a, RectangleF b)
        {
            return
                (a.Min.X == b.Min.X) &&
                (a.Min.Y == b.Min.Y) &&
                (a.Max.X == b.Max.X) &&
                (a.Max.Y == b.Max.Y);
        }

        public static bool operator !=(RectangleF a, RectangleF b)
        {
            return
                (a.Min.X != b.Min.X) ||
                (a.Min.Y != b.Min.Y) ||
                (a.Max.X != b.Max.X) ||
                (a.Max.Y != b.Max.Y);
        }

        public static RectangleF operator +(RectangleF rect, Vector2 v)
        {
            return new RectangleF(rect.Min + v, rect.Max + v);
        }

        public override bool Equals(object obj)
        {
            if (obj is RectangleF)
            {
                return this == (RectangleF)obj;
            }

            return false;
        }
    }
}

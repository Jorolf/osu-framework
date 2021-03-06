﻿// Copyright (c) 2007-2017 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu-framework/master/LICENCE

using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using OpenTK;

namespace osu.Framework.Graphics.Primitives
{
    /// <summary>Stores a set of four floating-point numbers that represent the location and size of a rectangle. For more advanced region functions, use a <see cref="T:System.Drawing.Region"></see> object.</summary>
    /// <filterpriority>1</filterpriority>
    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct RectangleF
    {
        /// <summary>Represents an instance of the <see cref="T:System.Drawing.RectangleF"></see> class with its members uninitialized.</summary>
        /// <filterpriority>1</filterpriority>
        public static readonly RectangleF Empty;

        public float X;
        public float Y;

        public float Width;
        public float Height;

        /// <summary>Initializes a new instance of the <see cref="T:System.Drawing.RectangleF"></see> class with the specified location and size.</summary>
        /// <param name="y">The y-coordinate of the upper-left corner of the rectangle. </param>
        /// <param name="width">The width of the rectangle. </param>
        /// <param name="height">The height of the rectangle. </param>
        /// <param name="x">The x-coordinate of the upper-left corner of the rectangle. </param>
        public RectangleF(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Drawing.RectangleF"></see> class with the specified location and size.</summary>
        /// <param name="size">A <see cref="T:System.Drawing.SizeF"></see> that represents the width and height of the rectangular region. </param>
        /// <param name="location">A <see cref="T:System.Drawing.PointF"></see> that represents the upper-left corner of the rectangular region. </param>
        public RectangleF(PointF location, SizeF size)
        {
            X = location.X;
            Y = location.Y;
            Width = size.Width;
            Height = size.Height;
        }

        /// <summary>Creates a <see cref="T:System.Drawing.RectangleF"></see> structure with upper-left corner and lower-right corner at the specified locations.</summary>
        /// <returns>The new <see cref="T:System.Drawing.RectangleF"></see> that this method creates.</returns>
        /// <param name="right">The x-coordinate of the lower-right corner of the rectangular region. </param>
        /// <param name="bottom">The y-coordinate of the lower-right corner of the rectangular region. </param>
        /// <param name="left">The x-coordinate of the upper-left corner of the rectangular region. </param>
        /// <param name="top">The y-coordinate of the upper-left corner of the rectangular region. </param>
        /// <filterpriority>1</filterpriority>
        public static RectangleF FromLTRB(float left, float top, float right, float bottom) => new RectangleF(left, top, right - left, bottom - top);

        /// <summary>Gets or sets the coordinates of the upper-left corner of this <see cref="T:System.Drawing.RectangleF"></see> structure.</summary>
        /// <returns>A <see cref="T:System.Drawing.PointF"></see> that represents the upper-left corner of this <see cref="T:System.Drawing.RectangleF"></see> structure.</returns>
        /// <filterpriority>1</filterpriority>
        [Browsable(false)]
        public PointF Location
        {
            get { return new PointF(X, Y); }
            set
            {
                X = value.X;
                Y = value.Y;
            }
        }

        /// <summary>Gets or sets the size of this <see cref="T:System.Drawing.RectangleF"></see>.</summary>
        /// <returns>A <see cref="T:System.Drawing.SizeF"></see> that represents the width and height of this <see cref="T:System.Drawing.RectangleF"></see> structure.</returns>
        /// <filterpriority>1</filterpriority>
        [Browsable(false)]
        public Vector2 Size
        {
            get { return new Vector2(Width, Height); }
            set
            {
                Width = value.X;
                Height = value.Y;
            }
        }

        /// <summary>Gets the y-coordinate of the top edge of this <see cref="T:System.Drawing.RectangleF"></see> structure.</summary>
        /// <returns>The y-coordinate of the top edge of this <see cref="T:System.Drawing.RectangleF"></see> structure.</returns>
        /// <filterpriority>1</filterpriority>
        [Browsable(false)]
        public float Left => X;

        /// <summary>Gets the y-coordinate of the top edge of this <see cref="T:System.Drawing.RectangleF"></see> structure.</summary>
        /// <returns>The y-coordinate of the top edge of this <see cref="T:System.Drawing.RectangleF"></see> structure.</returns>
        /// <filterpriority>1</filterpriority>
        [Browsable(false)]
        public float Top => Y;

        /// <summary>Gets the x-coordinate that is the sum of <see cref="P:System.Drawing.RectangleF.X"></see> and <see cref="P:System.Drawing.RectangleF.Width"></see> of this <see cref="T:System.Drawing.RectangleF"></see> structure.</summary>
        /// <returns>The x-coordinate that is the sum of <see cref="P:System.Drawing.RectangleF.X"></see> and <see cref="P:System.Drawing.RectangleF.Width"></see> of this <see cref="T:System.Drawing.RectangleF"></see> structure.</returns>
        /// <filterpriority>1</filterpriority>
        [Browsable(false)]
        public float Right => X + Width;

        /// <summary>Gets the y-coordinate that is the sum of <see cref="P:System.Drawing.RectangleF.Y"></see> and <see cref="P:System.Drawing.RectangleF.Height"></see> of this <see cref="T:System.Drawing.RectangleF"></see> structure.</summary>
        /// <returns>The y-coordinate that is the sum of <see cref="P:System.Drawing.RectangleF.Y"></see> and <see cref="P:System.Drawing.RectangleF.Height"></see> of this <see cref="T:System.Drawing.RectangleF"></see> structure.</returns>
        /// <filterpriority>1</filterpriority>
        [Browsable(false)]
        public float Bottom => Y + Height;

        [Browsable(false)]
        public Vector2 TopLeft => new Vector2(Left, Top);

        [Browsable(false)]
        public Vector2 TopRight => new Vector2(Right, Top);

        [Browsable(false)]
        public Vector2 BottomLeft => new Vector2(Left, Bottom);

        [Browsable(false)]
        public Vector2 BottomRight => new Vector2(Right, Bottom);

        /// <summary>Tests whether the <see cref="P:System.Drawing.RectangleF.Width"></see> or <see cref="P:System.Drawing.RectangleF.Height"></see> property of this <see cref="T:System.Drawing.RectangleF"></see> has a value of zero.</summary>
        /// <returns>This property returns true if the <see cref="P:System.Drawing.RectangleF.Width"></see> or <see cref="P:System.Drawing.RectangleF.Height"></see> property of this <see cref="T:System.Drawing.RectangleF"></see> has a value of zero; otherwise, false.</returns>
        /// <filterpriority>1</filterpriority>
        [Browsable(false)]
        public bool IsEmpty => Width <= 0 || Height <= 0;

        /// <summary>Tests whether obj is a <see cref="T:System.Drawing.RectangleF"></see> with the same location and size of this <see cref="T:System.Drawing.RectangleF"></see>.</summary>
        /// <returns>This method returns true if obj is a <see cref="T:System.Drawing.RectangleF"></see> and its X, Y, Width, and Height properties are equal to the corresponding properties of this <see cref="T:System.Drawing.RectangleF"></see>; otherwise, false.</returns>
        /// <param name="obj">The <see cref="T:System.Object"></see> to test. </param>
        /// <filterpriority>1</filterpriority>
        public override bool Equals(object obj)
        {
            if (!(obj is RectangleF))
                return false;
            RectangleF ef = (RectangleF)obj;
            return ef.X == X && ef.Y == Y && ef.Width == Width && ef.Height == Height;
        }

        /// <summary>Tests whether two <see cref="T:System.Drawing.RectangleF"></see> structures have equal location and size.</summary>
        /// <returns>This operator returns true if the two specified <see cref="T:System.Drawing.RectangleF"></see> structures have equal <see cref="P:System.Drawing.RectangleF.X"></see>, <see cref="P:System.Drawing.RectangleF.Y"></see>, <see cref="P:System.Drawing.RectangleF.Width"></see>, and <see cref="P:System.Drawing.RectangleF.Height"></see> properties.</returns>
        /// <param name="right">The <see cref="T:System.Drawing.RectangleF"></see> structure that is to the right of the equality operator. </param>
        /// <param name="left">The <see cref="T:System.Drawing.RectangleF"></see> structure that is to the left of the equality operator. </param>
        /// <filterpriority>3</filterpriority>
        public static bool operator ==(RectangleF left, RectangleF right) => left.X == right.X && left.Y == right.Y && left.Width == right.Width && left.Height == right.Height;

        /// <summary>Tests whether two <see cref="T:System.Drawing.RectangleF"></see> structures differ in location or size.</summary>
        /// <returns>This operator returns true if any of the <see cref="P:System.Drawing.RectangleF.X"></see> , <see cref="P:System.Drawing.RectangleF.Y"></see>, <see cref="P:System.Drawing.RectangleF.Width"></see>, or <see cref="P:System.Drawing.RectangleF.Height"></see> properties of the two <see cref="T:System.Drawing.Rectangle"></see> structures are unequal; otherwise false.</returns>
        /// <param name="right">The <see cref="T:System.Drawing.RectangleF"></see> structure that is to the right of the inequality operator. </param>
        /// <param name="left">The <see cref="T:System.Drawing.RectangleF"></see> structure that is to the left of the inequality operator. </param>
        /// <filterpriority>3</filterpriority>
        public static bool operator !=(RectangleF left, RectangleF right) => left != right;

        public static RectangleF operator *(RectangleF left, float right) => new RectangleF(left.X * right, left.Y * right, left.Width * right, left.Height * right);

        public static RectangleF operator /(RectangleF left, float right) => new RectangleF(left.X / right, left.Y / right, left.Width / right, left.Height / right);

        /// <summary>Determines if the specified point is contained within this <see cref="T:System.Drawing.RectangleF"></see> structure.</summary>
        /// <returns>This method returns true if the point defined by x and y is contained within this <see cref="T:System.Drawing.RectangleF"></see> structure; otherwise false.</returns>
        /// <param name="y">The y-coordinate of the point to test. </param>
        /// <param name="x">The x-coordinate of the point to test. </param>
        /// <filterpriority>1</filterpriority>
        public bool Contains(float x, float y) => X <= x && x < X + Width && Y <= y && y < Y + Height;

        public bool Contains(Vector2 pt) => Contains(pt.X, pt.Y);

        /// <summary>Determines if the specified point is contained within this <see cref="T:System.Drawing.RectangleF"></see> structure.</summary>
        /// <returns>This method returns true if the point represented by the pt parameter is contained within this <see cref="T:System.Drawing.RectangleF"></see> structure; otherwise false.</returns>
        /// <param name="pt">The <see cref="T:System.Drawing.PointF"></see> to test. </param>
        /// <filterpriority>1</filterpriority>
        public bool Contains(PointF pt) => Contains(pt.X, pt.Y);

        /// <summary>Determines if the specified point is contained within this <see cref="T:System.Drawing.RectangleF"></see> structure.</summary>
        /// <returns>This method returns true if the point represented by the pt parameter is contained within this <see cref="T:System.Drawing.RectangleF"></see> structure; otherwise false.</returns>
        /// <param name="pt">The <see cref="T:System.Drawing.PointF"></see> to test. </param>
        /// <filterpriority>1</filterpriority>
        public bool Contains(Point pt) => Contains(pt.X, pt.Y);

        /// <summary>Determines if the rectangular region represented by rect is entirely contained within this <see cref="T:System.Drawing.RectangleF"></see> structure.</summary>
        /// <returns>This method returns true if the rectangular region represented by rect is entirely contained within the rectangular region represented by this <see cref="T:System.Drawing.RectangleF"></see>; otherwise false.</returns>
        /// <param name="rect">The <see cref="T:System.Drawing.RectangleF"></see> to test. </param>
        /// <filterpriority>1</filterpriority>
        public bool Contains(RectangleF rect) => X <= rect.X && rect.X + rect.Width <= X + Width && Y <= rect.Y &&
                    rect.Y + rect.Height <= Y + Height;

        public bool ContainsRotated(Vector2 pt, Vector2 rotationCenter, float angle)
        {
            if (angle == 0)
                return Contains(pt);

            Matrix2 rotationMatrix = Matrix2.CreateRotation(-angle);
            Vector2 rotatedPt = (pt - rotationCenter) * rotationMatrix + rotationCenter;
            return Contains(rotatedPt.X, rotatedPt.Y);
        }

        public void GetRotatedCorners(Vector2 rotationCenter, float angle, out Vector2 topLeft, out Vector2 topRight, out Vector2 bottomLeft, out Vector2 bottomRight)
        {
            Matrix2 rotationMatrix = Matrix2.CreateRotation(angle);
            topLeft = (new Vector2(Left, Top) - rotationCenter) * rotationMatrix + rotationCenter;
            topRight = (new Vector2(Right, Top) - rotationCenter) * rotationMatrix + rotationCenter;
            bottomLeft = (new Vector2(Left, Bottom) - rotationCenter) * rotationMatrix + rotationCenter;
            bottomRight = (new Vector2(Right, Bottom) - rotationCenter) * rotationMatrix + rotationCenter;
        }

        /// <summary>Gets the hash code for this <see cref="T:System.Drawing.RectangleF"></see> structure. For information about the use of hash codes, see Object.GetHashCode.</summary>
        /// <returns>The hash code for this <see cref="T:System.Drawing.RectangleF"></see>.</returns>
        /// <filterpriority>1</filterpriority>
        public override int GetHashCode()
        {
            return
                (int)
                ((uint)X ^ (uint)Y << 13 | (uint)Y >> 0x13 ^
                  (uint)Width << 0x1a | (uint)Width >> 6 ^
                 (uint)Height << 7 | (uint)Height >> 0x19);
        }

        public float Area => Width * Height;

        public RectangleF WithPositiveExtent
        {
            get
            {
                RectangleF result = this;

                if (result.Width < 0)
                {
                    result.Width = -result.Width;
                    result.X -= result.Width;
                }

                if (Height < 0)
                {
                    result.Height = -result.Height;
                    result.Y -= result.Height;
                }

                return result;
            }
        }

        public RectangleF Inflate(float amount) => Inflate(new Vector2(amount, amount));

        public RectangleF Inflate(Vector2 amount) => Inflate(new MarginPadding { Left = amount.X, Right = amount.X, Top = amount.Y, Bottom = amount.Y });

        public RectangleF Inflate(MarginPadding amount) => new RectangleF(
                X - amount.Left,
                Y - amount.Top,
                Width + amount.TotalHorizontal,
                Height + amount.TotalVertical);

        public RectangleF Shrink(float amount) => Shrink(new Vector2(amount, amount));

        public RectangleF Shrink(Vector2 amount) => Shrink(new MarginPadding { Left = amount.X, Right = amount.X, Top = amount.Y, Bottom = amount.Y });

        public RectangleF Shrink(MarginPadding amount) => Inflate(-amount);

        /// <summary>Replaces this <see cref="T:System.Drawing.RectangleF"></see> structure with the intersection of itself and the specified <see cref="T:System.Drawing.RectangleF"></see> structure.</summary>
        /// <returns>This method does not return a value.</returns>
        /// <param name="rect">The rectangle to intersect. </param>
        /// <filterpriority>1</filterpriority>
        public void Intersect(RectangleF rect)
        {
            RectangleF ef = Intersect(rect, this);
            X = ef.X;
            Y = ef.Y;
            Width = ef.Width;
            Height = ef.Height;
        }

        /// <summary>Returns a <see cref="T:System.Drawing.RectangleF"></see> structure that represents the intersection of two rectangles. If there is no intersection, and empty <see cref="T:System.Drawing.RectangleF"></see> is returned.</summary>
        /// <returns>A third <see cref="T:System.Drawing.RectangleF"></see> structure the size of which represents the overlapped area of the two specified rectangles.</returns>
        /// <param name="a">A rectangle to intersect. </param>
        /// <param name="b">A rectangle to intersect. </param>
        /// <filterpriority>1</filterpriority>
        public static RectangleF Intersect(RectangleF a, RectangleF b)
        {
            float x = Math.Max(a.X, b.X);
            float num2 = Math.Min(a.X + a.Width, b.X + b.Width);
            float y = Math.Max(a.Y, b.Y);
            float num4 = Math.Min(a.Y + a.Height, b.Y + b.Height);
            if (num2 >= x && num4 >= y)
                return new RectangleF(x, y, num2 - x, num4 - y);
            return Empty;
        }

        /// <summary>Determines if this rectangle intersects with rect.</summary>
        /// <returns>This method returns true if there is any intersection.</returns>
        /// <param name="rect">The rectangle to test. </param>
        /// <filterpriority>1</filterpriority>
        public bool IntersectsWith(RectangleF rect) =>
            rect.X < X + Width && X < rect.X + rect.Width && rect.Y < Y + Height && Y < rect.Y + rect.Height;

        /// <summary>Determines if this rectangle intersects with rect.</summary>
        /// <returns>This method returns true if there is any intersection.</returns>
        /// <param name="rect">The rectangle to test. </param>
        /// <filterpriority>1</filterpriority>
        public bool IntersectsWith(Rectangle rect) =>
            rect.X < X + Width && X < rect.X + rect.Width && rect.Y < Y + Height && Y < rect.Y + rect.Height;

        /// <summary>Creates the smallest possible third rectangle that can contain both of two rectangles that form a union.</summary>
        /// <returns>A third <see cref="T:System.Drawing.RectangleF"></see> structure that contains both of the two rectangles that form the union.</returns>
        /// <param name="a">A rectangle to union. </param>
        /// <param name="b">A rectangle to union. </param>
        /// <filterpriority>1</filterpriority>
        public static RectangleF Union(RectangleF a, RectangleF b)
        {
            float x = Math.Min(a.X, b.X);
            float num2 = Math.Max(a.X + a.Width, b.X + b.Width);
            float y = Math.Min(a.Y, b.Y);
            float num4 = Math.Max(a.Y + a.Height, b.Y + b.Height);
            return new RectangleF(x, y, num2 - x, num4 - y);
        }

        /// <summary>Adjusts the location of this rectangle by the specified amount.</summary>
        /// <returns>This method does not return a value.</returns>
        /// <param name="pos">The amount to offset the location. </param>
        /// <filterpriority>1</filterpriority>
        public RectangleF Offset(Vector2 pos) => Offset(pos.X, pos.Y);

        /// <summary>Adjusts the location of this rectangle by the specified amount.</summary>
        /// <returns>This method does not return a value.</returns>
        /// <param name="y">The amount to offset the location vertically. </param>
        /// <param name="x">The amount to offset the location horizontally. </param>
        /// <filterpriority>1</filterpriority>
        public RectangleF Offset(float x, float y) => new RectangleF(X + x, Y + y, Width, Height);

        internal float DistanceSquared(Vector2 localSpacePos)
        {
            Vector2 dist = new Vector2(
                Math.Max(0.0f, Math.Max(localSpacePos.X - Right, Left - localSpacePos.X)),
                Math.Max(0.0f, Math.Max(localSpacePos.Y - Bottom, Top - localSpacePos.Y))
            );

            return dist.LengthSquared;
        }

        // This could be optimized further in the future, but made for a simple implementation right now.
        public Rectangle AABB => ((Quad)this).AABB;

        /// <summary>Converts the specified <see cref="T:System.Drawing.Rectangle"></see> structure to a <see cref="T:System.Drawing.RectangleF"></see> structure.</summary>
        /// <returns>The <see cref="T:System.Drawing.RectangleF"></see> structure that is converted from the specified <see cref="T:System.Drawing.Rectangle"></see> structure.</returns>
        /// <param name="r">The <see cref="T:System.Drawing.Rectangle"></see> structure to convert. </param>
        /// <filterpriority>3</filterpriority>
        public static implicit operator RectangleF(Rectangle r) => new RectangleF(r.X, r.Y, r.Width, r.Height);

        /// <summary>Converts the Location and <see cref="T:System.Drawing.Size"></see> of this <see cref="T:System.Drawing.RectangleF"></see> to a human-readable string.</summary>
        /// <returns>A string that contains the position, width, and height of this <see cref="T:System.Drawing.RectangleF"></see> structure¾for example, "{X=20, Y=20, Width=100, Height=50}".</returns>
        /// <filterpriority>1</filterpriority>
        /// <PermissionSet><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode" /></PermissionSet>
        public override string ToString() => "{X=" + X.ToString(CultureInfo.CurrentCulture) + ",Y=" + Y.ToString(CultureInfo.CurrentCulture) +
                    ",Width=" + Width.ToString(CultureInfo.CurrentCulture) + ",Height=" +
                    Height.ToString(CultureInfo.CurrentCulture) + "}";

        static RectangleF()
        {
            Empty = new RectangleF();
        }

        public static bool RectCollide(double angle, RectangleF rP, RectangleF rQ, Vector2 origin)
        {
            // Calling arguments similar to a C implementation by Oren Becker
            // but different algorithm for detecting rectangle intersection.
            // angle gives rotation of the first rectangle around its left side's
            // midpoint.  The second rectangle is unrotated, aligned to axes.

            double xPmin, xPmax, yPmin, yPmax, xQmin, xQmax, yQmin, yQmax;

            if (angle == 0.0)
            {
                xPmax = (xPmin = rP.X) + rP.Width;
                yPmax = (yPmin = rP.Y) + rP.Height;
                xQmax = (xQmin = rQ.X) + rQ.Width;
                yQmax = (yQmin = rQ.Y) + rQ.Height;

                return xPmin <= xQmax && xQmin <= xPmax && yPmin <= yQmax && yQmin <= yPmax;
            }

            // Otherwise we need two trigonometric function calls
            double c = Math.Cos(angle), s = Math.Sin(angle);
            bool cPos = c > 0.0, sPos = s > 0.0;

            /* Phase 1: Form bounding box on tilted rectangle P.
             *          Check whether bounding box and Q intersect.
             *          If not, then P and Q do not intersect.
             *          Otherwise proceed to Phase 2.
             */

            double xPdif = 0.5 * rP.Width;
            double yPdif = 0.5 * rP.Height;

            /* P rotates around the midpoint of its left side. */
            double xPctr = rP.X + origin.X;
            double yPctr = rP.Y + origin.Y;
            double cxPdf = c * xPdif;
            double sxPdf = s * xPdif;
            double cyPdf = c * yPdif;
            double syPdf = s * yPdif;

            /* Translate coordinates of Q so P is re-centered at origin. */
            xQmax = (xQmin = rQ.X - xPctr) + rQ.Width;
            yQmax = (yQmin = rQ.Y - yPctr) + rQ.Height;

            /* Compute the bounding box coordinates for P. */
            if (cPos)
                if (sPos)
                {
                    xPmin = -(xPmax = cxPdf + syPdf);
                    yPmin = -(yPmax = cyPdf + sxPdf);
                }
                else /* s <= 0.0 */
                {
                    xPmin = -(xPmax = cxPdf - syPdf);
                    yPmin = -(yPmax = cyPdf - sxPdf);
                }
            else /* c <= 0.0 */ if (sPos)
            {
                xPmax = -(xPmin = cxPdf - syPdf);
                yPmax = -(yPmin = cyPdf - sxPdf);
            }
            else /* s <= 0.0 */
            {
                xPmax = -(xPmin = cxPdf + syPdf);
                yPmax = -(yPmin = cyPdf + sxPdf);
            }

            /*
                        if (InputManager.PressedKeys != null && OsuGame.Input.DontUseMeState.Keyboard.Keys.Contains(Keys.N))
                        {
                            OsuGame.LineManager.DrawPoint(xPmin + xPctr, yPmin + yPctr, Microsoft.Xna.Framework.Graphics.Color4.GreenYellow);
                            OsuGame.LineManager.DrawPoint(xPmax + xPctr, yPmax + yPctr, Microsoft.Xna.Framework.Graphics.Color4.GreenYellow);
            
                            OsuGame.LineManager.DrawPoint(xQmin + xPctr, yQmin + yPctr, Microsoft.Xna.Framework.Graphics.Color4.GreenYellow);
                            OsuGame.LineManager.DrawPoint(xQmax + xPctr, yQmax + yPctr, Microsoft.Xna.Framework.Graphics.Color4.GreenYellow);
                        }
            */

            /* Now perform the standard rectangle intersection test. */
            if (xPmin > xQmax || xQmin > xPmax || yPmin > yQmax || yQmin > yPmax)
                return false;

            /* Phase 2: If we get here, check the edges of P to see
             *          if one of them excludes all vertices of Q.
             *          If so, then P and Q do not intersect.
             *          (If not, then P and Q do intersect.)
             */
            if (cPos)
            {
                if (sPos)
                {
                    return c * xQmax + s * yQmax >= -xPdif
                        && c * xQmin + s * yQmin <= xPdif
                        && c * yQmax - s * xQmin >= -yPdif
                        && c * yQmin - s * xQmax <= yPdif;
                }
                return c * xQmax + s * yQmin >= -xPdif
                    && c * xQmin + s * yQmax <= xPdif
                    && c * yQmax - s * xQmax >= -yPdif
                    && c * yQmin - s * xQmin <= yPdif;
            }
            if (sPos)
            {
                return c * xQmin + s * yQmax >= -xPdif
                    && c * xQmax + s * yQmin <= xPdif
                    && c * yQmin - s * xQmin >= -yPdif
                    && c * yQmax - s * xQmax <= yPdif;
            }
            return c * xQmin + s * yQmin >= -xPdif
                && c * xQmax + s * yQmax <= xPdif
                && c * yQmin - s * xQmax >= -yPdif
                && c * yQmax - s * xQmin <= yPdif;
        }
    }
}

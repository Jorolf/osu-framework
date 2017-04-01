// Copyright (c) 2007-2017 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu-framework/master/LICENCE

using System;
using osu.Framework.Graphics.OpenGL;
using osu.Framework.Graphics.Textures;
using OpenTK;
using osu.Framework.Graphics.Primitives;
using System.Collections.Generic;
using System.Linq;

namespace osu.Framework.Graphics.Sprites
{
    public class Polygon : Sprite 
    {
        private const int polygonCorners = 7;

        public Polygon()
        {
            Texture = Texture.WhitePixel;
        }

        public override RectangleF BoundingBox => ToParentSpace(LayoutRectangle).AABBFloat;

        private static List<Triangle> toPolygon(Quad q)
        {
            Vector2 start = (q.TopLeft + q.TopRight) / 2;
            double angle = MathHelper.TwoPi / polygonCorners;
            return Enumerable.Range(1, polygonCorners - 1).Select(i => new Triangle(
                    start,
                    new Vector2(
                        (float)Math.Cos((angle * i) - MathHelper.PiOver2) * (q.Width / 2), 
                        (float)Math.Sin((angle * i) - MathHelper.PiOver2) * (q.Height / 2)
                    ) + q.Centre,
                    new Vector2(
                        (float)Math.Cos((angle * (i + 1)) - MathHelper.PiOver2) * (q.Width / 2),
                        (float)Math.Sin((angle * (i + 1)) - MathHelper.PiOver2) * (q.Height / 2)
                    ) + q.Centre
                )
            ).ToList();
        }

        protected override bool InternalContains(Vector2 screenSpacePos) =>toPolygon(ScreenSpaceDrawQuad).Any(triangle => triangle.Contains(screenSpacePos));

        protected override DrawNode CreateDrawNode() => new PolygonDrawNode();

        private class PolygonDrawNode : SpriteDrawNode
        {
            protected override void Blit(Action<TexturedVertex2D> vertexAction)
            {
                toPolygon(ScreenSpaceDrawQuad).ForEach(triangle => Texture.DrawTriangle(triangle, DrawInfo.Colour, null, null,
                    new Vector2(InflationAmount.X / DrawRectangle.Width, InflationAmount.Y / DrawRectangle.Height)));
            }
        }
    }
}

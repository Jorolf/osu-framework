// Copyright (c) 2007-2017 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu-framework/master/LICENCE

using OpenTK;

namespace osu.Framework.Graphics.Transformations
{
    public class TransformPositionX : TransformFloat
    {
        public override void Apply(Drawable d)
        {
            base.Apply(d);
            d.Position = new Vector2(CurrentValue, d.Position.Y);
        }
    }
}

// Copyright (c) 2007-2017 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu-framework/master/LICENCE

using OpenTK;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.IO.Stores;
using osu.Framework.Testing;
using System.Collections.Generic;
using System.Linq;

namespace osu.Framework.VisualTests.Tests
{
    internal class TestCaseLocalisation : TestCase
    {

        private LocaleStore locale;
        private Dropdown<string> dropdown;

        public override void Reset()
        {
            base.Reset();

            Children = new Drawable[]
            {
                dropdown = new TestCaseDropdownBox.StyledDropdownMenu
                {
                    Width = 250,
                    Items = locale.Locales.Select(localeName => new KeyValuePair<string, string>(locale.GetDisplayName(localeName), localeName)),
                },
                new SpriteText
                {
                    Position = new Vector2(300,0),
                    Localisation = "localised",
                },
            };
            locale.Locale = dropdown.SelectedValue;
        }

        [BackgroundDependencyLoader]
        private void load(LocaleStore store)
        {
            locale = store;
        }
    }
}

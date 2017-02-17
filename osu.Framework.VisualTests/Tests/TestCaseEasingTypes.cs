using OpenTK;
using OpenTK.Graphics;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Transformations;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Screens.Testing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace osu.Framework.VisualTests.Tests
{
    class TestCaseEasingTypes : TestCase
    {
        public override string Name => "EasingTypes";
        public override string Description => "Visualisation of Easings";

        private Container box;
        private Dictionary<string,bool> activated = new Dictionary<string, bool>();
        private DropDownMenu<EasingTypes> dropMenu;

        public override void Reset()
        {
            base.Reset();
            List<KeyValuePair<string, EasingTypes>> list = new List<KeyValuePair<string, EasingTypes>>();
            foreach (EasingTypes type in Enum.GetValues(typeof(EasingTypes)))
                list.Add(new KeyValuePair<string, EasingTypes>(type.ToString(), type));
            Children = new Drawable[]
            {
                box = new Container()
                {
                    Colour = Color4.White,
                    Origin = Anchor.Centre,
                    Anchor = Anchor.Centre,
                    Size = new Vector2(100,100),
                    RelativePositionAxes = Axes.X,
                    Position = new Vector2(-0.3f,0),
                    Children = new Drawable[]
                    {
                        new Box()
                        {
                            Colour = Color4.White,
                            Origin = Anchor.Centre,
                            Anchor = Anchor.Centre,
                            RelativeSizeAxes = Axes.Both,
                        },
                        new Box()
                        {
                            Colour = Color4.Red,
                            Origin = Anchor.Centre,
                            Anchor = Anchor.Centre,
                            Size = new Vector2(50,50),
                        },
                    }
                },
            };

            //I hate C# Dictionaries for this
            activated["Move"] = false;
            activated["Scale"] = false;
            activated["Rotate"] = false;
            activated["Resize"] = false;

            //Is there a better way to do this?
            AddToggle("Move", delegate
            {
                if (activated["Move"])
                    box.MoveTo(new Vector2(-0.3f, 0), 1000, dropMenu.SelectedValue);
                else
                    box.MoveTo(new Vector2(0.3f, 0), 1000, dropMenu.SelectedValue);
                activated["Move"] = !activated["Move"];
            });
            AddToggle("Rotate", delegate
            {
                if (activated["Rotate"])
                    box.RotateTo(0, 1000, dropMenu.SelectedValue);
                else
                    box.RotateTo(180, 1000, dropMenu.SelectedValue);
                activated["Rotate"] = !activated["Rotate"];
            });
            AddToggle("Scale", delegate
            {
                if (activated["Scale"])
                   box.ScaleTo(1, 1000, dropMenu.SelectedValue);
                else
                    box.ScaleTo(0.5f, 1000, dropMenu.SelectedValue);
                activated["Scale"] = !activated["Scale"];
            });
            AddToggle("Resize", delegate
            {
                if (activated["Resize"])
                    box.ResizeTo(new Vector2(100,100), 1000, dropMenu.SelectedValue);
                else
                    box.ResizeTo(new Vector2(50,50), 1000, dropMenu.SelectedValue);
                activated["Resize"] = !activated["Resize"];
            });

            ButtonsContainer.Add(dropMenu = new StyledDropDownMenu()
            {
                Width = 150,
                Items = list.ToArray(),
                SelectedIndex = 0,
                MaxDropDownHeight = 500,
            });
            
        }


        //I'm not happy with this, copied from TestCaseDropDownBox and adjusted for this purpose
        private class StyledDropDownMenu : DropDownMenu<EasingTypes>
        {
            protected override DropDownHeader CreateHeader()
            {
                return new StyledDropDownHeader();
            }

            protected override IEnumerable<DropDownMenuItem<EasingTypes>> GetDropDownItems(IEnumerable<KeyValuePair<string, EasingTypes>> values)
            {
                return values.Select(v => new StyledDropDownMenuItem(v.Key,v.Value));
            }

            public StyledDropDownMenu()
            {
                Header.CornerRadius = 4;
                ContentContainer.CornerRadius = 4;
            }

            protected override void AnimateOpen()
            {
                ContentContainer.Show();
            }

            protected override void AnimateClose()
            {
                ContentContainer.Hide();
            }

        }

        private class StyledDropDownHeader : DropDownHeader
        {
            private SpriteText label;
            protected override string Label
            {
                get { return label.Text; }
                set { label.Text = value; }
            }

            public StyledDropDownHeader()
            {
                Foreground.Padding = new MarginPadding(4);
                BackgroundColour = new Color4(255, 255, 255, 100);
                BackgroundColourHover = Color4.HotPink;
                Children = new[]
                {
                    label = new SpriteText(),
                };
            }
        }

        private class StyledDropDownMenuItem : DropDownMenuItem<EasingTypes>
        {
            public StyledDropDownMenuItem(string text, EasingTypes type) : base(text, type)
            {
                AutoSizeAxes = Axes.Y;
                Foreground.Padding = new MarginPadding(2);

                Children = new[]
                {
                    new SpriteText { Text = text },
                };
            }
        }
    }
}

﻿// Copyright (c) 2007-2017 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu-framework/master/LICENCE

using osu.Framework.Input.Handlers;
using OpenTK;
using osu.Framework.Allocation;

namespace osu.Framework.Input
{
    public class UserInputManager : InputManager
    {
        public override bool Contains(Vector2 screenSpacePos) => true;

        [BackgroundDependencyLoader]
        private void load()
        {
            if (Host != null)
            {
                foreach (InputHandler h in Host.GetInputHandlers())
                    AddHandler(h);
            }
        }
    }
}

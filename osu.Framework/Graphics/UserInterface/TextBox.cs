﻿// Copyright (c) 2007-2017 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu-framework/master/LICENCE

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using osu.Framework.Caching;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Transformations;
using osu.Framework.Input;
using osu.Framework.MathUtils;
using osu.Framework.Threading;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Input;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Platform;

namespace osu.Framework.Graphics.UserInterface
{
    public class TextBox : Container
    {
        protected FlowContainer TextFlow;
        protected Box Background;
        protected Box Caret;
        protected Container TextContainer;

        /// <summary>
        /// Padding to be used within the TextContainer. Requires special handling due to the sideways scrolling of text content.
        /// </summary>
        protected virtual float LeftRightPadding => 5;

        public int? LengthLimit;

        public bool AllowClipboardExport => true;

        //represents the left/right selection coordinates of the word double clicked on when dragging
        private int[] doubleClickWord;

        private AudioManager audio;

        /// <summary>
        /// Should this TextBox accept arrow keys for navigation?
        /// </summary>
        public bool HandleLeftRightArrows = true;

        protected virtual Color4 BackgroundCommit => new Color4(249, 90, 255, 200);
        protected virtual Color4 BackgroundFocused => new Color4(100, 100, 100, 255);
        protected virtual Color4 BackgroundUnfocused => new Color4(100, 100, 100, 120);

        public bool ReadOnly;

        private ITextInputSource textInput;
        private Clipboard clipboard;

        public delegate void OnCommitHandler(TextBox sender, bool newText);

        public event OnCommitHandler OnCommit;
        public event OnCommitHandler OnChange;

        private Scheduler textUpdateScheduler = new Scheduler();

        public TextBox()
        {
            Masking = true;
            CornerRadius = 3;

            Add(new Drawable[]
            {
                Background = new Box
                {
                    Colour = BackgroundUnfocused,
                    RelativeSizeAxes = Axes.Both,
                },
                TextContainer = new Container
                {
                    AutoSizeAxes = Axes.X,
                    RelativeSizeAxes = Axes.Y,
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                    Position = new Vector2(LeftRightPadding, 0),
                    Children = new Drawable[]
                    {
                        Placeholder = CreatePlaceholder(),
                        Caret = new Box
                        {
                            Size = new Vector2(1, 0.9f),
                            Colour = Color4.Transparent,
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                            RelativeSizeAxes = Axes.Y,
                            Alpha = 0,
                        },
                        TextFlow = new FlowContainer
                        {
                            Direction = FlowDirections.Horizontal,
                            AutoSizeAxes = Axes.X,
                            RelativeSizeAxes = Axes.Y,
                        },
                    },
                },
            });
        }

        [BackgroundDependencyLoader]
        private void load(BasicGameHost host, AudioManager audio)
        {
            this.audio = audio;

            textInput = host.GetTextInput();
            clipboard = host.GetClipboard();

            if (textInput != null)
            {
                textInput.OnNewImeComposition += delegate (string s)
                {
                    textUpdateScheduler.Add(() => onImeComposition(s));
                    cursorAndLayout.Invalidate();
                };
                textInput.OnNewImeResult += delegate (string s)
                {
                    textUpdateScheduler.Add(() => onImeResult(s));
                    cursorAndLayout.Invalidate();
                };
            }
        }

        private void resetSelection()
        {
            selectionStart = selectionEnd;
            cursorAndLayout.Invalidate();
        }

        protected override void Dispose(bool isDisposing)
        {
            OnChange = null;
            OnCommit = null;

            unbindInput();

            base.Dispose(isDisposing);
        }

        private float textContainerPosX;

        private string textAtLastLayout = string.Empty;

        protected override void UpdateAfterChildren()
        {
            base.UpdateAfterChildren();

            //have to run this after children flow
            cursorAndLayout.Refresh(delegate
            {
                Placeholder.TextSize = CalculatedTextSize;

                textUpdateScheduler.Update();

                Vector2 cursorPos = Vector2.Zero;
                if (text?.Length > 0)
                    cursorPos.X = getPositionAt(selectionLeft);

                float cursorPosEnd = getPositionAt(selectionEnd);

                float cursorWidth = 2;

                if (selectionLength > 0)
                    cursorWidth = getPositionAt(selectionRight) - cursorPos.X;

                float cursorRelativePositionAxesInBox = (cursorPosEnd - textContainerPosX) / DrawWidth;

                //we only want to reposition the view when the cursor reaches near the extremities.
                if (cursorRelativePositionAxesInBox < 0.1 || cursorRelativePositionAxesInBox > 0.9)
                {
                    textContainerPosX = cursorPosEnd - DrawWidth / 2 + LeftRightPadding * 2;
                }

                textContainerPosX = MathHelper.Clamp(textContainerPosX, 0, Math.Max(0, TextFlow.DrawWidth - DrawWidth + LeftRightPadding * 2));

                TextContainer.MoveToX(LeftRightPadding - textContainerPosX, 300, EasingTypes.OutExpo);

                if (HasFocus)
                {
                    Caret.ClearTransformations();
                    Caret.MoveTo(cursorPos, 60, EasingTypes.Out);
                    Caret.ScaleTo(new Vector2(cursorWidth, 1), 60, EasingTypes.Out);

                    if (selectionLength > 0)
                    {
                        Caret.FadeTo(0.5f, 200, EasingTypes.Out);
                        Caret.FadeColour(new Color4(249, 90, 255, 255), 200, EasingTypes.Out);
                    }
                    else
                    {
                        Caret.FadeTo(0.7f, 200, EasingTypes.Out);
                        Caret.FadeColour(Color4.White, 200, EasingTypes.Out);
                        Caret.Transforms.Add(new TransformAlpha
                        {
                            StartValue = 0.7f,
                            EndValue = 0.4f,
                            StartTime = Time.Current,
                            EndTime = Time.Current + 500,
                            Easing = EasingTypes.InOutSine,
                            LoopCount = -1,
                        });
                    }
                }

                OnChange?.Invoke(this, textAtLastLayout != text);
                if (textAtLastLayout.Length == 0 || text.Length == 0)
                    Placeholder.FadeTo(text.Length == 0 ? 1 : 0, 200);

                textAtLastLayout = text;

                return cursorPos;
            });
        }

        private float getPositionAt(int index)
        {
            if (index > 0)
            {
                if (index < text.Length)
                    return TextFlow.Children.ElementAt(index).DrawPosition.X + TextFlow.DrawPosition.X;
                var d = TextFlow.Children.ElementAt(index - 1);
                return d.DrawPosition.X + d.DrawSize.X + TextFlow.Spacing.X + TextFlow.DrawPosition.X;
            }
            return 0;
        }

        private int getCharacterClosestTo(Vector2 pos)
        {
            pos = TextFlow.ToLocalSpace(pos * DrawInfo.Matrix);

            int i = 0;
            foreach (Drawable d in TextFlow.Children)
            {
                if (d.DrawPosition.X + d.DrawSize.X / 2 > pos.X)
                    break;
                i++;
            }

            return i;
        }

        int selectionStart;
        int selectionEnd;

        int selectionLength => Math.Abs(selectionEnd - selectionStart);

        int selectionLeft => Math.Min(selectionStart, selectionEnd);
        int selectionRight => Math.Max(selectionStart, selectionEnd);

        Cached<Vector2> cursorAndLayout = new Cached<Vector2>();

        private void moveSelection(int offset, bool expand)
        {
            if (textInput?.ImeActive == true) return;

            int oldStart = selectionStart;
            int oldEnd = selectionEnd;

            if (expand)
                selectionEnd = MathHelper.Clamp(selectionEnd + offset, 0, text.Length);
            else
            {
                if (selectionLength > 0 && Math.Abs(offset) <= 1)
                {
                    //we don't want to move the location when "removing" an existing selection, just set the new location.
                    if (offset > 0)
                        selectionEnd = selectionStart = selectionRight;
                    else
                        selectionEnd = selectionStart = selectionLeft;
                }
                else
                    selectionEnd = selectionStart = MathHelper.Clamp((offset > 0 ? selectionRight : selectionLeft) + offset, 0, text.Length);
            }

            if (oldStart != selectionStart || oldEnd != selectionEnd)
            {
                audio.Sample.Get(@"Keyboard/key-movement")?.Play();
                cursorAndLayout.Invalidate();
            }
        }

        private bool removeCharacterOrSelection(bool sound = true)
        {
            if (text.Length == 0) return false;
            if (selectionLength == 0 && selectionLeft == 0) return false;

            int count = MathHelper.Clamp(selectionLength, 1, text.Length);
            int start = MathHelper.Clamp(selectionLength > 0 ? selectionLeft : selectionLeft - 1, 0, text.Length - count);

            if (count == 0) return false;

            if (sound)
                audio.Sample.Get(@"Keyboard/key-delete")?.Play();

            foreach (var d in TextFlow.Children.Skip(start).Take(count).ToArray()) //ToArray since we are removing items from the children in this block.
            {
                TextFlow.Remove(d);

                TextContainer.Add(d);
                d.FadeOut(200);
                d.MoveToY(d.DrawSize.Y, 200, EasingTypes.InExpo);
                d.Expire();
            }

            text = text.Remove(start, count);

            if (selectionLength > 0)
                selectionStart = selectionEnd = selectionLeft;
            else
                selectionStart = selectionEnd = selectionLeft - 1;

            cursorAndLayout.Invalidate();
            return true;
        }

        protected virtual Drawable GetDrawableCharacter(char c) => new SpriteText { Text = c.ToString(), TextSize = CalculatedTextSize };

        protected virtual Drawable AddCharacterToFlow(char c)
        {
            // Remove all characters to the right and store them in a local list,
            // such that their depth can be updated.
            List<Drawable> charsRight = new List<Drawable>();
            foreach (Drawable d in TextFlow.Children.Skip(selectionLeft))
                charsRight.Add(d);
            TextFlow.Remove(charsRight);

            // Update their depth to make room for the to-be inserted character.
            int i = -selectionLeft;
            foreach (Drawable d in charsRight)
                d.Depth = --i;

            // Add the character
            Drawable ch = GetDrawableCharacter(c);
            ch.Depth = -selectionLeft;

            TextFlow.Add(ch);

            // Add back all the previously removed characters
            TextFlow.Add(charsRight);

            return ch;
        }

        protected float CalculatedTextSize => TextFlow.DrawSize.Y - (TextFlow.Padding.Top + TextFlow.Padding.Bottom);

        /// <summary>
        /// Insert an arbitrary string into the text at the current position.
        /// </summary>
        /// <param name="addText"></param>
        private void insertString(string addText)
        {
            if (string.IsNullOrEmpty(addText)) return;

            foreach (char c in addText)
                addCharacter(c);
        }

        private Drawable addCharacter(char c)
        {
            if (char.IsControl(c)) return null;

            if (selectionLength > 0)
                removeCharacterOrSelection();

            if (text.Length + 1 > LengthLimit)
            {
                if (Background.Alpha > 0)
                    Background.FlashColour(Color4.Red, 200);
                else
                    TextFlow.FlashColour(Color4.Red, 200);
                return null;
            }

            Drawable ch = AddCharacterToFlow(c);

            text = text.Insert(selectionLeft, c.ToString());
            selectionStart = selectionEnd = selectionLeft + 1;

            cursorAndLayout.Invalidate();

            return ch;
        }

        protected virtual SpriteText CreatePlaceholder() => new SpriteText
        {
            Colour = Color4.Gray,
        };

        protected SpriteText Placeholder;

        public string PlaceholderText
        {
            get { return Placeholder.Text; }
            set
            {
                Placeholder.Text = value;
            }
        }

        private string text = string.Empty;

        public virtual string Text
        {
            get { return text; }
            set
            {
                Debug.Assert(value != null);

                if (value == text)
                    return;

                Placeholder.FadeTo(value.Length == 0 ? 1 : 0);

                textUpdateScheduler.Add(delegate
                {
                    int startBefore = selectionStart;
                    selectionStart = selectionEnd = 0;
                    TextFlow?.Clear();
                    text = string.Empty;

                    foreach (char c in value)
                        addCharacter(c);

                    selectionStart = MathHelper.Clamp(startBefore, 0, text.Length);
                });

                cursorAndLayout.Invalidate();
            }
        }

        public string SelectedText => selectionLength > 0 ? Text.Substring(selectionLeft, selectionLength) : string.Empty;

        protected bool HandlePendingText(InputState state)
        {
            string str = textInput?.GetPendingText();
            if (string.IsNullOrEmpty(str))
                return false;

            if (state.Keyboard.ShiftPressed)
                audio.Sample.Get(@"Keyboard/key-caps")?.Play();
            else
                audio.Sample.Get($@"Keyboard/key-press-{RNG.Next(1, 5)}")?.Play();
            insertString(str);
            return true;
        }

        protected override bool OnKeyDown(InputState state, KeyDownEventArgs args)
        {
            if (!HasFocus)
                return false;

            if (textInput?.ImeActive == true) return true;

            if (args.Key >= Key.F1 && args.Key <= Key.F35)
                return false;

            if (HandlePendingText(state)) return true;

            switch (args.Key)
            {
                case Key.Tab:
                    return false;
                case Key.End:
                    moveSelection(text.Length, state.Keyboard.ShiftPressed);
                    return true;
                case Key.Home:
                    moveSelection(-text.Length, state.Keyboard.ShiftPressed);
                    return true;
                case Key.Left:
                    {
                        if (!HandleLeftRightArrows) return false;

                        if (selectionEnd == 0)
                        {
                            //we only clear if you aren't holding shift
                            if (!state.Keyboard.ShiftPressed)
                                resetSelection();
                            return true;
                        }

                        int amount = 1;
                        if (state.Keyboard.ControlPressed)
                        {
                            int lastSpace = text.LastIndexOf(' ', Math.Max(0, selectionEnd - 2));
                            if (lastSpace >= 0)
                            {
                                //if you have something selected and shift is not held down
                                //A selection reset is required to select a word inside the current selection
                                if (!state.Keyboard.ShiftPressed)
                                    resetSelection();
                                amount = selectionEnd - lastSpace - 1;
                            }
                            else
                                amount = selectionEnd;
                        }

                        moveSelection(-amount, state.Keyboard.ShiftPressed);
                        return true;
                    }
                case Key.Right:
                    {
                        if (!HandleLeftRightArrows) return false;

                        if (selectionEnd == text.Length)
                        {
                            if (!state.Keyboard.ShiftPressed)
                                resetSelection();
                            return true;
                        }

                        int amount = 1;
                        if (state.Keyboard.ControlPressed)
                        {
                            int nextSpace = text.IndexOf(' ', selectionEnd + 1);
                            if (nextSpace >= 0)
                            {
                                if (!state.Keyboard.ShiftPressed)
                                    resetSelection();
                                amount = nextSpace - selectionEnd;
                            }
                            else
                                amount = text.Length - selectionEnd;
                        }

                        moveSelection(amount, state.Keyboard.ShiftPressed);
                        return true;
                    }
                case Key.Enter:
                    selectionStart = selectionEnd = 0;
                    TriggerFocusLost(state);
                    return true;
                case Key.Delete:
                    if (selectionLength == 0)
                    {
                        if (text.Length == selectionStart)
                            return true;

                        if (state.Keyboard.ControlPressed)
                        {
                            int spacePos = selectionStart;
                            while (text[spacePos] == ' ' && spacePos < text.Length)
                                spacePos++;

                            spacePos = MathHelper.Clamp(text.IndexOf(' ', spacePos), 0, text.Length);
                            selectionEnd = spacePos;

                            if (selectionStart == 0 && spacePos == 0)
                                selectionEnd = text.Length;

                            if (selectionLength == 0)
                                return true;
                        }
                        else
                        {
                            //we're deleting in front of the cursor, so move the cursor forward once first
                            selectionStart = selectionEnd = selectionStart + 1;
                        }
                    }

                    removeCharacterOrSelection();
                    return true;
                case Key.Back:
                    if (selectionLength == 0 && state.Keyboard.ControlPressed)
                    {
                        int spacePos = selectionLeft >= 2 ? Math.Max(0, text.LastIndexOf(' ', selectionLeft - 2) + 1) : 0;
                        selectionStart = spacePos;
                    }

                    removeCharacterOrSelection();
                    return true;
            }

            if (state.Keyboard.ControlPressed)
            {
                //handling of function keys
                switch (args.Key)
                {
                    case Key.A:
                        selectionStart = 0;
                        selectionEnd = text.Length;
                        cursorAndLayout.Invalidate();
                        return true;
                    case Key.C:
                        if (string.IsNullOrEmpty(SelectedText) || !AllowClipboardExport) return true;

                        clipboard?.SetText(SelectedText);
                        return true;
                    case Key.X:
                        if (string.IsNullOrEmpty(SelectedText) || !AllowClipboardExport) return true;

                        clipboard?.SetText(SelectedText);
                        removeCharacterOrSelection();
                        return true;
                    case Key.V:

                        //the text may get pasted into the hidden textbox, so we don't need any direct clipboard interaction here.
                        string pending = textInput?.GetPendingText();

                        if (string.IsNullOrEmpty(pending))
                            pending = clipboard?.GetText();

                        insertString(pending);
                        return true;
                }

                return false;
            }

            return true;
        }

        protected override bool OnDrag(InputState state)
        {
            //if (textInput?.ImeActive == true) return true;

            if (doubleClickWord != null)
            {
                //select words at a time
                if (getCharacterClosestTo(state.Mouse.Position) > doubleClickWord[1])
                {
                    selectionStart = doubleClickWord[0];
                    selectionEnd = findSeparatorIndex(text, getCharacterClosestTo(state.Mouse.Position) - 1, 1);
                    selectionEnd = selectionEnd >= 0 ? selectionEnd : text.Length;
                }
                else if (getCharacterClosestTo(state.Mouse.Position) < doubleClickWord[0])
                {
                    selectionStart = doubleClickWord[1];
                    selectionEnd = findSeparatorIndex(text, getCharacterClosestTo(state.Mouse.Position), -1);
                    selectionEnd = selectionEnd >= 0 ? selectionEnd + 1 : 0;
                }
                else
                {
                    //in the middle
                    selectionStart = doubleClickWord[0];
                    selectionEnd = doubleClickWord[1];
                }
                cursorAndLayout.Invalidate();
            }
            else
            {
                if (text.Length == 0) return true;

                selectionEnd = getCharacterClosestTo(state.Mouse.Position);
                if (selectionLength > 0)
                    TriggerFocus();

                cursorAndLayout.Invalidate();
            }
            return true;
        }

        protected override bool OnDragStart(InputState state)
        {
            if (HasFocus) return true;

            Vector2 posDiff = state.Mouse.PositionMouseDown.Value - state.Mouse.Position;

            return Math.Abs(posDiff.X) > Math.Abs(posDiff.Y);
        }

        protected override bool OnDoubleClick(InputState state)
        {
            if (textInput?.ImeActive == true) return true;

            if (text.Length == 0) return true;

            int hover = Math.Min(text.Length - 1, getCharacterClosestTo(state.Mouse.Position));

            int lastSeparator = findSeparatorIndex(text, hover, -1);
            int nextSeparator = findSeparatorIndex(text, hover, 1);

            selectionStart = lastSeparator >= 0 ? lastSeparator + 1 : 0;
            selectionEnd = nextSeparator >= 0 ? nextSeparator : text.Length;

            //in order to keep the home word selected
            doubleClickWord = new[] { selectionStart, selectionEnd };

            cursorAndLayout.Invalidate();
            return true;
        }

        private static int findSeparatorIndex(string input, int searchPos, int direction)
        {
            bool isLetterOrDigit = char.IsLetterOrDigit(input[searchPos]);

            for (int i = searchPos; i >= 0 && i < input.Length; i += direction)
            {
                if (char.IsLetterOrDigit(input[i]) != isLetterOrDigit)
                    return i;
            }

            return -1;
        }

        protected override bool OnMouseDown(InputState state, MouseDownEventArgs args)
        {
            if (textInput?.ImeActive == true) return true;

            selectionStart = selectionEnd = getCharacterClosestTo(state.Mouse.Position);

            cursorAndLayout.Invalidate();

            return false;
        }

        protected override bool OnMouseUp(InputState state, MouseUpEventArgs args)
        {
            doubleClickWord = null;
            return true;
        }

        protected override void OnFocusLost(InputState state)
        {
            unbindInput();

            Caret.ClearTransformations();
            Caret.FadeOut(200);

            if (state.Keyboard.Keys.Contains(Key.Enter))
            {
                Background.Colour = BackgroundUnfocused;
                Background.ClearTransformations();
                Background.FlashColour(BackgroundCommit, 400);

                audio.Sample.Get(@"Keyboard/key-confirm")?.Play();
                OnCommit?.Invoke(this, true);
            }
            else
            {
                Background.ClearTransformations();
                Background.FadeColour(BackgroundUnfocused, 200, EasingTypes.OutExpo);
            }

            cursorAndLayout.Invalidate();
        }

        protected override bool OnFocus(InputState state)
        {
            if (ReadOnly) return false;

            bindInput();

            Background.ClearTransformations();
            Background.FadeColour(BackgroundFocused, 200, EasingTypes.Out);

            cursorAndLayout.Invalidate();
            return true;
        }

        #region Native TextBox handling (winform specific)

        private void unbindInput()
        {
            textInput?.Deactivate(this);
        }

        private void bindInput()
        {
            textInput.Activate(this);
        }

        private void onImeResult(string s)
        {
            //we only succeeded if there is pending data in the textbox
            if (imeDrawables.Count > 0)
            {
                audio.Sample.Get(@"Keyboard/key-confirm")?.Play();

                foreach (Drawable d in imeDrawables)
                {
                    d.Colour = Color4.White;
                    d.FadeTo(1, 200, EasingTypes.Out);
                }
            }

            imeDrawables.Clear();
        }

        private readonly List<Drawable> imeDrawables = new List<Drawable>();

        private void onImeComposition(string s)
        {
            //search for unchanged characters..
            int matchCount = 0;
            bool matching = true;
            bool didDelete = false;

            int searchStart = text.Length - imeDrawables.Count;

            //we want to keep processing to the end of the longest string (the current displayed or the new composition).
            int maxLength = Math.Max(imeDrawables.Count, s.Length);

            for (int i = 0; i < maxLength; i++)
            {
                if (matching && searchStart + i < text.Length && i < s.Length && text[searchStart + i] == s[i])
                {
                    matchCount = i + 1;
                    continue;
                }

                matching = false;

                if (matchCount < imeDrawables.Count)
                {
                    //if we are no longer matching, we want to remove all further characters.
                    removeCharacterOrSelection(false);
                    imeDrawables.RemoveAt(matchCount);
                    didDelete = true;
                }
            }

            if (matchCount == s.Length)
            {
                //in the case of backspacing (or a NOP), we can exit early here.
                if (didDelete)
                    audio.Sample.Get(@"Keyboard/key-delete")?.Play();
                return;
            }

            //add any new or changed characters
            for (int i = matchCount; i < s.Length; i++)
            {
                Drawable dr = addCharacter(s[i]);
                if (dr != null)
                {
                    dr.Colour = Color4.Aqua;
                    dr.Alpha = 0.6f;
                    imeDrawables.Add(dr);
                }
            }

            audio.Sample.Get($@"Keyboard/key-press-{RNG.Next(1, 5)}")?.Play();
        }

        #endregion
    }
}
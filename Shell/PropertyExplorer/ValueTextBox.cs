using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PopUpUI.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace NoteKeeper.Shell.PropertyExplorer
{
    internal class ValueTextBox
    {
        private readonly BasicProcesser processer;

        private TextPosition caretPosition;
        private ITextProcesserResult lastResult;

        public Rectangle Bounds { get; set; }

        public string Text => processer.Text;

        public void Render()
        {
            lastResult.Render(PopUpUI.Surface.BatchDrawOptions.Default, Bounds.Location.ToVector2(), null, 1f);
        }

        public void SetText(string value)
        {
            processer.Text = value;
            lastResult = processer.GetParseResult();
            caretPosition = lastResult.Caret.MaximumPosition;
        }

        public void Selected(Point mousePosition)
        {
            Vector2 inTextBoxPosition = (mousePosition - Bounds.Location).ToVector2();
            caretPosition = lastResult.Caret.MouseCaretPosition(inTextBoxPosition, null);
        }

        public void InsertChar(char ch)
        {
            if (!processer.Font.Characters.Contains(ch))
            {
                return;
            }

            int indexInText = lastResult.Caret.GetLineFreePosition(caretPosition);
            processer.Text = processer.Text.Insert(indexInText, ch.ToString());

            lastResult = processer.GetParseResult();
            caretPosition = lastResult.Caret.Advance(caretPosition, 1);
        }

        public void Backspace()
        {
            if (processer.Text.Length == 0 || (caretPosition.IndexInsideLine == 0 && caretPosition.LineIndex == 0))
                return;

            int indexInText = lastResult.Caret.GetLineFreePosition(caretPosition);
            processer.Text = processer.Text[..(indexInText - 1)] + processer.Text[indexInText..];

            lastResult = processer.GetParseResult();
            caretPosition = lastResult.Caret.GetCaretPosition(indexInText - 1);
        }

        public void KeyPressed(Keys key)
        {
            switch (key)
            {
                case Keys.Up:

                    caretPosition = lastResult.Caret.MinimumPosition;
                    break;
                case Keys.Down:

                    caretPosition = lastResult.Caret.MaximumPosition;
                    break;
                case Keys.Left:

                    caretPosition = lastResult.Caret.Advance(caretPosition, -1);
                    break;
                case Keys.Right:

                    caretPosition = lastResult.Caret.Advance(caretPosition, +1);
                    break;

                case Keys.Back:

                    Backspace();
                    break;
            }
        }
    }
}

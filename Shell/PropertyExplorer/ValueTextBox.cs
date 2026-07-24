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

        private bool isActive;
        private TextPosition caretPosition;
        private ITextProcesserResult lastResult;

        public Point Location { get; set; }
        public Point Size { get; set; }

        public bool IsActive => isActive;

        public void Render()
        {
            lastResult.Render(PopUpUI.Surface.BatchDrawOptions.Default, Location.ToVector2(), null, 1f);
        }

        public void Selected(Point mousePosition, string value)
        {
            Vector2 inTextBoxPosition = (mousePosition - Location).ToVector2();
            caretPosition = lastResult.Caret.MouseCaretPosition(inTextBoxPosition, null);

            isActive = true;
        }

        public void InsertChar(char ch)
        {
            if (ch == '\n')
            {
                isActive = false;
                return;
            }

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
                case Keys.Enter:

                    InsertChar('\n');
                    break;

                case Keys.Back:

                    Backspace();
                    break;
            }
        }
    }
}

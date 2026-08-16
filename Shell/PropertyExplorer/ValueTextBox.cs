using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using NoteKeeper.Shell.Styling;
using PopUpUI.Text;

namespace NoteKeeper.Shell.PropertyExplorer
{
    internal class ValueTextBox
    {
        private readonly TextProcesser processer;

        private TextPosition caretPosition;
        private ITextProcesserResult lastResult;

        public Rectangle Bounds { get; set; }

        public string Text => processer.Text;

        public ValueTextBox()
        {
            processer = new TextProcesser();
            FontProvider_FontChanged();

            FontProvider.FontChanged += FontProvider_FontChanged;
        }

        private void FontProvider_FontChanged()
        {
            FontProvider.UpdateProcesserFont(processer, PropertyExplorerLayoutCalculator.TextSize, isBold: false);
        }

        public void Render()
        {
            int yPadding = (Bounds.Height - FontProvider.GetLineSpacing(PropertyExplorerLayoutCalculator.TextSize, false)) / 2;
            lastResult.Render(PopUpUI.Surface.BatchDrawOptions.Default, new Vector2(Bounds.X, Bounds.Y + yPadding), null, 1f);
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

            // input inside box will be single line, so we can directly pass zero as Y value
            caretPosition = lastResult.Caret.MouseCaretPosition(new Vector2(inTextBoxPosition.X, 0), null);
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

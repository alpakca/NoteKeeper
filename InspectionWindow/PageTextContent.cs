using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PopUpUI.Text;
using PopUpUI.Toolkit;
using System.Linq;

namespace NoteKeeper.InspectionWindow
{
    internal class PageTextContent : IPageContent, IKeyTextReceiverContent
    {
        private TextPosition caretPosition;
        private ITextProcesserResult lastResult;
        private readonly TextProcesser processer;

        public bool ReceivingInput { get; set; }

        public Point Size 
        {
            get => lastResult.TextSize.ToPoint(); 
            set
            {
                processer.HorizontalTextLength = value.X;
                lastResult = processer.GetParseResult();
            }
        }
        public Point Location { get; set; }

        public PageTextContent(SpriteFont font)
        {
            processer = new TextProcesser { Font = font, TextColor = Color.Black };
            font.DefaultCharacter = '?';
        }

        public void Render(Point pageLocation)
        {
            lastResult.Render(PopUpUI.Surface.BatchDrawOptions.Default, (Location + pageLocation).ToVector2(), null, 1f);

            // draw the caret
            if (ReceivingInput)
            {
                Vector2 screenPosition = lastResult.Caret.ScreenCaretPosition(caretPosition, null);
                DrawHelper.DrawBackground(new Rectangle(pageLocation + Location + screenPosition.ToPoint(), new Point(2, processer.Font.LineSpacing)), Color.Black);
            }
        }

        public void InsertText(string str)
        {
            foreach (var ch in str)
            {
                InsertChar(ch);
            }
        }

        public void InsertChar(char ch)
        {
            if (char.IsControl(ch) && ch != '\n')
            {
                return;
            }

            processer.Text = processer.Text.Insert(lastResult.Caret.GetLineFreePosition(caretPosition), ch.ToString());

            lastResult = processer.GetParseResult();
            caretPosition = lastResult.Caret.Advance(caretPosition, 1);
        }

        public void Backspace()
        {
            if (processer.Text.Length == 0 || (caretPosition.IndexInsideLine == 0 && caretPosition.LineIndex == 0))
            {
                return;
            }

            int lineFreePosition = lastResult.Caret.GetLineFreePosition(caretPosition);
            processer.Text = processer.Text[..(lineFreePosition - 1)] + processer.Text[lineFreePosition..];

            lastResult = processer.GetParseResult();
            caretPosition = lastResult.Caret.Advance(caretPosition, -1);
        }

        public void KeyPressed(Keys key)
        {
            switch (key)
            {
                case Keys.Back:
                    Backspace();
                    break;
                case Keys.Enter:
                    InsertChar('\n');
                    break;
                case Keys.Left:
                    caretPosition = lastResult.Caret.Advance(caretPosition, -1);
                    break;
                case Keys.Right:
                    caretPosition = lastResult.Caret.Advance(caretPosition, +1);
                    break;
                case Keys.Up:

                    caretPosition = caretPosition.LineIndex == 0
                                  ? lastResult.Caret.MinimumPosition 
                                  : lastResult.Caret.Clamp(new TextPosition(caretPosition.LineIndex - 1, caretPosition.IndexInsideLine));
                    break;
                case Keys.Down:
                    caretPosition = caretPosition.LineIndex ==  lastResult.Lines.LineCount - 1
                                  ? lastResult.Caret.MaximumPosition
                                  : lastResult.Caret.Clamp(new TextPosition(caretPosition.LineIndex + 1, caretPosition.IndexInsideLine));
                    break;
            }
        }

        public void MouseSelected(Point pagePosition)
        {
            caretPosition = lastResult.Caret.MouseCaretPosition(pagePosition.ToVector2(), null);
            ReceivingInput = true;
        }

        public void InputEnded()
        {
            ReceivingInput = false;
        }
    }
}

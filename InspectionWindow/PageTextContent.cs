using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PopUpUI.Text;
using PopUpUI.Toolkit;

namespace NoteKeeper.InspectionWindow
{
    internal class PageTextContent : IPageContent, IKeyTextReceiverContent
    {
        private readonly BasicProcesser processer;

        private bool receivingInput;
        private TextPosition caretPosition;
        private ITextProcesserResult lastResult;
        private RenderTarget2D renderTarget;

        public Point Size
        {
            get;
            set
            {
                if (field == value || value.X <= 0 || value.Y <= 0)
                {
                    return;
                }

                field = value;
               
                processer.HorizontalTextLength = value.X;
                lastResult = processer.GetParseResult();

                if (field.Y < lastResult.TextSize.Y)
                {
                    field.Y = (int)lastResult.TextSize.Y;
                }

                renderTarget?.Dispose();
                renderTarget = DrawHelper.CreateRenderTarget(field.X, field.Y);
            }
        }
        public Point Location { get; set; }

        public PageTextContent(BasicProcesser processer)
        {
            this.processer = processer;
        }

        public void RenderUpdate()
        {
            DrawHelper.SetBeginBatch(renderTarget);

            lastResult.Render(PopUpUI.Surface.BatchDrawOptions.Default, new Vector2(), null, 1f);

            if (receivingInput)
            {
                Point caretPageLocation = lastResult.Caret.ScreenCaretPosition(caretPosition, null).ToPoint();
                DrawHelper.DrawBackground(new Rectangle(caretPageLocation, new Point(2, processer.Font.LineSpacing)), Color.Black);

                // draw outline indicating the boundary of content

                const int OutlineThickness = 2;
                const int OutlineMargin = 4;

                DrawHelper.DrawBackground(new Rectangle(new Point(-OutlineMargin), new Point(Size.X + 2 * OutlineMargin, OutlineThickness)), Color.Black);
                DrawHelper.DrawBackground(new Rectangle(new Point(-OutlineMargin, OutlineMargin) + new Point(0, Size.Y - OutlineThickness), new Point(Size.X + 2 * OutlineMargin, OutlineThickness)), Color.Black);

                DrawHelper.DrawBackground(new Rectangle(new Point(-OutlineMargin), new Point(OutlineThickness, Size.Y + 2 * OutlineMargin)), Color.Black);
                DrawHelper.DrawBackground(new Rectangle(new Point(OutlineMargin, -OutlineMargin) + new Point(Size.X - OutlineThickness, 0), new Point(OutlineThickness, Size.Y + 2 * OutlineMargin)), Color.Black);
            }

            DrawHelper.EndBatch();
        }

        public void RenderToPage(Point pageLocation, float zoomMutliplier)
        {
            DrawHelper.Batch.Draw(renderTarget, new Rectangle(pageLocation + Location.Times(zoomMutliplier), Size.Times(zoomMutliplier)), Color.White);
        }

        public void MouseSelected(Point pagePosition)
        {
            Vector2 inContentPosition = (pagePosition - Location).ToVector2();
            if (lastResult.TextSize.Y <= inContentPosition.Y)
            {
                caretPosition = lastResult.Caret.MaximumPosition;
            }
            else if (inContentPosition.Y < 0)
            {
                caretPosition = lastResult.Caret.MinimumPosition;
            }
            else
            {
                caretPosition = lastResult.Caret.MouseCaretPosition(inContentPosition, null);
            }

            receivingInput = true;
        }

        public void InputEnded()
        {
            receivingInput = false;
        }

        public void InsertChar(char ch)
        {
            if (ch != '\n' && !processer.Font.Characters.Contains(ch))
            {
                return;
            }

            int indexInText = lastResult.Caret.GetLineFreePosition(caretPosition);
            processer.Text = processer.Text.Insert(indexInText, ch.ToString());

            lastResult = processer.GetParseResult();
            caretPosition = lastResult.Caret.Advance(caretPosition, 1);

            if (lastResult.TextSize.Y > Size.Y)
            {
                Size = lastResult.TextSize.ToPoint();
            }
        }

        public void InsertText(string text)
        {
            int indexInText = lastResult.Caret.GetLineFreePosition(caretPosition);
            processer.Text = processer.Text.Insert(indexInText, text);

            lastResult = processer.GetParseResult();
            caretPosition = lastResult.Caret.Advance(caretPosition, text.Length);

            if (lastResult.TextSize.Y > Size.Y)
            {
                Size = lastResult.TextSize.ToPoint();
            }
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
                    
                    if (caretPosition.LineIndex == 0)
                    {
                        caretPosition = lastResult.Caret.MinimumPosition;
                    }
                    else
                    {
                        caretPosition = lastResult.Caret.Clamp(new TextPosition(caretPosition.LineIndex - 1, caretPosition.IndexInsideLine));
                    }

                    break;
                case Keys.Down:

                    if (caretPosition.LineIndex == lastResult.Lines.LineCount - 1)
                    {
                        caretPosition = lastResult.Caret.MaximumPosition;
                    }
                    else
                    {
                        caretPosition = lastResult.Caret.Clamp(new TextPosition(caretPosition.LineIndex + 1, caretPosition.IndexInsideLine));
                    }

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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PopUpUI.Text;
using PopUpUI.Toolkit;
using static System.Net.Mime.MediaTypeNames;

namespace NoteKeeper.InspectionWindow.PagedTabs
{
    internal class PageTextContent : IPageContent, IKeyTextReceiverContent
    {
        private readonly BasicProcesser processer;

        private Point size;
        private Point absSize;

        private bool receivingInput;
        private TextPosition caretPosition;
        private ITextProcesserResult lastResult;
        private RenderTarget2D renderTarget;

        public Point Location { get; set; }
        public Point Size => size;

        public PageTextContent(BasicProcesser processer)
        {
            this.processer = processer;
        }

        public void ResizeUpdate(Point change)
        {
        }

        public void SetSize(Point value)
        {
            if (size == value || value.X == 0 || value.Y == 0)
            {
                return;
            }

            size = value;
            absSize = value.Abs();

            processer.HorizontalTextLength = absSize.X;
            lastResult = processer.GetParseResult();

            if (absSize.Y < lastResult.TextSize.Y)
            {
                absSize.Y = (int)lastResult.TextSize.Y;
            }

            renderTarget?.Dispose();
            renderTarget = DrawHelper.CreateRenderTarget(absSize.X, absSize.Y);
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

                DrawHelper.DrawBackground(new Rectangle(new Point(-OutlineMargin), new Point(absSize.X + 2 * OutlineMargin, OutlineThickness)), Color.Black);
                DrawHelper.DrawBackground(new Rectangle(new Point(-OutlineMargin, OutlineMargin) + new Point(0, absSize.Y - OutlineThickness), new Point(absSize.X + 2 * OutlineMargin, OutlineThickness)), Color.Black);

                DrawHelper.DrawBackground(new Rectangle(new Point(-OutlineMargin), new Point(OutlineThickness, absSize.Y + 2 * OutlineMargin)), Color.Black);
                DrawHelper.DrawBackground(new Rectangle(new Point(OutlineMargin, -OutlineMargin) + new Point(absSize.X - OutlineThickness, 0), new Point(OutlineThickness, absSize.Y + 2 * OutlineMargin)), Color.Black);
            }

            DrawHelper.EndBatch();
        }

        public void RenderToPage(Point offset, float zoomMultiplier)
        {
            Point location = Location;
            SpriteEffects effects = SpriteEffects.None;

            if (size.X < 0)
            {
                effects |= SpriteEffects.FlipHorizontally;
                location.X += size.X;
            }

            if (size.Y < 0)
            {
                effects |= SpriteEffects.FlipVertically;
                location.Y += size.Y;
            }

            DrawHelper.Batch.Draw(renderTarget, new Rectangle(offset + location.Times(zoomMultiplier), size.Times(zoomMultiplier).Abs()),
                                  null, Color.White, 0, default, effects, 0);
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

            if (lastResult.TextSize.Y > absSize.Y)
            {
                SetSize(lastResult.TextSize.ToPoint());
            }
        }

        public void InsertText(string text)
        {
            int indexInText = lastResult.Caret.GetLineFreePosition(caretPosition);
            processer.Text = processer.Text.Insert(indexInText, text);

            lastResult = processer.GetParseResult();
            caretPosition = lastResult.Caret.Advance(caretPosition, text.Length);

            if (lastResult.TextSize.Y > absSize.Y)
            {
                SetSize(lastResult.TextSize.ToPoint());
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

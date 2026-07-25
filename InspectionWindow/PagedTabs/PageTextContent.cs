using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PopUpUI.Text;
using PopUpUI.Toolkit;

namespace NoteKeeper.InspectionWindow.PagedTabs
{
    internal class PageTextContent : IKeyTextReceiverContent
    {
        private readonly string _fontName;
        private readonly TextProcesser processer;

        private Point size;
        private Point absSize;

        private ITextProcesserResult lastResult;
        private RenderTarget2D renderTarget;

        public Point Location { get; set; }
        public Point Size => size;

        public ITextProcesserResult LastResult => lastResult;

        public PageTextContent(string fontName)
        {
            _fontName = fontName;
            processer = new TextProcesser { Font = ContentAssistant.FromLoaded<SpriteFont>(fontName) };
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

            DrawHelper.EndBatch();
        }

        public void RenderToPage(Point offset, float zoomMultiplier)
        {
            (Point transformedLocation, SpriteEffects effects) = PageContentUtil.GetTransformedLocation(Location, Size);

            DrawHelper.Batch.Draw(renderTarget, new Rectangle(offset + transformedLocation.Times(zoomMultiplier), absSize.Times(zoomMultiplier)),
                                  null, Color.White, 0, default, effects, 0);
        }

        public TextPosition MouseSelected(Point pagePosition)
        {
            Vector2 inContentPosition = (pagePosition - Location).ToVector2();
            if (lastResult.TextSize.Y <= inContentPosition.Y)
            {
                return lastResult.Caret.MaximumPosition;
            }
            else if (inContentPosition.Y < 0)
            {
                return lastResult.Caret.MinimumPosition;
            }
            else
            {
                return lastResult.Caret.MouseCaretPosition(inContentPosition, null);
            }
        }

        public TextPosition InsertChar(char ch, TextPosition caretPosition)
        {
            if (ch != '\n' && !processer.Font.Characters.Contains(ch))
            {
                return caretPosition;
            }

            int indexInText = lastResult.Caret.GetLineFreePosition(caretPosition);
            processer.Text = processer.Text.Insert(indexInText, ch.ToString());

            lastResult = processer.GetParseResult();
            caretPosition = lastResult.Caret.Advance(caretPosition, 1);

            if (lastResult.TextSize.Y > absSize.Y)
            {
                SetSize(lastResult.TextSize.ToPoint());
            }

            return caretPosition;
        }

        public TextPosition InsertText(string text, TextPosition caretPosition)
        {
            int indexInText = lastResult.Caret.GetLineFreePosition(caretPosition);
            processer.Text = processer.Text.Insert(indexInText, text);

            lastResult = processer.GetParseResult();
            caretPosition = lastResult.Caret.Advance(caretPosition, text.Length);

            if (lastResult.TextSize.Y > absSize.Y)
            {
                SetSize(lastResult.TextSize.ToPoint());
            }

            return caretPosition;
        }

        public TextPosition Backspace(TextPosition caretPosition)
        {
            if (processer.Text.Length == 0 || (caretPosition.IndexInsideLine == 0 && caretPosition.LineIndex == 0))
                return caretPosition;

            int indexInText = lastResult.Caret.GetLineFreePosition(caretPosition);
            processer.Text = processer.Text[..(indexInText - 1)] + processer.Text[indexInText..];

            lastResult = processer.GetParseResult();
            caretPosition = lastResult.Caret.GetCaretPosition(indexInText - 1);

            return caretPosition;
        }

        public TextPosition KeyPressed(Keys key, TextPosition caretPosition)
        {
            switch (key)
            {
                case Keys.Up:
                    
                    if (caretPosition.LineIndex == 0)
                    {
                        return lastResult.Caret.MinimumPosition;
                    }
                    else
                    {
                        return lastResult.Caret.Clamp(new TextPosition(caretPosition.LineIndex - 1, caretPosition.IndexInsideLine));
                    }

                case Keys.Down:

                    if (caretPosition.LineIndex == lastResult.Lines.LineCount - 1)
                    {
                        return lastResult.Caret.MaximumPosition;
                    }
                    else
                    {
                        return lastResult.Caret.Clamp(new TextPosition(caretPosition.LineIndex + 1, caretPosition.IndexInsideLine));
                    }

                case Keys.Left:

                    return lastResult.Caret.Advance(caretPosition, -1);

                case Keys.Right:

                    return lastResult.Caret.Advance(caretPosition, +1);

                case Keys.Enter:

                    return InsertChar('\n', caretPosition);

                case Keys.Back:

                    return Backspace(caretPosition);

                default:
                    return caretPosition;
            }
        }
    }
}

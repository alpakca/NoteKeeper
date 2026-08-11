using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NoteKeeper.InspectionWindow.MultiPage.Input;
using NoteKeeper.InspectionWindow.MultiPage.Serialization;
using PopUpUI.Text;
using PopUpUI.Toolkit;

namespace NoteKeeper.InspectionWindow.MultiPage
{
    internal class PageTextContent : IKeyTextReceiverContent
    {
        private readonly TextProcesser processer;

        private Point location;
        private Point size;
        private Point absSize;

        private int _fontSize;
        private string _fontName;

        private ITextProcesserResult lastResult;
        private RenderTarget2D renderTarget;

        public Point Location => location;
        public Point Size => size;

        public int FontSize => _fontSize;
        public string FontName => _fontName;

        public string Text
        {
            get => processer.Text;
            set
            {
                processer.Text = value;
                lastResult = processer.GetParseResult();
            }
        }
        public ITextProcesserResult LastResult => lastResult;

        public PageTextContent()
        {
            processer = new TextProcesser() { TextColor = Color.Black };
        }

        public void SetFont(string fontName, int fontSize)
        {
            _fontSize = fontSize;
            _fontName = fontName;

            processer.Font = ContentAssistant.FromLoaded<SpriteFont>(fontName + "_" + fontSize);
            lastResult = processer.GetParseResult();
        }

        public void ResizeUpdate(Point value)
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

        public void RelocationUpdate(Point value)
        {
            location = value;
        }

        public void SetLocation(Point value)
        {
            location = value;
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

        public TextPosition GetMouseCaretPosition(Point pagePosition)
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

        public void InsertChar(char ch, WritingSession writingSession)
        {
            if (ch != '\n' && !processer.Font.Characters.Contains(ch))
            {
                return;
            }

            int indexInText = lastResult.Caret.GetLineFreePosition(writingSession.CaretPosition);
            processer.Text = processer.Text.Insert(indexInText, ch.ToString());

            lastResult = processer.GetParseResult();
            writingSession.CaretPosition = lastResult.Caret.Advance(writingSession.CaretPosition, 1);

            ExpandHeightIfNeed();
        }

        public void InsertText(string text, WritingSession writingSession)
        {
            int indexInText = lastResult.Caret.GetLineFreePosition(writingSession.CaretPosition);
            processer.Text = processer.Text.Insert(indexInText, text);

            lastResult = processer.GetParseResult();
            writingSession.CaretPosition = lastResult.Caret.Advance(writingSession.CaretPosition, text.Length);

            ExpandHeightIfNeed();
        }

        private void ExpandHeightIfNeed()
        {
            if (lastResult.TextSize.Y > absSize.Y)
            {
                // let lastResult's size copy the signs of actual size (so rendered text won't become normal from a upside state)
                Point transformedSize = new Point((int)lastResult.TextSize.X * int.Sign(size.X), (int)lastResult.TextSize.Y * int.Sign(size.Y));
                SetSize(transformedSize);
            }
        }

        public void Backspace(WritingSession writingSession)
        {
            if (processer.Text.Length == 0 || (writingSession.CaretPosition.IndexInsideLine == 0 && writingSession.CaretPosition.LineIndex == 0))
                return;

            int indexInText = lastResult.Caret.GetLineFreePosition(writingSession.CaretPosition);
            processer.Text = processer.Text[..(indexInText - 1)] + processer.Text[indexInText..];

            lastResult = processer.GetParseResult();
            writingSession.CaretPosition = lastResult.Caret.GetCaretPosition(indexInText - 1);
        }

        public void KeyPressed(Keys key, WritingSession writingSession)
        {
            TextPosition caretPosition = writingSession.CaretPosition;
            switch (key)
            {
                case Keys.Up:
                    
                    if (writingSession.CaretPosition.LineIndex == 0)
                    {
                        writingSession.CaretPosition = lastResult.Caret.MinimumPosition;
                    }
                    else
                    {
                        writingSession.CaretPosition = lastResult.Caret.Clamp(new TextPosition(caretPosition.LineIndex - 1, caretPosition.IndexInsideLine));
                    }

                    break;
                case Keys.Down:

                    if (caretPosition.LineIndex == lastResult.Lines.LineCount - 1)
                    {
                        writingSession.CaretPosition = lastResult.Caret.MaximumPosition;
                    }
                    else
                    {
                        writingSession.CaretPosition = lastResult.Caret.Clamp(new TextPosition(caretPosition.LineIndex + 1, caretPosition.IndexInsideLine));
                    }

                    break;
                case Keys.Left:

                    writingSession.CaretPosition = lastResult.Caret.Advance(caretPosition, -1);
                    break;
                case Keys.Right:

                    writingSession.CaretPosition = lastResult.Caret.Advance(caretPosition, +1);
                    break;
                case Keys.Enter:

                    InsertChar('\n', writingSession);
                    break;
                case Keys.Back:

                    Backspace(writingSession);
                    break;
            }
        }

        public PageContentInfo GetSerializationInfo()
        {
            return new TextContentInfo
            {
                Location = location,
                Size = size,

                Text = this.Text,
                FontSize = _fontSize,
                FontName = _fontName
            };
        }

        public static PageTextContent Create(PageContentInfo info)
        {
            if (info is not TextContentInfo textInfo)
            {
                throw new System.Exception();
            }

            PageTextContent content = new PageTextContent();
            content.SetLocation(textInfo.Location);
            content.SetSize(textInfo.Size);

            content.SetFont(textInfo.FontName, textInfo.FontSize);
            content.Text = textInfo.Text;

            return content;
        }
    }
}

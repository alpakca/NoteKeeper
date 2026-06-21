using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PopUpUI.Input;
using PopUpUI.Toolkit;

#nullable enable

namespace NoteKeeper.InspectionWindow.SinglePage
{
    internal class TabInputHandler
    {
        private IKeyTextReceiverContent? currentKeyTextReceiver;

        private readonly NotePage page;
        private readonly TabViewport viewport;

        public ShellModes CurrentMode { get; set; }

        public TabInputHandler(NotePage page, TabViewport viewport)
        {
            this.page = page;
            this.viewport = viewport;
        }

        public IPageContent? HitContent(Point mousePosition)
        {
            Point pagePosition = viewport.ToPagePosition(mousePosition);

            for (int i = page.Contents.Count - 1; i >= 0; i--)
            {
                IPageContent content = page.Contents[i];
                if (content.Location.X <= pagePosition.X && pagePosition.X < content.Location.X + content.Size.X &&
                    content.Location.Y <= pagePosition.Y && pagePosition.Y < content.Location.Y + content.Size.Y)
                {
                    return content;
                }
            }

            return null;
        }

        public void LeftMouseSelection(Point mousePosition)
        {
            if (CurrentMode == ShellModes.Writing)
            {
                IKeyTextReceiverContent? hit = HitContent(mousePosition) as IKeyTextReceiverContent;
                if (currentKeyTextReceiver != hit)
                {
                    currentKeyTextReceiver?.InputEnded();
                    currentKeyTextReceiver = hit;
                }

                currentKeyTextReceiver?.MouseSelected(viewport.ToPagePosition(mousePosition));
            }
        }

        public void TextInput(PopWindowMouseEventArgs e, char ch)
        {
            currentKeyTextReceiver?.InsertChar(ch);
        }

        public void KeyInput(PopWindowMouseEventArgs e, Keys key)
        {
            currentKeyTextReceiver?.KeyPressed(key);
        }
    }
}

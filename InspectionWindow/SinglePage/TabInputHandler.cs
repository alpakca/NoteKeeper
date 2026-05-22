using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
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
            Point pageTopLeftPosition = viewport.PageTopLeftPosition;
            mousePosition -= pageTopLeftPosition;

            mousePosition = viewport.ZoomedPosition(mousePosition);

            for (int i = page.Contents.Count - 1; i >= 0; i--)
            {
                IPageContent content = page.Contents[i];
                if (content.Location.X <= mousePosition.X && mousePosition.X < content.Location.X + content.Size.X &&
                    content.Location.Y <= mousePosition.Y && mousePosition.Y < content.Location.Y + content.Size.Y)
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

                currentKeyTextReceiver?.MouseSelected(viewport.RelativeToPage(mousePosition));
            }
        }

        public void TextInput(char ch)
        {
            currentKeyTextReceiver?.InsertChar(ch);
        }

        public void KeyInput(Keys key)
        {
            if (currentKeyTextReceiver == null)
            {
                if (InputManager.IsDown(Keys.LeftControl))
                {
                    viewport.Zoom += 0.1f;
                }
                else if (InputManager.IsDown(Keys.Left))
                {
                    viewport.ScrolledDistance += new Point(10, 0);
                }
                else if (InputManager.IsDown(Keys.Right))
                {
                    viewport.ScrolledDistance += new Point(-10, 0);
                }
            }

            currentKeyTextReceiver?.KeyPressed(key);
        }
    }
}

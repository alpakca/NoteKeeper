using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PopUpUI.Toolkit;

namespace NoteKeeper.InspectionWindow.PagedTabs
{
    internal class PageImageContent : IPageContent
    {
        private readonly Texture2D image;

        public Point Size { get; set; }
        public Point Location { get; set; }

        public PageImageContent(Texture2D image)
        {
            this.image = image;
        }

        public void RenderUpdate()
        {
        }

        public void RenderToPage(Point offset, float zoomMultiplier)
        {
            DrawHelper.Batch.Draw(image, new Rectangle(offset + Location.Times(zoomMultiplier), Size.Times(zoomMultiplier)), Color.White);
        }
    }
}

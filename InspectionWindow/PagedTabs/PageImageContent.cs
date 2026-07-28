using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PopUpUI.Toolkit;

namespace NoteKeeper.InspectionWindow.PagedTabs
{
    internal class PageImageContent : IPageContent
    {
        private Point location;
        private Point size;
        private readonly Texture2D image;

        public Point Location => location;
        public Point Size => size;

        public PageImageContent(Texture2D image)
        {
            this.image = image;
            size = image.Bounds.Size;
        }

        public PageImageContent(Texture2D image, Point size)
        {
            this.image = image;
            this.size = size;
        }

        public void RenderUpdate()
        {
        }

        public void RenderToPage(Point offset, float zoomMultiplier)
        {
            (Point transformedLocation, SpriteEffects effects) = PageContentUtil.GetTransformedLocation(Location, Size);

            DrawHelper.Batch.Draw(image, new Rectangle(offset + transformedLocation.Times(zoomMultiplier), size.Times(zoomMultiplier).Abs()), 
                                  null, Color.White, 0, default, effects, 0);
        }

        public void ResizeUpdate(Point value)
        {
            size = value;
        }

        public void SetSize(Point value)
        {
            size = value;
        }

        public void RelocationUpdate(Point value)
        {
            location = value;
        }

        public void SetLocation(Point value)
        {
            location = value;
        }
    }
}

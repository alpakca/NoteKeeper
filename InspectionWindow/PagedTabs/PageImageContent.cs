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
            Size = image.Bounds.Size;
        }

        public void RenderUpdate()
        {
        }

        public void RenderToPage(Point offset, float zoomMultiplier)
        {
            Point location = Location;
            SpriteEffects effects = SpriteEffects.None;

            if (Size.X < 0)
            {
                effects |= SpriteEffects.FlipHorizontally;
                location.X += Size.X;
            }

            if (Size.Y < 0)
            {
                effects |= SpriteEffects.FlipVertically;
                location.Y += Size.Y;
            }

            DrawHelper.Batch.Draw(image, new Rectangle(offset + location.Times(zoomMultiplier), Size.Times(zoomMultiplier).Abs()), 
                                  null, Color.White, 0, default, effects, 0);
        }
    }
}

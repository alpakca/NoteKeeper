using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PopUpUI.Toolkit;

namespace NoteKeeper.InspectionWindow
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

        public void Render(Point pageLocation)
        {
            DrawHelper.Batch.Draw(image, new Rectangle(Location + pageLocation, Size), Color.White);
        }
    }
}

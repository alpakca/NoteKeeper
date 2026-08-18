using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NoteKeeper.InspectionWindow.MultiPage.Serialization;
using PopUpUI.Toolkit;
using System.Diagnostics;

namespace NoteKeeper.InspectionWindow.MultiPage
{
    internal class PageImageContent : IPageContent
    {
        private Point location;
        private Point size;
        private readonly string _imagePath;
        private readonly Texture2D image;

        public Point Location => location;
        public Point Size => size;

        public PageImageContent(string imagePath)
        {
            _imagePath = imagePath;
            image = DrawHelper.FromFile(imagePath);
            this.size = new Point(image.Width, image.Height);
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

        public PageContentInfo GetSerializationInfo()
        {
            return new ImageContentInfo
            {
                Location = location,
                Size = size,

                ImagePath = _imagePath
            };
        }

        public static PageImageContent Create(PageContentInfo info)
        {
            if (info is not ImageContentInfo imageInfo)
            {
                throw new System.Exception();
            }

            PageImageContent content = new PageImageContent(imageInfo.ImagePath);
            content.SetLocation(imageInfo.Location);
            content.SetSize(imageInfo.Size);

            return content;
        }
    }
}

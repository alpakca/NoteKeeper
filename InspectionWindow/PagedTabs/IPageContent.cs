using Microsoft.Xna.Framework;
using NoteKeeper.InspectionWindow.PagedTabs.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.InspectionWindow.PagedTabs
{
    internal interface IPageContent
    {
        public Point Location { get; }
        public Point Size { get; }

        /// <summary>An update method that can be used to pre-render into textures inside content.</summary>
        public void RenderUpdate();
        /// <summary>Renders directly into page, using <see cref="PopUpUI.Toolkit.DrawHelper.Batch"/> with applying given offset and zoom.</summary>
        /// <param name="offset">Offset to apply addition to location of content.</param>
        /// <param name="zoomMultiplier">A zoom mutliplier of viewport of rendering page, to apply to <see cref="Size"/> and <see cref="Location"/> of content.</param>
        public void RenderToPage(Point offset, float zoomMultiplier);

        public void ResizeUpdate(Point value);
        public void SetSize(Point value);

        public void RelocationUpdate(Point value);
        public void SetLocation(Point value);

        public PageContentInfo GetSerializationInfo();
    }
}

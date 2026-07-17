using Microsoft.Xna.Framework;
using PopUpUI.Input;
using PopUpUI.Surface;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace NoteKeeper.InspectionWindow.MultiPage
{
    internal class MultiPageTab : Tab
    {
        private readonly TabViewport viewport;
        private readonly TabWindow window;

        public override ITabViewport TabViewport => viewport;
        public override PopWindow Window => window;

        public ObservableCollection<NotePage> NotePages { get; }

        public MultiPageTab(Point size)
        {
            NotePages = new ObservableCollection<NotePage>();

            viewport = new TabViewport { UISize = size };
            window = new TabWindow(size, viewport, NotePages);

            NotePages.CollectionChanged += (sender, _) => viewport.PageCount = ((ObservableCollection<NotePage>)sender).Count;
        }

        public override void AddContent(IPageContent content)
        {
        }

        public override Point GetContentScreenLocation(IPageContent content)
        {
            throw new NotImplementedException();
        }

        public override void ShellModeUpdated(ShellModes mode)
        {
        }

        public override void SimulateMouseButtonStateInput(PopWindowMouseEventArgs e)
        {
        }
    }
}

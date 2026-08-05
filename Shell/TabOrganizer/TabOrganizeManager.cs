using Microsoft.Xna.Framework;
using NoteKeeper.InspectionWindow;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.Shell.TabOrganizer
{
    internal class TabOrganizeManager
    {
        private readonly TabsHeaderLayoutCalculator layoutCalculator;
        private readonly TabsHeaderRenderer renderer;

        public TabsHeaderStyleProvider StyleProvider { get; }
        public Rectangle ScreenBounds
        {
            get => layoutCalculator.ScreenBounds;
            set => layoutCalculator.ScreenBounds = value;
        }

        public TabOrganizeManager()
        {
            StyleProvider = new TabsHeaderStyleProvider();

            layoutCalculator = new TabsHeaderLayoutCalculator(StyleProvider);
            renderer = new TabsHeaderRenderer(StyleProvider, layoutCalculator);
        }

        public void Render(IList<Tab> activeTabs, Tab? displayedTab)
        {
            renderer.Render(activeTabs, displayedTab, null, false, false);
        }
    }
}

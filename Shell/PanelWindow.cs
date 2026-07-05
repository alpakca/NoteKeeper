using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using NoteKeeper.InspectionWindow;
using PopUpUI.Surface;
using PopUpUI.Toolkit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

#nullable enable

namespace NoteKeeper.Shell
{
    internal class PanelWindow : PopWindow, ITabUpdateReceiver
    {
        private Tab? displayedTab;

        public PanelWindow(Point size) : base(size)
        {
        }

        protected override void Update()
        {
            if (InputManager.AllPressed(Keys.LeftControl, Keys.Add))
            {
                displayedTab?.TabViewport.Zoom += 0.1f;
            }
            else if (InputManager.AllPressed(Keys.LeftControl, Keys.Subtract))
            {
                displayedTab?.TabViewport.Zoom -= 0.1f;
            }

            base.Update();
        }

        public void DisplayedTabChanged(Tab setTab)
        {
            displayedTab = setTab;
        }
    }
}

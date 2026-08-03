using Microsoft.Xna.Framework;
using NoteKeeper.Shell.ToolBar;

namespace NoteKeeper.InspectionWindow.PagedTabs.MultiPage
{
    internal class TabToolMenu : IToolMenu
    {
        public ActionToolButton[] Buttons { get; }
        public ToolMenuStyle MenuStyle { get; }

        public TabToolMenu()
        {
            MenuStyle = new ToolMenuStyle
            {
                Background = Util.GetTexture(1, 1, Color.Yellow),
                CloseButtonTexture = Util.GetTexture(1, 1, Color.Red),
                ColumnCount = 2,
            };

            Buttons = [new ActionToolButton(() => { }) {
                DefaultTexture = Util.GetTexture(1, 1, Color.Turquoise),
                HoveredTexture = Util.GetTexture(1, 1, Color.DarkGray * 0.5f),
                HeldTexture = Util.GetTexture(1, 1, Color.LightGray * 0.5f)
            }];
        }
    }
}

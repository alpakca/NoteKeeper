using Microsoft.Xna.Framework;
using NoteKeeper.Shell.PUIHelpers;
using NoteKeeper.Shell.ToolBar;
using PopUpUI.Elements;
using PopUpUI.Input;
using PopUpUI.PUI;

namespace NoteKeeper.InspectionWindow.PagedTabs.MultiPage
{
    internal class TabToolMenu : IToolMenu
    {
        private readonly MultiPageTab _tab;

        public IToolButton[] Buttons { get; }
        public ToolMenuStyle MenuStyle { get; }

        public TabToolMenu(MultiPageTab tab)
        {
            _tab = tab;

            MenuStyle = new ToolMenuStyle
            {
                Background = Util.GetTexture(1, 1, Color.Yellow),
                CloseButtonTexture = Util.GetTexture(1, 1, Color.Red),
                CloseButtonHoverTexture = Util.GetTexture(1, 1, Color.DarkRed),
                CloseButtonHeldTexture = Util.GetTexture(1, 1, Color.LightPink),
                ColumnCount = 2,
            };

            PUIInterpreter interpreter = PUIHelper.GetInterpreter(PUIHelper.MultiPageTabAssetsFolderPath + "ToolMenu.pui");
            interpreter.ProcessTokens();

            ActionToolButton addTextBlock = new ActionToolButton(AddTextBlockCommand);
            interpreter.AssignProperties(PUIHelper.ActionToolButtonAssigner, "Add_Text_Block", addTextBlock);

            Buttons = [addTextBlock];
        }

        private void AddTextBlockCommand()
        {
            PageTextContent content = new PageTextContent("Font_ComicSans");
            _tab.AddContent(content);
            _tab.RequestShellModeChange(ShellModes.Writing);

            // apply margin to Screen Location of content that may appear a little lower due to rounding errors
            // which shouldn't happen with zoom value of 1f
            const int RoundingErrorMargin = 3;

            _tab.SimulateMouseButtonStateInput(
                new PopWindowMouseEventArgs(MouseButtons.Left, MouseButtons.None, MouseButtons.None,
                                            _tab.GetContentScreenLocation(content) + new Point(RoundingErrorMargin)));
        }
    }
}

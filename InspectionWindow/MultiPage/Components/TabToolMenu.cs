using Microsoft.Xna.Framework;
using NoteKeeper.Shell.PUIHelpers;
using NoteKeeper.Shell.ToolBar;
using PopUpUI.Input;
using PopUpUI.PUI;

namespace NoteKeeper.InspectionWindow.MultiPage.Components
{
    internal class TabToolMenu : IToolMenu
    {
        private readonly MultiPageTab _tab;
        private readonly PageActions _pageActions;

        public IToolButton[] Buttons { get; }
        public ToolMenuStyle MenuStyle { get; }

        public TabToolMenu(MultiPageTab tab, PageActions pageActions)
        {
            _tab = tab;
            _pageActions = pageActions;

            MenuStyle = new ToolMenuStyle
            {
                Background = Util.GetTexture(1, 1, Color.Yellow),
                CloseButtonTexture = Util.GetTexture(1, 1, Color.Red),
                CloseButtonHoverTexture = Util.GetTexture(1, 1, Color.DarkRed),
                CloseButtonHeldTexture = Util.GetTexture(1, 1, Color.LightPink),
                MoveBoxTexture = Util.GetTexture(1, 1, Color.Turquoise),
                ColumnCount = 2,
            };

            PUIInterpreter interpreter = PUIHelper.GetInterpreter(PUIHelper.MultiPageTabAssetsFolderPath + "ToolMenu.pui");
            interpreter.ProcessTokens();

            ActionToolButton addTextBlock = new ActionToolButton(AddTextBlock);
            interpreter.AssignProperties(PUIHelper.ToolButtonAssigner, "Add_Text_Block", addTextBlock);

            ToggleToolButton swapPages = new ToggleToolButton(SwapNotePagesToggled);
            interpreter.AssignProperties(PUIHelper.ToolButtonAssigner, "Swap_Pages", swapPages);

            ActionToolButton appendPage = new ActionToolButton(AppendPage);
            interpreter.AssignProperties(PUIHelper.ToolButtonAssigner, "Append_Page", appendPage);

            Buttons = [addTextBlock, swapPages, appendPage];
        }

        private void AddTextBlock()
        {
            PageTextContent content = new PageTextContent();
            content.SetFont("Font_ComicSans", 16);
            _tab.AddContent(content);
            _tab.RequestShellModeChange(ShellModes.Writing);

            // apply margin to Screen Location of content that may appear a little lower due to rounding errors
            // which shouldn't happen with zoom value of 1f
            const int RoundingErrorMargin = 3;

            _tab.SimulateMouseButtonStateInput(
                new PopWindowMouseEventArgs(MouseButtons.Left, MouseButtons.Left, MouseButtons.None,
                                            _tab.GetContentScreenLocation(content) + new Point(RoundingErrorMargin)));
        }

        private void SwapNotePagesToggled(ToggleToolButton button)
        {
            ((TabWindow)_tab.Window).WindowInputStateUpdated(button.IsChecked ? WindowInputState.SwappingPages : WindowInputState.Default);
        }

        private void AppendPage()
        {
            _pageActions.AddPage(new NotePage(), _tab.NotePages);

            TabViewport viewport = (TabViewport)_tab.TabViewport;

            // substract page spacing so two pages can be seen when scrolled
            int scrollOffset = (viewport.PageCount - 1) * viewport.MovingDimensionPageStepLength - viewport.PageSpacing;

            if (viewport.InVerticalMode)
            {
                viewport.SetVerticalScroll(scrollOffset);
            }
            else
            {
                viewport.SetHorizontalScroll(scrollOffset);
            }
        }
    }
}

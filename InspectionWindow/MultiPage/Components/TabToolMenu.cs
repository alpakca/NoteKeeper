using Microsoft.Xna.Framework;
using NoteKeeper.Shell.ChildWindow;
using NoteKeeper.Shell.PUIHelpers;
using NoteKeeper.Shell.ToolBar;
using PopUpUI.Input;
using PopUpUI.PUI;
using System;
using System.IO;

namespace NoteKeeper.InspectionWindow.MultiPage.Components
{
    internal class TabToolMenu : IToolMenu
    {
        private readonly MultiPageTab _tab;
        private readonly PageActions _pageActions;

        private readonly ToggleToolButton[] _combinedToggles;

        public IToolButton[] Buttons { get; }

        public event Action<WindowInputState> WindowInputStateChanged;
        public event Action<bool> TransformerCornerLockStateChanged;
        public event Action<string> SavingPagesRequested;

        public TabToolMenu(MultiPageTab tab, PageActions pageActions)
        {
            _tab = tab;
            _pageActions = pageActions;

            PUIInterpreter interpreter = PUIHelper.GetInterpreter(PUIHelper.MultiPageTabAssetsFolderPath + "ToolMenu.pui");
            interpreter.ProcessTokens();

            ToggleToolButton transformContents = new ToggleToolButton(button => ButtonToggled(button, WindowInputState.Transform));
            interpreter.AssignProperties(PUIHelper.ToolButtonAssigner, "Transforming_Mode", transformContents);

            ToggleToolButton writingContent = new ToggleToolButton(button => ButtonToggled(button, WindowInputState.Writing));
            interpreter.AssignProperties(PUIHelper.ToolButtonAssigner, "Writing_Mode", writingContent);

            ToggleToolButton swapPages = new ToggleToolButton(button => ButtonToggled(button, WindowInputState.SwappingPages));
            interpreter.AssignProperties(PUIHelper.ToolButtonAssigner, "Swap_Pages", swapPages);

            ActionToolButton addTextBlock = new ActionToolButton(AddTextBlock);
            interpreter.AssignProperties(PUIHelper.ToolButtonAssigner, "Add_Text_Block", addTextBlock);

            ActionToolButton appendPage = new ActionToolButton(AppendPage);
            interpreter.AssignProperties(PUIHelper.ToolButtonAssigner, "Append_Page", appendPage);

            ToggleToolButton lockTransformerCorner = new ToggleToolButton(button => TransformerCornerLockStateChanged?.Invoke(button.IsChecked));
            interpreter.AssignProperties(PUIHelper.ToolButtonAssigner, "Lock_Transformer_Corner", lockTransformerCorner);

            ActionToolButton savePages = new ActionToolButton(SavePages);
            interpreter.AssignProperties(PUIHelper.ToolButtonAssigner, "Save_Pages", savePages);

            ActionToolButton changeOrientation = new ActionToolButton(ChangeOrientation);
            interpreter.AssignProperties(PUIHelper.ToolButtonAssigner, "Change_Orientation", changeOrientation);

            _combinedToggles = [transformContents, writingContent, swapPages];
            Buttons = [transformContents, writingContent, swapPages, addTextBlock, appendPage, lockTransformerCorner, savePages, changeOrientation];
        }

        private void ButtonToggled(ToggleToolButton button, WindowInputState state)
        {
            bool isChecked = button.IsChecked;

            WindowInputStateChanged?.Invoke(button.IsChecked ? state : WindowInputState.Default);
            foreach (var toggle in _combinedToggles)
            {
                if (toggle == button) continue;

                toggle.SilentSetCheckStatus(false);
            }

            button.SilentSetCheckStatus(isChecked);
        }

        private void AddTextBlock()
        {
            PageTextContent content = new PageTextContent();
            content.SetFont("ComicSans", 16);

            _tab.AddContent(content);
            WindowInputStateChanged?.Invoke(WindowInputState.Writing);

            // apply margin to Screen Location of content that may appear a little lower due to rounding errors
            // which shouldn't happen with zoom value of 1f
            const int RoundingErrorMargin = 3;

            _tab.SimulateMouseButtonStateInput(
                new PopWindowMouseEventArgs(MouseButtons.Left, MouseButtons.Left, MouseButtons.None,
                                            _tab.GetContentScreenLocation(content) + new Point(RoundingErrorMargin)));
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

        private void SavePages()
        {
            string directoryPath = Path.ChangeExtension(_tab.SourcePath, null) + "Pages";
            SavingPagesRequested?.Invoke(directoryPath);
        }

        private void ChangeOrientation()
        {
            ((TabViewport)_tab.TabViewport).ToggleOrientation();
        }
    }
}

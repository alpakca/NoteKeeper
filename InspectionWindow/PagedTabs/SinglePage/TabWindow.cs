using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PopUpUI.Input;
using PopUpUI.Surface;
using PopUpUI.Elements;

namespace NoteKeeper.InspectionWindow.PagedTabs.SinglePage
{
    internal class TabWindow : PopWindow
    {
        private ShellModes currentMode;

        private readonly TabRenderer renderer;
        private readonly TabViewport viewport;
        private readonly ContentTransformer transformer;
        private readonly TabInputHandler inputHandler;

        public TabWindow(NotePage page, TabViewport viewport) : base(new Point(viewport.ViewedWidth, viewport.ViewedHeight))
        {
            this.viewport = viewport;

            transformer = new ContentTransformer();
            renderer = new TabRenderer(page, viewport);
            inputHandler = new TabInputHandler(page, viewport);

            ModeUpdated(ShellModes.Transform);
        }

        protected override void RenderUpdate()
        {
            renderer.RenderPage();

            if (currentMode == ShellModes.Transform && transformer.IsTransforming)
            {
                renderer.RenderTransformerNodes(transformer.NodePagePositions);
            }

            // render to window texture at last
            renderer.RenderToWindowTexture(WindowTexture);
        }

        public void ModeUpdated(ShellModes mode)
        {
            currentMode = mode;
            inputHandler.CurrentMode = mode;
        }

        protected override void OnMousePositionChange(PopWindowMouseEventArgs e)
        {
            if (currentMode == ShellModes.Transform)
            {
                if (transformer.AnyNodeSelected)
                {
                    transformer.MouseMove(viewport.ToPagePosition(e.MousePosition));
                }
            }
        }

        protected override void OnMouseButtonStateChange(PopWindowMouseEventArgs e)
        {
            if (currentMode == ShellModes.Transform)
            {
                if (!transformer.HitAnyNode(viewport.ToPagePosition(e.MousePosition)))
                {
                    IPageContent hitContent = inputHandler.HitContent(e.MousePosition);
                    if (transformer.TransformedContent != hitContent && e.FirstTimeButtons.HasLeft)
                    {
                        transformer.SetTransformedContent(hitContent, 0);
                    }
                }

                if (!transformer.AnyNodeSelected && e.FirstTimeButtons.HasLeft)
                {
                    transformer.MouseHoldStart(viewport.ToPagePosition(e.MousePosition), e.MousePosition);
                }
                else if (transformer.AnyNodeSelected && !e.CurrentButtons.HasLeft)
                {
                    transformer.MouseHoldEnd();
                }
            }
            else if (e.FirstTimeButtons.HasLeft)
            {
                inputHandler.LeftMouseSelection(e.MousePosition);
            }
        }

        public void SimulateMouseButtonStateInput(PopWindowMouseEventArgs e)
        {
            OnMouseButtonStateChange(e);
        }

        protected override void OnTextInput(PopWindowMouseEventArgs e, char ch)
        {
            inputHandler.TextInput(e, ch);
        }

        protected override void OnKeyDown(PopWindowMouseEventArgs e, Keys key)
        {
            inputHandler.KeyInput(e, key);
        }

        protected override void OnMouseVerticalScrollChange(PopWindowMouseEventArgs e, int change)
        {
            viewport.ScrollVertical(-change / 6);
        }

        protected override void OnMouseHorizontalScrollChange(PopWindowMouseEventArgs e, int change)
        {
            viewport.ScrollHorizontal(-change / 6);
        }
    }
}

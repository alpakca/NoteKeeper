using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PopUpUI.Input;
using PopUpUI.Surface;
using PopUpUI.Elements;
using System.Diagnostics;

namespace NoteKeeper.InspectionWindow.SinglePage
{
    internal class TabWindow : PopWindow
    {
        private ShellModes currentMode;

        private readonly TabRenderer renderer;
        private readonly TabViewport viewport;
        private readonly PageContentTransformer transformer;
        private readonly TabInputHandler inputHandler;

        public TabWindow(NotePage page, TabViewport viewport) : base(new Point(viewport.VisualWidth, viewport.VisualHeight))
        {
            this.viewport = viewport;

            transformer = new PageContentTransformer();
            renderer = new TabRenderer(page, viewport);
            inputHandler = new TabInputHandler(page, viewport);

            ModeUpdated(ShellModes.Transform);
        }

        protected override void RenderUpdate()
        {
            renderer.RenderPage();

            if (currentMode == ShellModes.Transform && transformer.IsTransforming)
            {
                renderer.RenderTransformer(transformer.NodeScreenPositions);
            }

            // render to window texture at last
            renderer.RenderWindowTexture(WindowTexture);
        }

        public void ModeUpdated(ShellModes mode)
        {
            currentMode = mode;
            inputHandler.CurrentMode = mode;
        }

        protected override void OnMousePositionChange(PopUpWindowEventArgs e)
        {
            if (currentMode == ShellModes.Transform)
            {
                if (transformer.AnyNodeSelected)
                {
                    transformer.MouseMove(e.MousePoint - viewport.PageTopLeftPosition);
                }
            }
        }

        protected override void OnMouseButtonStateChange(PopUpWindowEventArgs e)
        {
            if (currentMode == ShellModes.Transform)
            {
                if (!transformer.HitAnyNode(e.MousePoint - viewport.PageTopLeftPosition))
                {
                    IPageContent hitContent = inputHandler.HitContent(e.MousePoint);
                    if (transformer.TransformedContent != hitContent && e.NewlyPressedButtons.HasFlag(MouseButtons.Left))
                    {
                        transformer.TransformedContent = hitContent;
                    }
                }

                if (!transformer.AnyNodeSelected && e.NewlyPressedButtons.HasFlag(MouseButtons.Left))
                {
                    transformer.MouseHoldStart(e.MousePoint - viewport.PageTopLeftPosition);
                }
                else if (transformer.AnyNodeSelected && !e.PressedButtons.HasFlag(MouseButtons.Left))
                {
                    transformer.MouseHoldEnd();
                }
            }
            else if (e.NewlyPressedButtons.HasFlag(MouseButtons.Left))
            {
                inputHandler.LeftMouseSelection(e.MousePoint);
            }
        }

        protected override void OnTextInput(PopUpWindowEventArgs e, char ch)
        {
            inputHandler.TextInput(ch);
        }

        protected override void OnKeyDown(PopUpWindowEventArgs e, Keys key)
        {
            inputHandler.KeyInput(key);
        }

        protected override void OnMouseWheelChange(PopUpWindowEventArgs e)
        {
            viewport.ScrolledDistance = new Point(viewport.ScrolledDistance.X, viewport.ScrolledDistance.Y + e.MouseWheelChange / 6);   
        }
    }
}

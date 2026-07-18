using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PopUpUI.Input;
using PopUpUI.Surface;
using PopUpUI.Toolkit;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.InspectionWindow.PagedTabs.MultiPage
{
    internal class TabWindow : PopWindow
    {
        private ShellModes currentMode;

        private readonly TabViewport viewport;
        private readonly TabRenderer renderer;
        private readonly TabInputHandler inputHandler;
        private readonly ContentTransformer transformer;

        public TabWindow(Point size, TabViewport viewport, IList<NotePage> notePages) : base(size)
        {
            this.viewport = viewport;
            renderer = new TabRenderer(viewport, notePages);
            inputHandler = new TabInputHandler(viewport, notePages);
            transformer = new ContentTransformer(notePages, viewport);
        }

        public void ModeUpdated(ShellModes mode)
        {
            currentMode = mode;
            inputHandler.CurrentMode = mode;
        }

        protected override void RenderUpdate()
        {
            renderer.RenderPageBackgrounds();

            if (currentMode == ShellModes.Transform && transformer.IsTransforming)
            {
                renderer.RenderTransformerNodes(transformer.TransformedContentPageIndex, transformer.NodePagePositions);
            }

            renderer.RenderWindowTexture(WindowTexture);
        }

        protected override void OnMousePositionChange(PopWindowMouseEventArgs e)
        {
            if (currentMode == ShellModes.Transform)
            {
                if (transformer.AnyNodeSelected)
                {
                    (int pageIndex, _) = viewport.ToPagePosition(e.MousePosition);
                    transformer.MouseMove(pageIndex, e.MousePosition);
                }
            }
        }

        protected override void OnMouseButtonStateChange(PopWindowMouseEventArgs e)
        {
            if (currentMode == ShellModes.Transform)
            {
                (int pageIndex, Point pagePosition) = viewport.ToPagePosition(e.MousePosition);
                if (!transformer.HitAnyNode(pageIndex, pagePosition))
                {
                    IPageContent hitContent = inputHandler.HitContent(e.MousePosition);
                    if (transformer.TransformedContent != hitContent && e.FirstTimeButtons.HasLeft)
                    {
                        transformer.SetTransformedContent(hitContent, pageIndex);
                    }
                }

                if (!transformer.AnyNodeSelected && e.FirstTimeButtons.HasLeft)
                {
                    transformer.MouseHoldStart(pageIndex, pagePosition);
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

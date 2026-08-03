using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PopUpUI.Toolkit;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.Shell.ToolBar
{
    internal class ToolMenuRenderer
    {
        private readonly ToolMenuStyleProvider _styleProvider;
        private readonly ToolMenuLayoutCalculator _layoutCalculator;

        public ToolMenuRenderer(ToolMenuStyleProvider styleProvider, ToolMenuLayoutCalculator layoutCalculator)
        {
            _styleProvider = styleProvider;
            _layoutCalculator = layoutCalculator;
        }

        public void Render(IToolMenu menu, ToolButton hoveredButton, bool isHeld)
        {
            int currentRow = 0;
            int currentColumn = 0;
            ToolMenuStyle style = _styleProvider.Style;

            DrawTexture(style.Background, new Rectangle(_layoutCalculator.ScreenLocation, _layoutCalculator.BackgroundSize));
            DrawTexture(style.CloseButtonTexture, _layoutCalculator.GetCloseButtonLayout());

            if (style.MoveBoxTexture != null)
            {
                DrawTexture(style.MoveBoxTexture, _layoutCalculator.GetMoveBoxLayout());
            }

            foreach (var button in menu.Buttons)
            {
                Rectangle layout = _layoutCalculator.GetButtonLayout(currentRow, currentColumn);
                ToolButtonStyle buttonStyle = style.ButtonStyles[button.ID];

                if (!button.Enabled && buttonStyle.UnenabledTexture != null)
                {
                    DrawTexture(buttonStyle.UnenabledTexture, layout);
                }
                else
                {
                    DrawTexture(buttonStyle.DefaultTexture, layout);
                }

                if (button == hoveredButton)
                {
                    if (isHeld && style.ButtonsHeldOverlay != null)
                    {
                        DrawTexture(style.ButtonsHeldOverlay, layout);
                    }
                    else if (!isHeld && style.ButtonsHoverOverlay != null)
                    {
                        DrawTexture(style.ButtonsHoverOverlay, layout);
                    }
                }

                currentColumn++;
                if (currentColumn == style.ColumnCount)
                {
                    currentColumn = 0;
                    currentRow++;
                }
            }
        }

        private static void DrawTexture(Texture2D texture, Rectangle layout)
        {
            DrawHelper.Batch.Draw(texture, layout, Color.White);
        }
    }
}

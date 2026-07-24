using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PopUpUI.Toolkit;

namespace NoteKeeper.Shell.PropertyExplorer
{
    internal class PropertiesScreenRenderer
    {
        private readonly PropertyScreenStyleProvider _styleProvider;
        private readonly PropertiesLayoutCalculator _layoutCalculator;

        public PropertiesScreenRenderer(PropertyScreenStyleProvider styleProvider, PropertiesLayoutCalculator layoutCalculator)
        {
            _styleProvider = styleProvider;
            _layoutCalculator = layoutCalculator;
        }

        public void Render(IPropertyCollection collection, IPropertyEntry focusedEntry)
        {
            PropertyScreenStyle style = _styleProvider.Style;

            DrawHelper.Batch.Draw(style.ScreenBackground, _layoutCalculator.ScreenBounds, Color.White);

            int index = 0;
            foreach (var entry in collection.Properties)
            {
                PropertyRowLayout rowLayout = _layoutCalculator.GetRowLayout(entry, index);

                DrawBox(focusedEntry == entry ? style.FocusedBoxBackground : style.BoxBackground, style.Font, entry.Name, rowLayout.LabelBounds);
                DrawBox(style.BoxBackground, style.Font, entry.CurrentValueAsString, rowLayout.ValueBounds);

                // Dropdown box is rendered here, only if it is contained in focusedEntry
                // its options are drawn in RenderDropdown(), since only one Dropdown can be
                // open at a time
                if (focusedEntry == entry && entry.HasDropdownOptions)
                {
                    DrawHelper.Batch.Draw(style.DropdownButtonTexture, rowLayout.DropdownButtonBounds, Color.White);
                }

                index++;
            }
        }

        private static void DrawBox(Texture2D background, SpriteFont font, string boxValue, Rectangle boxBounds)
        {
            DrawHelper.Batch.Draw(background, boxBounds, Color.White);
            DrawHelper.Batch.DrawString(font, boxValue, boxBounds.Location.ToVector2(), Color.White);
        }

        public void RenderDropdown(OptionDropdown optionDropdown)
        {
            PropertyScreenStyle style = _styleProvider.Style;
            foreach ((string option, Rectangle bounds) in optionDropdown.OptionBounds)
            {
                DrawBox(style.DropdownOptionBackground, style.Font, option, bounds);
            }
        }

        public void RenderWithTextBox(IPropertyCollection collection, IPropertyEntry focusedEntry, ValueTextBox valueTextBox)
        {
            PropertyScreenStyle style = _styleProvider.Style;
            Rectangle textBoxBounds = valueTextBox.Bounds;

            DrawHelper.Batch.Draw(style.ScreenBackground, _layoutCalculator.ScreenBounds, Color.White);

            int index = 0;
            foreach (var entry in collection.Properties)
            {
                PropertyRowLayout rowLayout = _layoutCalculator.GetRowLayout(entry, index);

                DrawBox(focusedEntry == entry ? style.FocusedBoxBackground : style.BoxBackground, style.Font, entry.Name, rowLayout.LabelBounds);

                if (rowLayout.ValueBounds == textBoxBounds)
                {
                    DrawHelper.Batch.Draw(style.BoxBackground, rowLayout.ValueBounds, Color.White);
                    valueTextBox.Render();
                }
                else
                    DrawBox(style.BoxBackground, style.Font, entry.CurrentValueAsString, rowLayout.ValueBounds);

                // Dropdown box is rendered here, only if it is contained in focusedEntry
                // its options are drawn in RenderDropdown(), since only one Dropdown can be
                // open at a time
                if (focusedEntry == entry && entry.HasDropdownOptions)
                {
                    DrawHelper.Batch.Draw(style.DropdownButtonTexture, rowLayout.DropdownButtonBounds, Color.White);
                }

                index++;
            }
        }
    }
}

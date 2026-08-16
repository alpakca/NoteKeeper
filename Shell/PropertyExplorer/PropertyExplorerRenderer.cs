using Microsoft.Xna.Framework;
using NoteKeeper.Shell.Styling;
using PopUpUI.Toolkit;

namespace NoteKeeper.Shell.PropertyExplorer
{
    internal class PropertyExplorerRenderer
    {
        private readonly PropertyExplorerLayoutCalculator _layoutCalculator;

        public PropertyExplorerRenderer(PropertyExplorerLayoutCalculator layoutCalculator)
        {
            _layoutCalculator = layoutCalculator;
        }

        /// <summary>Renders given collection to a set render target.</summary>
        /// <param name="valueTextBox">
        /// ValueTextBox to render its current Text to its row. 
        /// If given null; value box at its row uses actual row value.
        /// </param>
        public void Render(IPropertyCollection collection, IPropertyEntry focusedEntry, ValueTextBox? valueTextBox)
        {
            ResolvedTheme theme = ThemeService.CurrentTheme;
            DrawHelper.DrawBorderedBackground(_layoutCalculator.ScreenBounds, 
                                              theme.GetColor(PropertyExplorerStyles.Border), theme.GetColor(PropertyExplorerStyles.Surface), 1);

            int index = 0;
            foreach (var entry in collection.Properties)
            {
                PropertyRowLayout rowLayout = _layoutCalculator.GetRowLayout(entry, index);

                DrawBox(isFocused: focusedEntry == entry, rowLayout.LabelBounds, entry.Name, theme); // label part

                // value part, if text box was placed in this row
                if (valueTextBox != null && rowLayout.ValueBounds == valueTextBox.Bounds)
                {
                    DrawHelper.DrawBackground(rowLayout.ValueBounds, theme.GetColor(PropertyExplorerStyles.BoxColor));
                    valueTextBox.Render();
                }
                else
                {
                    DrawBox(isFocused: false, rowLayout.ValueBounds, entry.CurrentValueAsString, theme);
                }

                // Dropdown box is rendered here, only if it is contained in focusedEntry
                // its options are drawn in RenderDropdown(), since only one Dropdown can be
                // open at a time
                if (focusedEntry == entry && entry.HasDropdownOptions)
                {
                    DrawHelper.DrawBorderedBackground(rowLayout.DropdownButtonBounds,
                                                      theme.GetColor(PropertyExplorerStyles.BorderAccent), theme.GetColor(PropertyExplorerStyles.Background), 1f);
                    DrawHelper.Batch.Draw(theme.GetIcon(PropertyExplorerStyles.DropdownBoxButton), rowLayout.DropdownButtonBounds, Color.White);
                }

                index++;
            }
        }

        public void RenderDropdownOptions(OptionDropdown optionDropdown)
        {
            ResolvedTheme theme = ThemeService.CurrentTheme;
            DrawHelper.DrawBorderedBackground(_layoutCalculator.GetDropdownMenuBounds(optionDropdown),
                                              theme.GetColor(PropertyExplorerStyles.Border), theme.GetColor(PropertyExplorerStyles.Background), 1);
            
            foreach ((string option, Rectangle bounds) in optionDropdown.OptionBounds)
            {
                FontProvider.DrawText(PropertyExplorerLayoutCalculator.TextSize, isBold: false, option,
                                      bounds.Location.ToVector2(), theme.GetColor(PropertyExplorerStyles.TextColor));
            }
        }

        private static void DrawBox(bool isFocused, Rectangle boxBounds, string boxValue, ResolvedTheme theme)
        {
            DrawHelper.DrawBackground(boxBounds, isFocused ? theme.GetColor(PropertyExplorerStyles.FocusedBoxColor)
                                                           : theme.GetColor(PropertyExplorerStyles.BoxColor));

            int yPadding = (boxBounds.Height - FontProvider.GetLineSpacing(PropertyExplorerLayoutCalculator.TextSize, false)) / 2;
            FontProvider.DrawText(PropertyExplorerLayoutCalculator.TextSize, isBold: false,
                                  boxValue, new Vector2(boxBounds.Location.X, boxBounds.Location.Y + yPadding),  theme.GetColor(PropertyExplorerStyles.TextColor));
        }
    }
}

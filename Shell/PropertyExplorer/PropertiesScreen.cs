using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PopUpUI.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.Shell.PropertyExplorer
{
    internal class PropertiesScreen
    {
        private readonly PanelWindow panelWindow;
        private readonly PropertiesLayoutCalculator layoutCalculator;
        private readonly PropertiesScreenRenderer renderer;

        private readonly ValueTextBox _valueTextBox;
        private readonly OptionDropdown _optionDropdown;

        private IPropertyCollection _shownCollection;
        private IPropertyEntry _focusedEntry;
        private ScreenState _state;

        public PropertyScreenStyleProvider StyleProvider { get; }

        private enum ScreenState
        {
            Idle,
            EditingTextBoxValue,
            PickingDropdownOption
        }

        public PropertiesScreen(PanelWindow window)
        {
            panelWindow = window;

            StyleProvider = new PropertyScreenStyleProvider();

            layoutCalculator = new PropertiesLayoutCalculator(StyleProvider);
            renderer = new PropertiesScreenRenderer(StyleProvider, layoutCalculator);

            _valueTextBox = new ValueTextBox();
            _optionDropdown = new OptionDropdown(layoutCalculator);
        }

        public void Show(IPropertyCollection collection, Rectangle screenBounds)
        {
            Hide();

            layoutCalculator.ScreenBounds = screenBounds;
            _shownCollection = collection;
        }

        public void Hide()
        {
            _focusedEntry = null;
            _state = ScreenState.Idle;
            _shownCollection = null;
        }

        public void Render()
        {
            if (_shownCollection == null)
                return;

            renderer.Render(_shownCollection, _focusedEntry);

            if (_state == ScreenState.EditingTextBoxValue)
            {
                renderer.RenderTextBox(_valueTextBox);
            }
            else if (_state == ScreenState.PickingDropdownOption)
            {
                renderer.RenderDropdown(_optionDropdown);
            }
        }

        public void OnMouseButtonStateChange(PopWindowMouseEventArgs e)
        {
            if (e.FirstTimeButtons.HasLeft && !layoutCalculator.ScreenBounds.Contains(e.MousePosition))
            {
                Hide();
            }

            int index = 0;
            IPropertyEntry newFocused = null;
            foreach (var entry in _shownCollection.Properties)
            {
                PropertyRowLayout rowLayout = layoutCalculator.GetRowLayout(entry, index);

                if (rowLayout.LabelBounds.Contains(e.MousePosition) || rowLayout.ValueBounds.Contains(e.MousePosition))
                {
                    newFocused = entry;
                    break;
                }

                index++;
            }

            if (newFocused == null)
            {
                _focusedEntry = newFocused;
                return;
            }

            if (_state == ScreenState.Idle)
            {
                PropertyRowLayout focusedLayout = layoutCalculator.GetRowLayout(newFocused, index);

                // can only select dropdown button if row was already focused
                if (newFocused == _focusedEntry && focusedLayout.DropdownButtonBounds.Contains(e.MousePosition))
                {
                    _state = ScreenState.PickingDropdownOption;
                }
                else if (focusedLayout.ValueBounds.Contains(e.MousePosition))
                {
                    _state = ScreenState.EditingTextBoxValue;

                    _valueTextBox.Bounds = focusedLayout.ValueBounds;
                    _valueTextBox.SetText(newFocused.CurrentValueAsString);
                    _valueTextBox.Selected(e.MousePosition);
                }
            }
            else if (_state == ScreenState.EditingTextBoxValue)
            {
                if (_valueTextBox.Bounds.Contains(e.MousePosition)) // selected a position inside textblock
                {
                    _valueTextBox.Selected(e.MousePosition);
                }
                else if (_focusedEntry.TryValidate(_valueTextBox.Text)) // outside selected
                {
                    _focusedEntry.SetValue(_valueTextBox.Text);
                    _state = ScreenState.Idle;

                    _valueTextBox.Bounds = Rectangle.Empty;
                }
            }
            else if (_state == ScreenState.PickingDropdownOption)
            {
                if (_optionDropdown.TrySelectOption(e.MousePosition, out string selectedOption))
                {
                    _focusedEntry.SetDropdownOptionValue(selectedOption);
                }

                // for all cases state will equal idle: Selected button again, selected outside, selected an option
                _state = ScreenState.Idle;
            }

            _focusedEntry = newFocused;
        }

        public void OnTextInsert(PopWindowMouseEventArgs e, char ch)
        {
            if (_state == ScreenState.EditingTextBoxValue)
            {
                _valueTextBox.InsertChar(ch);
            }
        }

        public void OnKeyDown(PopWindowMouseEventArgs e, Keys key)
        {
            if (_state == ScreenState.EditingTextBoxValue)
            {
                if (key == Keys.Enter && _focusedEntry.TryValidate(_valueTextBox.Text))
                {
                    _focusedEntry.SetValue(_valueTextBox.Text);
                    return;
                }

                _valueTextBox.KeyPressed(key);
            }
        }
    }
}

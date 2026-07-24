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
            _focusedEntry = null;
            _state = ScreenState.Idle;

            _shownCollection = collection;
        }

        public void Hide()
        {
        }

        public void Render()
        {
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
            // if (_optionDropdown.IsActive) _optionDropdown.TrySelectOption() etc
        }

        public void OnTextInsert(PopWindowMouseEventArgs e, char ch)
        {
            if (_state == ScreenState.EditingTextBoxValue && _valueTextBox.IsActive)
                _valueTextBox.InsertChar(ch);
        }

        public void OnKeyDown(PopWindowMouseEventArgs e, Keys key)
        {
            if (_state == ScreenState.EditingTextBoxValue && _valueTextBox.IsActive)
                _valueTextBox.KeyPressed(key);
        }
    }
}

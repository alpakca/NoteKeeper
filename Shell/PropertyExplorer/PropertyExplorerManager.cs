using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PopUpUI.Input;

namespace NoteKeeper.Shell.PropertyExplorer
{
    internal class PropertyExplorerManager
    {
        private readonly PropertyExplorerLayoutCalculator layoutCalculator;
        private readonly PropertyExplorerRenderer renderer;

        private readonly ValueTextBox _valueTextBox;
        private readonly OptionDropdown _optionDropdown;

        private IPropertyCollection _shownCollection;
        private IPropertyEntry _focusedEntry;
        private ScreenState _state;

        private enum ScreenState
        {
            Idle,
            EditingTextBoxValue,
            PickingDropdownOption
        }

        public Rectangle ScreenBounds
        {
            get => layoutCalculator.ScreenBounds;
            set => layoutCalculator.ScreenBounds = value;
        }

        public PropertyExplorerManager()
        {
            layoutCalculator = new PropertyExplorerLayoutCalculator();
            renderer = new PropertyExplorerRenderer(layoutCalculator);

            _valueTextBox = new ValueTextBox();
            _optionDropdown = new OptionDropdown(layoutCalculator);
        }

        public void Show(IPropertyCollection collection)
        {
            Hide();

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

            if (_state == ScreenState.Idle)
            {
                renderer.Render(_shownCollection, _focusedEntry, valueTextBox: null);
            }
            else if (_state == ScreenState.EditingTextBoxValue)
            {
                renderer.Render(_shownCollection, _focusedEntry, _valueTextBox);
            }
            else if (_state == ScreenState.PickingDropdownOption)
            {
                renderer.Render(_shownCollection, _focusedEntry, valueTextBox: null);
                renderer.RenderDropdownOptions(_optionDropdown);
            }
        }

        public void MouseButtonStateChange(PopWindowMouseEventArgs e)
        {
            if (_shownCollection == null || !e.FirstTimeButtons.HasLeft)
                return;

            if (!layoutCalculator.ScreenBounds.Contains(e.MousePosition))
            {
                Hide();
                return;
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

            PropertyRowLayout focusedLayout = layoutCalculator.GetRowLayout(newFocused, index);

            if ((_state == ScreenState.Idle || _state == ScreenState.EditingTextBoxValue) &&
                newFocused == _focusedEntry && focusedLayout.DropdownButtonBounds.Contains(e.MousePosition))
            {
                _state = ScreenState.PickingDropdownOption;
                _optionDropdown.Open(newFocused.GetDropdownOptions(), focusedLayout.DropdownButtonBounds);

                return;
            }

            if (_state == ScreenState.Idle && newFocused != null)
            {
                if (focusedLayout.ValueBounds.Contains(e.MousePosition))
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
                else // outside selected, try validate & set
                {
                    TrySetFocusedEntryValue();
                }
            }
            else if (_state == ScreenState.PickingDropdownOption)
            {
                if (_optionDropdown.TrySelectOption(e.MousePosition, out string selectedOption) && 
                    !_focusedEntry.IsCurrentDropdownOptionEqual(selectedOption))
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
            if (_shownCollection == null)
                return;

            if (_state == ScreenState.EditingTextBoxValue)
            {
                _valueTextBox.InsertChar(ch);
            }
        }

        public void OnKeyDown(PopWindowMouseEventArgs e, Keys key)
        {
            if (_shownCollection == null)
                return;

            if (_state == ScreenState.EditingTextBoxValue)
            {
                if (key == Keys.Enter && _focusedEntry != null)
                {
                    TrySetFocusedEntryValue();
                    return;
                }

                _valueTextBox.KeyPressed(key);
            }
        }

        private void TrySetFocusedEntryValue()
        {
            if (_focusedEntry.TryValidate(_valueTextBox.Text) &&
                !_focusedEntry.IsCurrentValueEqual(_valueTextBox.Text))
            {
                _focusedEntry.SetValue(_valueTextBox.Text);
            }

            _state = ScreenState.Idle;
            _valueTextBox.Bounds = Rectangle.Empty;
        }
    }
}

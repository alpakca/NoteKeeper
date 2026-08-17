using NoteKeeper.Shell.PropertyExplorer;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using NoteKeeper.InspectionWindow.MultiPage.Components;

namespace NoteKeeper.InspectionWindow.MultiPage.Properties
{
    internal class PageContentProperties : IPropertyCollection
    {
        private readonly PageActions _pageActions;

        private readonly IUnderlyingValueSetter[] _imageContentProperties;
        private readonly IUnderlyingValueSetter[] _textContentProperties;
        private readonly Dictionary<PropertyType, IUnderlyingValueSetter> _entries;

        private IPageContent _currentContent;

        public IPropertyEntry[] Properties { get; private set; }

        public PageContentProperties(PageActions pageActions)
        {
            _pageActions = pageActions;

            IntegerPropertyEntry x = new IntegerPropertyEntry("X", v =>
            {
                _pageActions.ChangeContentProperty(_currentContent.Location, new Point(v, _currentContent.Location.Y),
                                                   _currentContent, (content, value) => content.SetLocation(value));
            });
            IntegerPropertyEntry y = new IntegerPropertyEntry("Y", v =>
            {
                _pageActions.ChangeContentProperty(_currentContent.Location, new Point(_currentContent.Location.X, v),
                                                   _currentContent, (content, value) => content.SetLocation(value));
            });
            IntegerPropertyEntry width = new IntegerPropertyEntry("Width", v =>
            {
                _pageActions.ChangeContentProperty(_currentContent.Size, new Point(v, _currentContent.Size.Y),
                                                   _currentContent, (content, value) => content.SetSize(value));
            });
            IntegerPropertyEntry height = new IntegerPropertyEntry("Height", v =>
            {
                _pageActions.ChangeContentProperty(_currentContent.Size, new Point(_currentContent.Size.X, v),
                                                   _currentContent, (content, value) => content.SetSize(value));
            });

            IntegerDropdownPropertyEntry fontSize = new IntegerDropdownPropertyEntry("Font Size", [16, 24], size =>
            {
                PageTextContent textContent = (PageTextContent)_currentContent;
                _pageActions.ChangeContentProperty(textContent.FontSize, size, textContent,
                                                   (content, value) => content.SetFont(content.FontName, value));
            });
            StringDropdownPropertyEntry fontName = new StringDropdownPropertyEntry("Font Name", ["ComicSans"], name =>
            {
                PageTextContent textContent = (PageTextContent)_currentContent;
                _pageActions.ChangeContentProperty(textContent.FontName, name, textContent,
                                                   (content, value) => content.SetFont(value, content.FontSize));
            });
            StringPropertyEntry text = new StringPropertyEntry("Text", textValue =>
            {
                PageTextContent textContent = (PageTextContent)_currentContent;
                _pageActions.ChangeContentProperty(textContent.Text, textValue, textContent,
                                                   (content, value) => content.Text = value);
            });

            _entries = new Dictionary<PropertyType, IUnderlyingValueSetter>
            {
                { PropertyType.X, x }, { PropertyType.Y, y}, { PropertyType.Width, width }, { PropertyType.Height, height },
                { PropertyType.FontName, fontName }, { PropertyType.FontSize, fontSize }, { PropertyType.Text, text }
            };

            _imageContentProperties = [x, y, width, height];
            _textContentProperties = [x, y, width, height, fontName, fontSize];
        }

        public void SetContent(IPageContent content)
        {
            _entries[PropertyType.X].SetUnderlyingValue(content.Location.X);
            _entries[PropertyType.Y].SetUnderlyingValue(content.Location.Y);
            _entries[PropertyType.Width].SetUnderlyingValue(content.Size.X);
            _entries[PropertyType.Height].SetUnderlyingValue(content.Size.Y);

            if (content is PageImageContent)
            {
                Properties = _imageContentProperties;
            }
            else if (content is PageTextContent textContent)
            {
                _entries[PropertyType.FontName].SetUnderlyingValue(textContent.FontName);
                _entries[PropertyType.FontSize].SetUnderlyingValue(textContent.FontSize);
                _entries[PropertyType.Text].SetUnderlyingValue(textContent.Text);
                Properties = _textContentProperties;
            }
            else
            {
                throw new ArgumentException("Unhandled type of IPageContent for PageContentProperties.");
            }

            _currentContent = content;
        }
    }
}

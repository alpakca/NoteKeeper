using NoteKeeper.Shell.PropertyExplorer;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace NoteKeeper.InspectionWindow.MultiPage.Properties
{
    internal class PageContentProperties : IPropertyCollection
    {
        private readonly IUnderlyingValueSetter[] _imageContentProperties;
        private readonly IUnderlyingValueSetter[] _textContentProperties;
        private readonly Dictionary<PropertyType, IUnderlyingValueSetter> _entries;

        private IPageContent _currentContent;

        public IPropertyEntry[] Properties { get; private set; }

        public PageContentProperties()
        {
            IntegerPropertyEntry x = new IntegerPropertyEntry("X", v => _currentContent.SetLocation(new Point(v, _currentContent.Location.Y)));
            IntegerPropertyEntry y = new IntegerPropertyEntry("Y", v => _currentContent.SetLocation(new Point(_currentContent.Location.X, v)));
            IntegerPropertyEntry width = new IntegerPropertyEntry("Width", v => _currentContent.SetSize(new Point(v, _currentContent.Size.Y)));
            IntegerPropertyEntry height = new IntegerPropertyEntry("Height", v => _currentContent.SetSize(new Point(_currentContent.Size.X, v)));

            IntegerDropdownPropertyEntry fontSize = new IntegerDropdownPropertyEntry("Font Size", [], size =>
            {
                PageTextContent textContent = (PageTextContent)_currentContent;
                textContent.SetFont(textContent.FontName, size);
            });
            StringDropdownPropertyEntry fontName = new StringDropdownPropertyEntry("Font Name", [], name =>
            {
                PageTextContent textContent = (PageTextContent)_currentContent;
                textContent.SetFont(name, textContent.FontSize);
            });

            _entries = new Dictionary<PropertyType, IUnderlyingValueSetter>
            {
                { PropertyType.X, x }, { PropertyType.Y, y}, { PropertyType.Width, width }, { PropertyType.Height, height },
                { PropertyType.FontName, fontName }, { PropertyType.FontSize, fontSize }
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

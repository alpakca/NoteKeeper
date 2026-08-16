using NoteKeeper.Shell.Styling;
using System.Runtime.CompilerServices;

namespace NoteKeeper.Shell.PropertyExplorer
{
    internal static class PropertiesScreenStyles
    {
        public const string Background = "propertyexplorer.background";
        public const string Border = "propertyexplorer.border";
        public const string Surface = "propertyexplorer.surface";
        public const string BorderAccent = "propertyexplorer.borderaccent";
        public const string TextColor = "propertyexplorer.textcolor";
        public const string BoxColor = "propertyexplorer.boxcolor";
        public const string FocusedBoxColor = "propertiesexplorer.focusedboxcolor";

        public const string DropdownBoxButton = "propertyexplorer.dropdownboxbutton";

        [ModuleInitializer]
        public static void InitializeColors()
        {
            ColorRegistry.Register(new ColorDefinition
            {
                Id = Background,
                FallbackColorId = ColorRegistry.BackgroundId
            });

            ColorRegistry.Register(new ColorDefinition
            {
                Id = Border,
                FallbackColorId = ColorRegistry.BorderId
            });

            ColorRegistry.Register(new ColorDefinition
            {
                Id = Surface,
                FallbackColorId = ColorRegistry.SurfaceId
            });

            ColorRegistry.Register(new ColorDefinition
            {
                Id = BorderAccent,
                FallbackColorId = ColorRegistry.BorderAccentId
            });

            ColorRegistry.Register(new ColorDefinition
            {
                Id = TextColor,
                FallbackColorId = ColorRegistry.TextColorId
            });

            ColorRegistry.Register(new ColorDefinition
            {
                Id = BoxColor,
                FallbackColorId = ColorRegistry.BackgroundId
            });

            ColorRegistry.Register(new ColorDefinition
            {
                Id = FocusedBoxColor,
                FallbackColorId = ColorRegistry.BorderAccentId
            });

            IconRegistry.Register(new IconDefinition
            {
                Id = DropdownBoxButton,
                DefaultPath = @"shell\PropertyExplorerDropdownButton.png"
            });
        }
    }
}

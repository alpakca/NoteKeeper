using NoteKeeper.Shell.Styling;
using System.Runtime.CompilerServices;

namespace NoteKeeper.Shell.TabOrganizer
{
    internal static class TabsHeaderStyles
    {
        public const string Background = "tabsheader.background";
        public const string Surface = "tabsheader.surface";
        public const string Border = "tabsheader.border";
        public const string BorderAccent = "tabsheader.borderaccent";
        public const string HoverOverlay = "tabsheader.hoveroverlay";
        public const string HeldOverlay = "tabsheader.heldoverlay";

        public const string TextColor = "tabsheader.textcolor";

        public const string CloseButton = "tabsheader.closebutton";
        public const string TabListButton = "tabsheader.tablistbutton";

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
                Id = Surface,
                FallbackColorId = ColorRegistry.SurfaceId
            });

            ColorRegistry.Register(new ColorDefinition
            {
                Id = Border,
                FallbackColorId = ColorRegistry.BorderId
            });

            ColorRegistry.Register(new ColorDefinition
            {
                Id = BorderAccent,
                FallbackColorId = ColorRegistry.BorderAccentId
            });

            ColorRegistry.Register(new ColorDefinition
            {
                Id = HoverOverlay,
                FallbackColorId = ColorRegistry.HoverOverlayId
            });

            ColorRegistry.Register(new ColorDefinition
            {
                Id = HeldOverlay,
                FallbackColorId = ColorRegistry.HeldOverlayId
            });

            ColorRegistry.Register(new ColorDefinition
            {
                Id = TextColor,
                FallbackColorId = ColorRegistry.TextColorId
            });

            IconRegistry.Register(new IconDefinition
            {
                Id = CloseButton,
                DefaultPath = @"shell\CloseButton.png"
            });

            IconRegistry.Register(new IconDefinition
            {
                Id = TabListButton,
                DefaultPath = @"shell\TabsHeaderTabListButton.png"
            });
        }
    }
}

using NoteKeeper.Shell.Styling;
using System.Runtime.CompilerServices;

namespace NoteKeeper.Shell.TabOrganizer
{
    internal static class TabsHeaderStyles
    {
        public static string Background = "tabsheader.background";
        public static string Surface = "tabsheader.surface";
        public static string HoverOverlay = "tabsheader.hoveroverlay";
        public static string HeldOverlay = "tabsheader.heldoverlay";

        public static string TextColor = "tabsheader.textcolor";

        public static string CloseButton = "tabsheader.closebutton";
        public static string TabListButton = "tabsheader.tablistbutton";

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
                DefaultPath = "/shell/closebutton.png"
            });

            IconRegistry.Register(new IconDefinition
            {
                Id = TabListButton,
                DefaultPath = "/shell/tablistbutton.png"
            });
        }
    }
}

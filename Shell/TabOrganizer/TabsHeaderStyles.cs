using NoteKeeper.Shell.Theming;
using System.Runtime.CompilerServices;
using System.Text;

namespace NoteKeeper.Shell.TabOrganizer
{
    internal static class TabsHeaderStyles
    {
        public static string Background = "tabsheader.background";
        public static string Surface = "tabsheader.surface";
        public static string HoverOverlay = "tabsheader.hoveroverlay";
        public static string HeldOverlay = "tabsheader.heldoverlay";

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
                FallbackColorId = ColorRegistry.HoverOverlay
            });

            ColorRegistry.Register(new ColorDefinition
            {
                Id = HeldOverlay,
                FallbackColorId = ColorRegistry.HeldOverlay
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

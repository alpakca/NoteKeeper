using NoteKeeper.Shell.Styling;
using System.Runtime.CompilerServices;

namespace NoteKeeper.Shell.ToolBar
{
    internal static class ToolMenuStyles
    {
        public const string Background = "toolmenu.background";
        public const string Border = "toolmenu.border";
        public const string HoverOverlay = "toolmenu.hoveroverlay";
        public const string HeldOverlay = "toolmenu.heldoverlay";

        public const string CloseButton = "toolmenu.closebutton";

        [ModuleInitializer]
        public static void InitializeColors()
        {
            ColorRegistry.Register(new ColorDefinition
            {
                Id = Background,
                FallbackColorId = ColorRegistry.SurfaceId
            });

            ColorRegistry.Register(new ColorDefinition
            {
                Id = Border,
                FallbackColorId = ColorRegistry.BorderId
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

            IconRegistry.Register(new IconDefinition
            {
                Id = CloseButton,
                FallbackIconId = IconRegistry.CloseButtonId
            });
        }
    }
}

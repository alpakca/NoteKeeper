using NoteKeeper.Shell.Styling;

namespace NoteKeeper.Shell.ToolBar
{
    internal static class ToolMenuStyles
    {
        public const string Background = "toolmenu.background";
        public const string Border = "toolmenu.border";
        public const string HoverOverlay = "toolmenu.hoveroverlay";
        public const string HeldOverlay = "toolmenu.heldoverlay";

        public const string CloseButton = "toolmenu.closebutton";
        public const string MovementBox = "toolmenu.movementbox";

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

            IconRegistry.Register(new IconDefinition
            {
                Id = CloseButton,
                DefaultPath = @"shell\CloseButton.png"
            });

            IconRegistry.Register(new IconDefinition
            {
                Id = MovementBox,
                DefaultPath = @"shell\ToolMenuMovementBox.png"
            });
        }
    }
}

using NoteKeeper.Shell.Styling;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace NoteKeeper.Shell.ContextMenus
{
    internal static class ContextMenuStyles
    {
        public const string Border = "contextmenu.border";
        public const string Background = "contextmenu.background";
        public const string HoverOverlay = "contextmenu.hoveroverlay";
        public const string HeldOverlay = "contextmenu.heldoverlay";
        public const string TextColor = "contextmenu.textcolor";
        public const string GroupLine = "contextmenu.groupline";

        public const string SubMenuButtonRightIcon = "contextmenu.submenurighticon";

        [ModuleInitializer]
        public static void InitializeColors()
        {
            ColorRegistry.Register(new ColorDefinition
            {
                Id = Border,
                FallbackColorId = ColorRegistry.BorderAccentId
            });

            ColorRegistry.Register(new ColorDefinition
            {
                Id = Background,
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

            ColorRegistry.Register(new ColorDefinition
            {
                Id = GroupLine,
                FallbackColorId = ColorRegistry.SurfaceId
            });

            IconRegistry.Register(new IconDefinition
            {
                Id = SubMenuButtonRightIcon,
                DefaultPath = @"shell\ContextMenuSubMenuRightIcon.png"
            });
        }
    }
}

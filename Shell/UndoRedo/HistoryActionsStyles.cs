using NoteKeeper.Shell.Styling;
using System.Runtime.CompilerServices;

namespace NoteKeeper.Shell.UndoRedo
{
    internal static class HistoryActionsStyles
    {
        public const string Background = "historyactions.background";
        public const string Border = "historyactions.border";
        public const string HoverOverlay = "historyactions.hoveroverlay";
        public const string TextColor = "historyactions.textcolor";
        public const string SelectedActionIndicator = "historyactions.selectedactionindicator";

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
                FallbackColorId = ColorRegistry.BorderAccentId
            });

            ColorRegistry.Register(new ColorDefinition
            {
                Id = HoverOverlay,
                FallbackColorId = ColorRegistry.HoverOverlayId
            });

            ColorRegistry.Register(new ColorDefinition
            {
                Id = TextColor,
                FallbackColorId = ColorRegistry.TextColorId
            });

            ColorRegistry.Register(new ColorDefinition
            {
                Id = SelectedActionIndicator,
                FallbackColorId = ColorRegistry.BorderId
            });
        }
    }
}

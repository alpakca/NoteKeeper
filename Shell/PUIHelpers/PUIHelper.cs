using NoteKeeper.Shell.Styling;
using PopUpUI.PUI;
using System.IO;

namespace NoteKeeper.Shell.PUIHelpers
{
    internal static class PUIHelper
    {
        public static string MultiPageTabAssetsFolderPath = Path.Combine(ThemeService.AssetsFolderRelativePath, "multiPageTab\\");
        public static string ShellAssetsFolderPath = Path.Combine(ThemeService.AssetsFolderRelativePath, "shell\\");

        public static ITupleParser[] TupleParsers { get; } = [new AnimationTupleParser(), new TextureTupleParser(),
                                                              new RectangleTupleParser(), new PointTupleParser()];

        public static IPUIVariableCreator[] VariableCreators { get; } = [new AnimationVariableCreator(), new PathVariableCreator(), 
                                                                         new HoldablePacketVariableCreator(), new SelectablePacketVariableCreator()];

        public static VisualToggleButtonInstantiator VisualToggleButtonInstantiator { get; } = new VisualToggleButtonInstantiator();
        public static VisualToggleButtonAssigner VisualToggleButtonAssigner { get; } = new VisualToggleButtonAssigner();

        public static VisualClickableInstantiator VisualClickableInstantiator { get; } = new VisualClickableInstantiator();
        public static VisualClickableAssigner VisualClickableAssigner { get; } = new VisualClickableAssigner();

        public static ToolButtonAssigner ToolButtonAssigner { get; } = new ToolButtonAssigner();

        public static ScrollerInstantiator ScrollerInstantiator { get; } = new ScrollerInstantiator();

        public static PUIInterpreter GetInterpreter(string puiFilePath)
        {
            PUIInterpreter interpreter = new PUIInterpreter(puiFilePath);
            interpreter.AddVariableCreators(VariableCreators);
            interpreter.AddTupleParsers(TupleParsers);

            return interpreter;
        }
    }
}

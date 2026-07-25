using Microsoft.Xna.Framework;
using PopUpUI.PUI;
using PopUpUI.Surface;
using PopUpUI.Toolkit;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.Shell.PUIHelpers
{
    internal static class PUIHelper
    {
        public const string AssetsFolderPath = @"C:\Users\user\Desktop\C#\NoteKeeper\NoteKeeper\Assets\";
        public const string ShellAssetsFolderPath = AssetsFolderPath + @"Shell\";

        public static ITupleParser[] TupleParsers { get; } = [new AnimationTupleParser(), new RectangleTupleParser(), new PointTupleParser()];

        public static IPUIVariableCreator[] VariableCreators { get; } = [new AnimationVariableCreator(), new PathVariableCreator(), 
                                                                         new HoldablePacketVariableCreator(), new SelectablePacketVariableCreator()];

        public static VisualToggleButtonInstantiator VisualToggleButtonInstantiator { get; } = new VisualToggleButtonInstantiator();
        public static VisualClickableInstantiator VisualClickableInstantiator { get; } = new VisualClickableInstantiator();
        public static VisualClickableAssigner VisualClickableAssigner { get; } = new VisualClickableAssigner();

        public static PUIInterpreter GetInterpreter(string puiFilePath)
        {
            PUIInterpreter interpreter = new PUIInterpreter(puiFilePath);
            interpreter.AddVariableCreators(VariableCreators);
            interpreter.AddTupleParsers(TupleParsers);

            return interpreter;
        }
    }
}

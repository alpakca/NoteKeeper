using PopUpUI.PUI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.Shell.PUIHelpers
{
    internal class PathVariableCreator : IPUIVariableCreator
    {
        public string TypeName => "Path";

        public PUIVariable Get()
        {
            return new PathVariable();
        }
    }

    internal class PathVariable : PUIVariable
    {
        private string path;

        public override object Value
        {
            get => path;
            set => path = value as string;
        }
    }
}

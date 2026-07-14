using PopUpUI.Input;
using PopUpUI.PUI;
using System;

namespace NoteKeeper.Shell.PUIHelpers
{
    public class SelectablePacketVariableCreator : IPUIVariableCreator
    {
        public string TypeName => "SelectablePacket";

        public PUIVariable Get()
        {
            return new SelectablePacketVariable();
        }
    }

    public class SelectablePacketVariable : PUIBraceVariable
    {
        private readonly SelectablePacket packet;

        public override object Value
        {
            get => packet;
            set { }
        }

        public SelectablePacketVariable()
        {
            packet = new SelectablePacket();
        }

        public override void AssignProperty(string name, object value)
        {
            switch (name)
            {
                case "EnableOutsideSelecting":
                    packet.EnabledOutsideSelecting = (bool)value;
                    break;

                case "ButtonsToSelect":
                    packet.ButtonsToSelect = (MouseButtons)value;
                    break;

                default:
                    throw new VariableUnknownPropertyException(name);
            }
        }
    }
}

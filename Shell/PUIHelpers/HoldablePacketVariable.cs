using PopUpUI.Input;
using PopUpUI.PUI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.Shell.PUIHelpers
{
    internal class HoldablePacketVariableCreator : IPUIVariableCreator
    {
        public string TypeName => "HoldablePacket";

        public PUIVariable Get()
        {
            return new HoldablePacketVariable();
        }
    }

    internal class HoldablePacketVariable : PUIBraceVariable
    {
        private readonly HoldablePacket packet;

        public override object Value
        {
            get => packet;
            set { }
        }

        public HoldablePacketVariable()
        {
            packet = new HoldablePacket();
        }

        public override void AssignProperty(string name, object value)
        {
            switch (name)
            {
                case "ButtonsToHold":
                    packet.ButtonsToHold = (MouseButtons)value;
                    break;

                default:
                    throw new VariableUnknownPropertyException(name);
            }
        }
    }
}

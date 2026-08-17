using PopUpUI.Input;
using PopUpUI.PUI;
using PopUpUI.Surface;

namespace NoteKeeper.Shell.PUIHelpers
{
    internal class ScrollerInstantiator : IDescriptorInstantiator<VerticalScroller>, IDescriptorInstantiator<HorizontalScroller>
    {
        string IDescriptorInstantiator<VerticalScroller>.TypeName => "VerticalScroller";
        string IDescriptorInstantiator<HorizontalScroller>.TypeName => "HorizontalScroller";

        VerticalScroller IDescriptorInstantiator<VerticalScroller>.Instantiate(PUIDescriptor desc)
        {
            return new VerticalScroller
            {
                Name = desc.GetPropertyOrDefault<string>("Name", null),

                SelectablePacket = desc.GetPropertyOrDefault<SelectablePacket>("SelectablePacket", null),
                HoldPacket = desc.GetPropertyOrDefault<HoldablePacket>("HoldablePacket", null)
            };
        }

        HorizontalScroller IDescriptorInstantiator<HorizontalScroller>.Instantiate(PUIDescriptor desc)
        {
            return new HorizontalScroller
            {
                Name = desc.GetPropertyOrDefault<string>("Name", null),

                SelectablePacket = desc.GetPropertyOrDefault<SelectablePacket>("SelectablePacket", null),
                HoldPacket = desc.GetPropertyOrDefault<HoldablePacket>("HoldablePacket", null)
            };
        }
    }
}

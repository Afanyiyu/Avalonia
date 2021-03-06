using Avalonia.Automation.Platform;
using Avalonia.Controls;

#nullable enable

namespace Avalonia.Automation.Peers
{
    public class SliderAutomationPeer : RangeBaseAutomationPeer
    {
        public SliderAutomationPeer(
            IAutomationNodeFactory factory,
            Control owner,
            AutomationRole role = AutomationRole.Slider)
            : base(factory, owner, role) 
        {
        }
    }
}

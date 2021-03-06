using Avalonia.Controls.Automation.Platform;

#nullable enable

namespace Avalonia.Controls.Automation.Peers
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

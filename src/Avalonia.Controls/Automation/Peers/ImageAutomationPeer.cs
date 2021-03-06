using Avalonia.Automation.Platform;
using Avalonia.Controls;

#nullable enable

namespace Avalonia.Automation.Peers
{
    public class ImageAutomationPeer : ControlAutomationPeer
    {
        public ImageAutomationPeer(
            IAutomationNodeFactory factory,
            Control owner,
            AutomationRole role = AutomationRole.Image)
            : base(factory, owner, role)
        {
        }
    }
}

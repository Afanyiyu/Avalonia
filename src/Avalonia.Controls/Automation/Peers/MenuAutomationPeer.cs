using Avalonia.Automation.Platform;
using Avalonia.Controls;

#nullable enable

namespace Avalonia.Automation.Peers
{
    public class MenuAutomationPeer : ControlAutomationPeer
    {
        public MenuAutomationPeer(
            IAutomationNodeFactory factory,
            Control owner,
            AutomationRole role = AutomationRole.Menu)
            : base(factory, owner, role) 
        { 
        }
    }
}

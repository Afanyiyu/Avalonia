using Avalonia.Controls.Automation.Platform;

#nullable enable

namespace Avalonia.Controls.Automation.Peers
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

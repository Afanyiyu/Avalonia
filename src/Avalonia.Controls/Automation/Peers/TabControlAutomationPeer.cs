using Avalonia.Controls.Automation.Platform;

#nullable enable

namespace Avalonia.Controls.Automation.Peers
{
    public class TabControlAutomationPeer : SelectingItemsControlAutomationPeer
    {
        public TabControlAutomationPeer(
            IAutomationNodeFactory factory,
            Control owner,
            AutomationRole role = AutomationRole.TabControl)
            : base(factory, owner, role) 
        {
        }

        protected override string GetLocalizedControlTypeCore() => "tab control";
    }
}

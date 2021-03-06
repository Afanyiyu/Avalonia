using Avalonia.Controls.Automation.Platform;

#nullable enable

namespace Avalonia.Controls.Automation.Peers
{
    public class ButtonAutomationPeer : ContentControlAutomationPeer,
        IInvocableAutomationPeer
    {
        public ButtonAutomationPeer(
            IAutomationNodeFactory factory, 
            Control owner,
            AutomationRole role = AutomationRole.Button)
            : base(factory, owner, role) 
        {
        }
        
        public void Invoke()
        {
            EnsureEnabled();
            (Owner as Button)?.PerformClick();
        }
    }
}


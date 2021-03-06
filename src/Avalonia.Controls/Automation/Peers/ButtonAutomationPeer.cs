using Avalonia.Automation.Platform;
using Avalonia.Automation.Provider;
using Avalonia.Controls;

#nullable enable

namespace Avalonia.Automation.Peers
{
    public class ButtonAutomationPeer : ContentControlAutomationPeer,
        IInvokeProvider
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


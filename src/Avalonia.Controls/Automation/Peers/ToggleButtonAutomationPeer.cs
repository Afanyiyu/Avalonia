using Avalonia.Controls.Automation.Platform;
using Avalonia.Controls.Primitives;

#nullable enable

namespace Avalonia.Controls.Automation.Peers
{
    public class ToggleButtonAutomationPeer : ContentControlAutomationPeer, IToggleableAutomationPeer
    {
        public ToggleButtonAutomationPeer(
            IAutomationNodeFactory factory,
            Control owner,
            AutomationRole role = AutomationRole.Toggle)
            : base(factory, owner, role)
        {
        }

        bool? IToggleableAutomationPeer.GetToggleState() => Owner.GetValue(ToggleButton.IsCheckedProperty);

        void IToggleableAutomationPeer.Toggle()
        {
            EnsureEnabled();
            (Owner as ToggleButton)?.PerformClick();
        }

        protected override string GetLocalizedControlTypeCore() => "toggle button";
    }
}

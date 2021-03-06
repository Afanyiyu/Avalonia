using Avalonia.Automation.Platform;
using Avalonia.Automation.Provider;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

#nullable enable

namespace Avalonia.Automation.Peers
{
    public class ToggleButtonAutomationPeer : ContentControlAutomationPeer, IToggleProvider
    {
        public ToggleButtonAutomationPeer(
            IAutomationNodeFactory factory,
            Control owner,
            AutomationRole role = AutomationRole.Toggle)
            : base(factory, owner, role)
        {
        }

        ToggleState IToggleProvider.ToggleState
        {
            get => Owner.GetValue(ToggleButton.IsCheckedProperty) switch
            {
                true => ToggleState.On,
                false => ToggleState.Off,
                null => ToggleState.Indeterminate,
            };
        }

        void IToggleProvider.Toggle()
        {
            EnsureEnabled();
            (Owner as ToggleButton)?.PerformClick();
        }

        protected override string GetLocalizedControlTypeCore() => "toggle button";
    }
}

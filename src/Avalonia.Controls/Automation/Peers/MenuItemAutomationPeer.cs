using Avalonia.Automation.Platform;
using Avalonia.Controls;

#nullable enable

namespace Avalonia.Automation.Peers
{
    public class MenuItemAutomationPeer : ControlAutomationPeer
    {
        public MenuItemAutomationPeer(
            IAutomationNodeFactory factory,
            Control owner,
            AutomationRole role = AutomationRole.MenuItem)
            : base(factory, owner, role) 
        { 
        }

        protected override string GetLocalizedControlTypeCore() => "menu item";

        protected override string? GetNameCore()
        {
            var result = base.GetNameCore();

            if (result is null && Owner is MenuItem m && m.HeaderPresenter.Child is TextBlock text)
            {
                result = text.Text;
            }

            if (result is null)
            {
                result = Owner.GetValue(ContentControl.ContentProperty)?.ToString();
            }

            return result;
        }
    }
}

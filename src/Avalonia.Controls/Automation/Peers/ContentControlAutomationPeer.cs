using Avalonia.Automation.Platform;
using Avalonia.Controls;

#nullable enable

namespace Avalonia.Automation.Peers
{
    public class ContentControlAutomationPeer : ControlAutomationPeer
    {
        protected ContentControlAutomationPeer(
            IAutomationNodeFactory factory,
            Control owner,
            AutomationRole role)
            : base(factory, owner, role) 
        { 
        }

        protected override string? GetNameCore()
        {
            var result = base.GetNameCore();

            if (result is null && Owner is ContentControl cc && cc.Presenter?.Child is TextBlock text)
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

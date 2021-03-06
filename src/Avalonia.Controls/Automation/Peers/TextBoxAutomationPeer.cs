using Avalonia.Controls.Automation.Platform;

#nullable enable

namespace Avalonia.Controls.Automation.Peers
{
    public class TextBoxAutomationPeer : TextAutomationPeer, IStringValueAutomationPeer
    {
        public TextBoxAutomationPeer(
            IAutomationNodeFactory factory,
            Control owner,
            AutomationRole role = AutomationRole.Edit)
            : base(factory, owner, role)
        {
        }

        string IStringValueAutomationPeer.GetValue() => Owner.GetValue(TextBlock.TextProperty);
        void IStringValueAutomationPeer.SetValue(string? value) => Owner.SetValue(TextBlock.TextProperty, value);

        protected override string GetLocalizedControlTypeCore() => "text box";
    }
}

using Avalonia.Automation.Platform;
using Avalonia.Automation.Provider;
using Avalonia.Controls;

#nullable enable

namespace Avalonia.Automation.Peers
{
    public class TextBoxAutomationPeer : TextAutomationPeer, IValueProvider
    {
        public TextBoxAutomationPeer(
            IAutomationNodeFactory factory,
            Control owner,
            AutomationRole role = AutomationRole.Edit)
            : base(factory, owner, role)
        {
        }

        bool IValueProvider.IsReadOnly => false;
        string? IValueProvider.Value => Owner.GetValue(TextBlock.TextProperty);
        void IValueProvider.SetValue(string? value) => Owner.SetValue(TextBlock.TextProperty, value);

        protected override string GetLocalizedControlTypeCore() => "text box";
    }
}

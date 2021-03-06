using Avalonia.Controls.Automation.Peers;
using Avalonia.Win32.Interop.Automation;

#nullable enable

namespace Avalonia.Win32.Automation
{
    internal partial class AutomationNode : IValueProvider
    {
        private string? _value;

        string? IValueProvider.Value => _value;
        bool IValueProvider.IsReadOnly => false;
        void IValueProvider.SetValue(string? value) => InvokeSync<IStringValueAutomationPeer>(x => x.SetValue(value));

        private void UpdateValue()
        {
            if (Peer is IStringValueAutomationPeer peer)
            {
                UpdateProperty(UiaPropertyId.ValueValue, ref _value, peer.GetValue());
            }
        }
    }
}

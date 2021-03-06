using Avalonia.Win32.Interop.Automation;
using AAP = Avalonia.Automation.Provider;

#nullable enable

namespace Avalonia.Win32.Automation
{
    internal partial class AutomationNode : IValueProvider
    {
        private string? _value;

        string? IValueProvider.Value => _value;
        bool IValueProvider.IsReadOnly => false;
        void IValueProvider.SetValue(string? value) => InvokeSync<AAP.IValueProvider>(x => x.SetValue(value));

        private void UpdateValue()
        {
            if (Peer is AAP.IValueProvider peer)
            {
                UpdateProperty(UiaPropertyId.ValueValue, ref _value, peer.Value);
            }
        }
    }
}

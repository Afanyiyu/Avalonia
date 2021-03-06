using Avalonia.Win32.Interop.Automation;
using AAP = Avalonia.Automation.Provider;

#nullable enable

namespace Avalonia.Win32.Automation
{
    internal partial class AutomationNode : IToggleProvider
    {
        private AAP.ToggleState _toggleState;

        AAP.ToggleState IToggleProvider.ToggleState => _toggleState;
        void IToggleProvider.Toggle() => InvokeSync<AAP.IToggleProvider>(x => x.Toggle());

        private void UpdateToggle()
        {
            if (Peer is AAP.IToggleProvider peer)
            {
                UpdateProperty(
                    UiaPropertyId.ToggleToggleState,
                    ref _toggleState,
                    peer.ToggleState);
            }
        }
    }
}

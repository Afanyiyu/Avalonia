using Avalonia.Automation;
using Avalonia.Win32.Interop.Automation;
using AAP = Avalonia.Automation.Provider;

#nullable enable

namespace Avalonia.Win32.Automation
{
    internal partial class AutomationNode : IExpandCollapseProvider
    {
        private ExpandCollapseState _expandCollapseState;

        ExpandCollapseState IExpandCollapseProvider.ExpandCollapseState => _expandCollapseState;
        void IExpandCollapseProvider.Expand() => InvokeSync((AAP.IExpandCollapseProvider x) => x.Expand());
        void IExpandCollapseProvider.Collapse() => InvokeSync((AAP.IExpandCollapseProvider x) => x.Collapse());

        private void UpdateExpandCollapse()
        {
            if (Peer is AAP.IExpandCollapseProvider peer)
            {
                UpdateProperty(
                    UiaPropertyId.ExpandCollapseExpandCollapseState,
                    ref _expandCollapseState,
                    peer.ExpandCollapseState);
            }
        }
    }
}

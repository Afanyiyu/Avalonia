using System;
using System.Linq;
using Avalonia.Win32.Interop.Automation;
using AAP = Avalonia.Automation.Provider;

#nullable enable

namespace Avalonia.Win32.Automation
{
    internal partial class AutomationNode : ISelectionProvider, ISelectionItemProvider
    {
        private bool _canSelectMultiple;
        private bool _isSelectionRequired;
        private bool _isSelected;
        private IRawElementProviderSimple[]? _selection;

        bool ISelectionProvider.CanSelectMultiple => _canSelectMultiple;
        bool ISelectionProvider.IsSelectionRequired => _isSelectionRequired;
        bool ISelectionItemProvider.IsSelected => _isSelected;
        IRawElementProviderSimple? ISelectionItemProvider.SelectionContainer => null;

        IRawElementProviderSimple[] ISelectionProvider.GetSelection() => _selection ?? Array.Empty<IRawElementProviderSimple>();
        void ISelectionItemProvider.Select() => InvokeSync<AAP.ISelectionItemProvider>(x => x.Select());
        void ISelectionItemProvider.AddToSelection() => InvokeSync<AAP.ISelectionItemProvider>(x => x.AddToSelection());
        void ISelectionItemProvider.RemoveFromSelection() => InvokeSync<AAP.ISelectionItemProvider>(x => x.RemoveFromSelection());

        private void UpdateSelection()
        {
            if (Peer is AAP.ISelectionProvider selectionPeer)
            {
                var selection = selectionPeer.GetSelection();

                UpdateProperty(
                    UiaPropertyId.SelectionCanSelectMultiple,
                    ref _canSelectMultiple,
                    selectionPeer.CanSelectMultiple);
                UpdateProperty(
                    UiaPropertyId.SelectionIsSelectionRequired,
                    ref _isSelectionRequired,
                    selectionPeer.IsSelectionRequired);
                UpdateProperty(
                    UiaPropertyId.SelectionSelection,
                    ref _selection,
                    selection.Select(x => (IRawElementProviderSimple)x.Node!).ToArray());
            }

            if (Peer is AAP.ISelectionItemProvider selectablePeer)
            {
                UpdateProperty(
                    UiaPropertyId.SelectionItemIsSelected,
                    ref _isSelected,
                    selectablePeer.IsSelected);
            }
        }
    }
}

using Avalonia.Automation.Platform;
using Avalonia.Automation.Provider;
using Avalonia.Controls;

#nullable enable

namespace Avalonia.Automation.Peers
{
    public class ItemsControlAutomationPeer : ControlAutomationPeer, IScrollProvider
    {
        private bool _searchedForScrollable;
        private IScrollProvider? _scroller;

        public ItemsControlAutomationPeer(
            IAutomationNodeFactory factory,
            Control owner,
            AutomationRole role = AutomationRole.List)
            : base(factory, owner, role)
        {
        }

        protected virtual IScrollProvider? Scroller
        {
            get
            {
                if (!_searchedForScrollable)
                {
                    if (Owner.GetValue(ListBox.ScrollProperty) is Control scrollable)
                        _scroller = GetOrCreatePeer(scrollable) as IScrollProvider;
                    _searchedForScrollable = true;
                }

                return _scroller;
            }
        }

        Size IScrollProvider.GetExtent() => Scroller?.GetExtent() ?? default;
        Size IScrollProvider.GetViewport() => Scroller?.GetViewport() ?? default;
        Vector IScrollProvider.GetOffset() => Scroller?.GetOffset() ?? default;
        void IScrollProvider.SetOffset(Vector value) => Scroller?.SetOffset(value);
        void IScrollProvider.LineDown() => Scroller?.LineDown();
        void IScrollProvider.LineLeft() => Scroller?.LineLeft();
        void IScrollProvider.LineRight() => Scroller?.LineRight();
        void IScrollProvider.LineUp() => Scroller?.LineUp();
        void IScrollProvider.PageDown() => Scroller?.PageDown();
        void IScrollProvider.PageLeft() => Scroller?.PageLeft();
        void IScrollProvider.PageRight() => Scroller?.PageRight();
        void IScrollProvider.PageUp() => Scroller?.PageUp();
    }
}

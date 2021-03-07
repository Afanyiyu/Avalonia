using System.Collections.Generic;
using Avalonia.Automation.Platform;
using Avalonia.Automation.Provider;
using Avalonia.Controls;

#nullable enable

namespace Avalonia.Automation.Peers
{
    public class ComboBoxAutomationPeer : SelectingItemsControlAutomationPeer,
        IExpandCollapseProvider
    {
        public ComboBoxAutomationPeer(
            IAutomationNodeFactory factory,
            Control owner,
            AutomationRole role = AutomationRole.ComboBox)
            : base(factory, owner, role) 
        {
        }

        public ExpandCollapseState ExpandCollapseState
        {
            get => State(Owner.GetValue(ComboBox.IsDropDownOpenProperty));
        }

        public void Collapse() => Owner.SetValue(ComboBox.IsDropDownOpenProperty, false);
        public void Expand() => Owner.SetValue(ComboBox.IsDropDownOpenProperty, true);

        protected override IReadOnlyList<AutomationPeer>? GetSelectionCore()
        {
            if (ExpandCollapseState == ExpandCollapseState.Expanded)
                return base.GetSelectionCore();

            // If the combo box is not open then we won't have an ItemsPresenter so the default
            // GetSelectionCore implementation won't work.
            if (Owner is ComboBox owner &&
                Owner.GetValue(ComboBox.SelectedItemProperty) is object selection)
            {
                var peer = new SurrogateItemPeer(Node.Factory, owner, selection);
                return new[] { peer };
            }

            return null;
        }

        protected override string GetLocalizedControlTypeCore() => "combo box";

        protected override void OwnerPropertyChanged(object sender, AvaloniaPropertyChangedEventArgs e)
        {
            base.OwnerPropertyChanged(sender, e);

            if (e.Property == ComboBox.IsDropDownOpenProperty)
            {
                RaisePropertyChangedEvent(
                    ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty,
                    State((bool)e.OldValue!),
                    State((bool)e.NewValue!));
            }
        }

        private ExpandCollapseState State(bool value)
        {
            return value ? ExpandCollapseState.Expanded : ExpandCollapseState.Collapsed;
        }

        private class SurrogateItemPeer : ListItemAutomationPeer
        {
            private readonly object _item;

            public SurrogateItemPeer(IAutomationNodeFactory factory, ComboBox owner, object item)
                : base(factory, owner, AutomationRole.ListItem)
            {
                _item = item;
            }

            protected override string? GetNameCore()
            {
                if (_item is Control c)
                {
                    var result = AutomationProperties.GetName(c);

                    if (result is null && c is ContentControl cc && cc.Presenter?.Child is TextBlock text)
                    {
                        result = text.Text;
                    }

                    if (result is null)
                    {
                        result = c.GetValue(ContentControl.ContentProperty)?.ToString();
                    }

                    return result;
                }

                return _item.ToString();
            }
        }
    }
}

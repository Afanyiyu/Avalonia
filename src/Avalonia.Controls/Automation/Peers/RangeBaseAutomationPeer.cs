using Avalonia.Automation.Platform;
using Avalonia.Automation.Provider;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

#nullable enable

namespace Avalonia.Automation.Peers
{
    public class RangeBaseAutomationPeer : ControlAutomationPeer, IRangeValueProvider
    {
        public RangeBaseAutomationPeer(
            IAutomationNodeFactory factory,
            Control owner,
            AutomationRole role)
            : base(factory, owner, role) 
        {
            owner.PropertyChanged += OwnerPropertyChanged;
        }

        public virtual bool IsReadOnly => false;
        public double Maximum => Owner.GetValue(RangeBase.MaximumProperty);
        public double Minimum => Owner.GetValue(RangeBase.MinimumProperty);
        public double Value => Owner.GetValue(RangeBase.ValueProperty);
        public void SetValue(double value) => Owner.SetValue(RangeBase.ValueProperty, value);

        protected virtual void OwnerPropertyChanged(object sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.Property == RangeBase.MinimumProperty)
                RaisePropertyChangedEvent(RangeValuePatternIdentifiers.MinimumProperty, e.OldValue, e.NewValue);
            else if (e.Property == RangeBase.MaximumProperty)
                RaisePropertyChangedEvent(RangeValuePatternIdentifiers.MaximumProperty, e.OldValue, e.NewValue);
            else if (e.Property == RangeBase.ValueProperty)
                RaisePropertyChangedEvent(RangeValuePatternIdentifiers.ValueProperty, e.OldValue, e.NewValue);
        }
    }
}

using Avalonia.Win32.Interop.Automation;
using AAP = Avalonia.Automation.Provider;

#nullable enable

namespace Avalonia.Win32.Automation
{
    internal partial class AutomationNode : IRangeValueProvider
    {
        private double _rangeValue;
        private double _rangeMinimum;
        private double _rangeMaximum;

        double IRangeValueProvider.Value => _rangeValue;
        bool IRangeValueProvider.IsReadOnly => false;
        double IRangeValueProvider.Maximum => _rangeMaximum;
        double IRangeValueProvider.Minimum => _rangeMinimum;
        double IRangeValueProvider.LargeChange => 1;
        double IRangeValueProvider.SmallChange => 1;

        void IRangeValueProvider.SetValue(double value) => InvokeSync((AAP.IRangeValueProvider x) => x.SetValue(value));

        private void UpdateRangeValue()
        {
            if (Peer is AAP.IRangeValueProvider peer)
            {
                UpdateProperty(UiaPropertyId.RangeValueValue, ref _rangeValue, peer.Value);
                UpdateProperty(UiaPropertyId.RangeValueMinimum, ref _rangeMinimum, peer.Minimum);
                UpdateProperty(UiaPropertyId.RangeValueMaximum, ref _rangeMaximum, peer.Maximum);
            }
        }
    }
}

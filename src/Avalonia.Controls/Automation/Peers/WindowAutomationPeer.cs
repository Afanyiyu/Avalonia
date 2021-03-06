using System;
using Avalonia.Automation.Platform;
using Avalonia.Controls;

#nullable enable

namespace Avalonia.Automation.Peers
{
    public class WindowAutomationPeer : WindowBaseAutomationPeer
    {
        public WindowAutomationPeer(
            IAutomationNodeFactory factory,
            Window owner)
            : base(factory, owner)
        {
            if (owner.IsVisible)
                StartTrackingFocus();
            else
                owner.Opened += OnOpened;
            owner.Closed += OnClosed;
        }

        private void OnOpened(object sender, EventArgs e)
        {
            ((Window)Owner).Opened -= OnOpened;
            StartTrackingFocus();
        }

        private void OnClosed(object sender, EventArgs e)
        {
            ((Window)Owner).Closed -= OnClosed;
            StopTrackingFocus();
        }
    }
}



using Avalonia.Platform;

#nullable enable

namespace Avalonia.Controls.Automation.Peers
{
    public interface IRootAutomationPeer
    {
        ITopLevelImpl? PlatformImpl { get; }
        AutomationPeer? GetFocus();
        AutomationPeer? GetPeerFromPoint(Point p);
    }
}

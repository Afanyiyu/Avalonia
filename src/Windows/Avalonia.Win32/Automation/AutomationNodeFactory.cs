using Avalonia.Controls.Automation.Peers;
using Avalonia.Controls.Automation.Platform;
using Avalonia.Threading;

#nullable enable

namespace Avalonia.Win32.Automation
{
    internal class AutomationNodeFactory : IAutomationNodeFactory
    {
        public static readonly AutomationNodeFactory Instance = new AutomationNodeFactory();

        public IAutomationNode CreateNode(AutomationPeer peer)
        {
            Dispatcher.UIThread.VerifyAccess();

            return peer switch
            {
                IRootAutomationPeer => new RootAutomationNode(peer),
                _ => new AutomationNode(peer),
            };
        }
    }
}

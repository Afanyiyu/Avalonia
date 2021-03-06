using Avalonia.Controls.Automation.Peers;

#nullable enable

namespace Avalonia.Controls.Automation.Platform
{
    /// <summary>
    /// Represents a platform implementation of a root node in the UI Automation tree.
    /// </summary>
    public interface IRootAutomationNode : IAutomationNode
    {
        /// <summary>
        /// Called by the <see cref="IRootAutomationPeer"/> when its focus changes.
        /// </summary>
        /// <param name="focus">
        /// The automation peer for the newly focused control or null if no control is focused.
        /// </param>
        void FocusChanged(AutomationPeer? focus);
    }
}

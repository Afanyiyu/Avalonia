using System;
using Avalonia.Controls.Automation.Peers;

namespace Avalonia.Controls.Automation.Platform
{
    /// <summary>
    /// Represents a platform implementation of a node in the UI Automation tree.
    /// </summary>
    public interface IAutomationNode : IDisposable
    {
        /// <summary>
        /// Gets a factory which can be used to create child nodes.
        /// </summary>
        IAutomationNodeFactory Factory { get; }

        /// <summary>
        /// Called by the <see cref="AutomationPeer"/> when the children of the peer change.
        /// </summary>
        void ChildrenChanged();

        /// <summary>
        /// Called by the <see cref="AutomationPeer"/> when the parent of the peer changes.
        /// </summary>
        void ParentChanged();

        /// <summary>
        /// Called by the <see cref="AutomationPeer"/> when a property other than the parent or
        /// children changes.
        /// </summary>
        void PropertyChanged();

        /// <summary>
        /// Called by the <see cref="AutomationPeer"/> when the root of the peer changes.
        /// </summary>
        void RootChanged();
    }
}

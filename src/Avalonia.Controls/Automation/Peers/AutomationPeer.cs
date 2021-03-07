using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Automation.Platform;
using Avalonia.Automation.Provider;

#nullable enable

namespace Avalonia.Automation.Peers
{
    /// <summary>
    /// Provides a base class that exposes an element to UI Automation.
    /// </summary>
    public abstract class AutomationPeer
    {
        private IReadOnlyList<AutomationPeer>? _children;
        private bool _childrenValid;
        private AutomationPeer? _parent;
        private bool _parentValid;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutomationPeer"/> class.
        /// </summary>
        /// <param name="factory">
        /// The factory to use to create the platform automation node.
        /// </param>
        protected AutomationPeer(IAutomationNodeFactory factory)
        {
            Node = factory.CreateNode(this);
        }

        /// <summary>
        /// Gets the related node in the platform UI Automation tree.
        /// </summary>
        public IAutomationNode Node { get; }

        /// <summary>
        /// Attempts to bring the element associated with the automation peer into view.
        /// </summary>
        public void BringIntoView() => BringIntoViewCore();

        /// <summary>
        /// Gets the bounding rectangle of the element that is associated with the automation peer
        /// in top-level coordinates.
        /// </summary>
        public Rect GetBoundingRectangle() => GetBoundingRectangleCore();

        /// <summary>
        /// Gets the child automation peers.
        /// </summary>
        public IReadOnlyList<AutomationPeer> GetChildren() => GetOrCreateChildren();

        /// <summary>
        /// Gets a string that describes the class of the element.
        /// </summary>
        public string GetClassName() => GetClassNameCore() ?? string.Empty;

        /// <summary>
        /// Gets a human-readable localized string that represents the type of the control that is
        /// associated with this automation peer.
        /// </summary>
        public string GetLocalizedControlType() => GetLocalizedControlTypeCore();

        /// <summary>
        /// Gets text that describes the element that is associated with this automation peer.
        /// </summary>
        public string GetName() => GetNameCore() ?? string.Empty;

        /// <summary>
        /// Gets the <see cref="AutomationPeer"/> that is the parent of this <see cref="AutomationPeer"/>.
        /// </summary>
        /// <returns></returns>
        public AutomationPeer? GetParent()
        {
            EnsureConnected();
            return _parent;
        }

        /// <summary>
        /// Gets the role of the element that is associated with this automation peer.
        /// </summary>
        public AutomationRole GetRole() => GetRoleCore();

        /// <summary>
        /// Gets a value that indicates whether the element that is associated with this automation
        /// peer currently has keyboard focus.
        /// </summary>
        public bool HasKeyboardFocus() => HasKeyboardFocusCore();

        /// <summary>
        /// Gets a value that indicates whether the element is understood by the user as
        /// interactive or as contributing to the logical structure of the control in the GUI.
        /// </summary>
        public bool IsControlElement() => IsControlElementCore();

        /// <summary>
        /// Gets a value indicating whether the control is enabled for user interaction.
        /// </summary>
        public bool IsEnabled() => IsEnabledCore();

        /// <summary>
        /// Gets a value that indicates whether the element can accept keyboard focus.
        /// </summary>
        /// <returns></returns>
        public bool IsKeyboardFocusable() => IsKeyboardFocusableCore();

        /// <summary>
        /// Sets the keyboard focus on the element that is associated with this automation peer.
        /// </summary>
        public void SetFocus() => SetFocusCore();

        /// <summary>
        /// Shows the context menu for the element that is associated with this automation peer.
        /// </summary>
        /// <returns>true if a context menu is present for the element; otherwise false.</returns>
        public bool ShowContextMenu() => ShowContextMenuCore();

        /// <summary>
        /// Invalidates the peer's children and causes a re-read from <see cref="GetChildrenCore"/>.
        /// </summary>
        public void InvalidateChildren()
        {
            _childrenValid = false;
            Node!.ChildrenChanged();
        }

        /// <summary>
        /// Invalidates the peer's parent.
        /// </summary>
        public void InvalidateParent()
        {
            _parent = null;
            _parentValid = false;
        }

        /// <summary>
        /// Raises an event to notify the automation client of a changed property value.
        /// </summary>
        /// <param name="automationProperty">The property that changed.</param>
        /// <param name="oldValue">The previous value of the property.</param>
        /// <param name="newValue">The new value of the property.</param>
        public void RaisePropertyChangedEvent(
            AutomationProperty automationProperty,
            object? oldValue,
            object? newValue)
        {
            Node.PropertyChanged(automationProperty, oldValue, newValue);
        }

        protected abstract void BringIntoViewCore();
        protected abstract IAutomationNode CreatePlatformImplCore(IAutomationNodeFactory factory);
        protected abstract Rect GetBoundingRectangleCore();
        protected abstract IReadOnlyList<AutomationPeer>? GetChildrenCore();
        protected abstract string GetClassNameCore();
        protected abstract string GetLocalizedControlTypeCore();
        protected abstract string? GetNameCore();
        protected abstract AutomationRole GetRoleCore();
        protected abstract bool HasKeyboardFocusCore();
        protected abstract bool IsControlElementCore();
        protected abstract bool IsEnabledCore();
        protected abstract bool IsKeyboardFocusableCore();
        protected abstract void SetFocusCore();
        protected abstract bool ShowContextMenuCore();

        /// <summary>
        /// When overriden in a derived class, tries to ensure that the peer is connected to a tree.
        /// </summary>
        /// <remarks>
        /// A peer's parent is set when it's added to another peer's children collection, so for the
        /// parent to be valid the ancestor tree must be fully constructed. In the case where a peer
        /// is created before some of its ancestors this method is called to construct the ancestor
        /// tree.
        /// </remarks>
        protected abstract void TryConnectToTree();

        protected void EnsureEnabled()
        {
            if (!IsEnabled())
                throw new ElementNotEnabledException();
        }

        private void EnsureConnected()
        {
            if (!_parentValid)
            {
                TryConnectToTree();
                _parentValid = true;
            }
        }

        private IReadOnlyList<AutomationPeer> GetOrCreateChildren()
        {
            var children = _children ?? Array.Empty<AutomationPeer>();

            if (_childrenValid)
                return children;

            var newChildren = GetChildrenCore() ?? Array.Empty<AutomationPeer>();

            foreach (var peer in children.Except(newChildren))
                peer.SetParent(null);
            foreach (var peer in newChildren)
                peer.SetParent(this);

            _childrenValid = true;
            return _children = newChildren;
        }

        private void SetParent(AutomationPeer? parent)
        {
            _parent = parent;
        }
    }
}

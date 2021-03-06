using System;
using System.Collections.Generic;
using Avalonia.Controls.Automation.Platform;
using Avalonia.VisualTree;

#nullable enable

namespace Avalonia.Controls.Automation.Peers
{
    /// <summary>
    /// An automation peer which represents a <see cref="Control"/> element.
    /// </summary>
    public class ControlAutomationPeer : AutomationPeer
    {
        private readonly AutomationRole _role;

        public ControlAutomationPeer(IAutomationNodeFactory factory, Control owner, AutomationRole role)
            : base(factory)
        {
            Owner = owner ?? throw new ArgumentNullException("owner");

            _role = role;

            owner.PropertyChanged += OwnerPropertyChanged;
            
            var visualChildren = ((IVisual)owner).VisualChildren;
            visualChildren.CollectionChanged += VisualChildrenChanged;
        }

        public Control Owner { get; }

        public static AutomationPeer GetOrCreatePeer(IAutomationNodeFactory factory, Control element)
        {
            element = element ?? throw new ArgumentNullException("element");
            return element.GetOrCreateAutomationPeer(factory);
        }

        public AutomationPeer GetOrCreatePeer(Control element)
        {
            return element == Owner ? this : GetOrCreatePeer(Node.Factory, element);
        }

        protected override void BringIntoViewCore() => Owner.BringIntoView();

        protected override IAutomationNode CreatePlatformImplCore(IAutomationNodeFactory factory)
        {
            return factory.CreateNode(this);
        }

        protected override void TryConnectToTree()
        {
            var parent = Owner.GetVisualParent();

            while (parent is object)
            {
                if (parent is Control c)
                {
                    var parentPeer = GetOrCreatePeer(c);
                    parentPeer.GetChildren();
                }

                parent = parent.GetVisualParent();
            }
        }

        protected override Rect GetBoundingRectangleCore()
        {
            var root = Owner.GetVisualRoot();

            if (root is null)
                return Rect.Empty;

            var t = Owner.TransformToVisual(root);

            if (!t.HasValue)
                return Rect.Empty;

            return new Rect(Owner.Bounds.Size).TransformToAABB(t.Value);
        }

        protected override IReadOnlyList<AutomationPeer>? GetChildrenCore()
        {
            var children = ((IVisual)Owner).VisualChildren;

            if (children.Count == 0)
                return null;

            var result = new List<AutomationPeer>();

            foreach (var child in children)
            {
                if (child is Control c && c.IsVisible)
                {
                    result.Add(GetOrCreatePeer(c));
                }
            }

            return result;
        }

        protected override string GetClassNameCore() => Owner.GetType().Name;
        protected override string GetLocalizedControlTypeCore() => GetClassNameCore();
        protected override string? GetNameCore() => AutomationProperties.GetName(Owner);
        protected override AutomationRole GetRoleCore() => _role;
        protected override bool HasKeyboardFocusCore() => Owner.IsFocused;
        protected override bool IsControlElementCore() => GetRole() != AutomationRole.None;
        protected override bool IsEnabledCore() => Owner.IsEnabled;
        protected override bool IsKeyboardFocusableCore() => Owner.Focusable;
        protected override void SetFocusCore() => Owner.Focus();
        
        protected override bool ShowContextMenuCore()
        {
            var c = Owner;

            while (c is object)
            {
                if (c.ContextMenu is object)
                {
                    c.ContextMenu.Open(c);
                    return true;
                }

                c = c.Parent as Control;
            }

            return false;
        }

        private void VisualChildrenChanged(object sender, EventArgs e) => InvalidateChildren();

        private void OwnerPropertyChanged(object sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.Property == Visual.IsVisibleProperty)
            {
                var parent = Owner.GetVisualParent();
                if (parent is Control c)
                    GetOrCreatePeer(c).InvalidateChildren();
            }
            else if (e.Property == Visual.TransformedBoundsProperty)
            {
                InvalidateProperties();
            }
            else if (e.Property == Visual.VisualParentProperty)
            {
                InvalidateParent();
            }
        }
    }
}


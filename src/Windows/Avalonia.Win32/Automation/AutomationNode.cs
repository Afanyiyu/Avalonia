using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Automation;
using Avalonia.Controls.Automation.Peers;
using Avalonia.Controls.Automation.Platform;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Avalonia.Win32.Interop.Automation;

#nullable enable

namespace Avalonia.Win32.Automation
{
    [ComVisible(true)]
    internal partial class AutomationNode : MarshalByRefObject,
        IAutomationNode,
        IRawElementProviderSimple,
        IRawElementProviderSimple2,
        IRawElementProviderFragment,
        IRawElementProviderAdviseEvents,
        IInvokeProvider
    {
        private Rect _boundingRect;
        private List<AutomationNode>? _children;
        private string? _className;
        private UiaControlTypeId _controlType;
        private bool _hasKeyboardFocus;
        private bool _childrenValid;
        private bool _isControlElement;
        private bool _isDisposed;
        private bool _isEnabled;
        private bool _isKeyboardFocusable;
        private string? _localizedControlType;
        private string? _name;
        private AutomationNode? _parent;
        private bool _parentValid;
        private bool _propertiesValid;
        private RootAutomationNode? _root;
        private bool _rootValid;
        private int[] _runtimeId;
        private int _raiseFocusChanged;
        private int _raisePropertyChanged;

        public AutomationNode(AutomationPeer peer)
        {
            Dispatcher.UIThread.VerifyAccess();

            Peer = peer;
            _runtimeId = new int[] { 3, Peer.GetHashCode() };
        }

        public AutomationPeer Peer { get; }
        public Rect BoundingRectangle => Root?.ToScreen(_boundingRect) ?? default;
        public IAutomationNodeFactory Factory => AutomationNodeFactory.Instance;
        public virtual IRawElementProviderFragmentRoot? FragmentRoot => Root;
        public virtual IRawElementProviderSimple? HostRawElementProvider => null;
        public ProviderOptions ProviderOptions => ProviderOptions.ServerSideProvider;

        public RootAutomationNode? Root
        {
            get
            {
                if (!_rootValid)
                {
                    var _rootPeer = InvokeSync(() => Peer.GetRoot());
                    _root = (_rootPeer as AutomationPeer)?.Node as RootAutomationNode;
                    _rootValid = true;
                }

                return _root;
            }
        }

        public void Dispose()
        {
            // Feels like we should be calling UiaDisconnectProvider here, but that slows things
            // down HORRIBLY and looking through the WPF codebase seems that function is never
            // called there.
            _isDisposed = true;
        }

        public void ChildrenChanged()
        {
            if (_isDisposed)
                return;

            _childrenValid = false;
            UiaCoreProviderApi.UiaRaiseStructureChangedEvent(
                this,
                StructureChangeType.ChildrenInvalidated,
                _runtimeId,
                _runtimeId.Length);
        }

        public void ParentChanged()
        {
            _parent = null;
            _parentValid = false;
        }

        public void PropertyChanged() 
        {
            if (_isDisposed)
                return;

            Dispatcher.UIThread.VerifyAccess();
            UpdateCore();
        }

        public void RootChanged()
        {
            _root = null;
            _rootValid = false;
        }

        [return: MarshalAs(UnmanagedType.IUnknown)]
        public virtual object? GetPatternProvider(int patternId)
        {
            if (_isDisposed)
                return null;

            return (UiaPatternId)patternId switch
            {
                UiaPatternId.ExpandCollapse => Peer is IOpenCloseAutomationPeer ? this : null,
                UiaPatternId.Invoke => Peer is IInvocableAutomationPeer ? this : null,
                UiaPatternId.RangeValue => Peer is IRangeValueAutomationPeer ? this : null,
                UiaPatternId.Scroll => Peer is IScrollableAutomationPeer ? this : null,
                UiaPatternId.ScrollItem => this,
                UiaPatternId.Selection => Peer is ISelectingAutomationPeer ? this : null,
                UiaPatternId.SelectionItem => Peer is ISelectableAutomationPeer ? this : null,
                UiaPatternId.Toggle => Peer is IToggleableAutomationPeer ? this : null,
                UiaPatternId.Value => Peer is IStringValueAutomationPeer ? this : null,
                _ => null,
            };
        }

        public virtual object? GetPropertyValue(int propertyId)
        {
            if (_isDisposed)
                return null;

            if (!_propertiesValid)
                Update().Wait();

            return (UiaPropertyId)propertyId switch
            {
                UiaPropertyId.ClassName => _className,
                UiaPropertyId.ClickablePoint => new[] { BoundingRectangle.Center.X, BoundingRectangle.Center.Y },
                UiaPropertyId.ControlType => _controlType,
                UiaPropertyId.Culture => CultureInfo.CurrentCulture.LCID,
                UiaPropertyId.FrameworkId => "Avalonia",
                UiaPropertyId.HasKeyboardFocus => _hasKeyboardFocus,
                UiaPropertyId.IsContentElement => _isControlElement,
                UiaPropertyId.IsControlElement => _isControlElement,
                UiaPropertyId.IsEnabled => _isEnabled,
                UiaPropertyId.IsKeyboardFocusable => _isKeyboardFocusable,
                UiaPropertyId.LocalizedControlType => _localizedControlType,
                UiaPropertyId.Name => _name,
                UiaPropertyId.ProcessId => Process.GetCurrentProcess().Id,
                UiaPropertyId.RuntimeId => _runtimeId,
                _ => null,
            };
        }

        public int[]? GetRuntimeId() => _runtimeId;

        public virtual IRawElementProviderFragment? Navigate(NavigateDirection direction)
        {
            if (_isDisposed)
                return null;

            if (direction == NavigateDirection.Parent)
            {
                return GetParent();
            }

            EnsureChildren();

            return direction switch
            {
                NavigateDirection.NextSibling => GetParent()?.GetSibling(this, 1),
                NavigateDirection.PreviousSibling => GetParent()?.GetSibling(this, -1),
                NavigateDirection.FirstChild => _children?.FirstOrDefault(),
                NavigateDirection.LastChild => _children?.LastOrDefault(),
                _ => null,
            };
        }

        public void SetFocus()
        {
            if (_isDisposed)
                return;

            InvokeSync(() => Peer.SetFocus());
        }

        public async Task Update()
        {
            if (Dispatcher.UIThread.CheckAccess())
                UpdateCore();
            else
                await Dispatcher.UIThread.InvokeAsync(() => Update());
            _propertiesValid = true;
        }

        public override string ToString() => _className!;
        IRawElementProviderSimple[]? IRawElementProviderFragment.GetEmbeddedFragmentRoots() => null;
        void IRawElementProviderSimple2.ShowContextMenu() => InvokeSync(() => Peer.ShowContextMenu());
        void IInvokeProvider.Invoke() => InvokeSync<IInvocableAutomationPeer>(x => x.Invoke());

        void IRawElementProviderAdviseEvents.AdviseEventAdded(int eventId, int[] properties)
        {
            switch ((UiaEventId)eventId)
            {
                case UiaEventId.AutomationPropertyChanged:
                    ++_raisePropertyChanged;
                    break;
                case UiaEventId.AutomationFocusChanged:
                    ++_raiseFocusChanged;
                    break;
            }
        }

        void IRawElementProviderAdviseEvents.AdviseEventRemoved(int eventId, int[] properties)
        {
            switch ((UiaEventId)eventId)
            {
                case UiaEventId.AutomationPropertyChanged:
                    --_raisePropertyChanged;
                    break;
                case UiaEventId.AutomationFocusChanged:
                    --_raiseFocusChanged;
                    break;
            }
        }

        public void UpdateFocus()
        {
            UpdateProperty(UiaPropertyId.HasKeyboardFocus, ref _hasKeyboardFocus, Peer.HasKeyboardFocus());
        }

        protected void InvokeSync(Action action)
        {
            if (_isDisposed)
                return;

            if (Dispatcher.UIThread.CheckAccess())
            {
                action();
            }
            else
            {
                Dispatcher.UIThread.InvokeAsync(action).Wait();
            }
        }

        [return: MaybeNull]
        protected T InvokeSync<T>(Func<T> func)
        {
            if (_isDisposed)
                return default;

            if (Dispatcher.UIThread.CheckAccess())
            {
                return func();
            }
            else
            {
                return Dispatcher.UIThread.InvokeAsync(func).Result;
            }
        }

        protected void InvokeSync<TInterface>(Action<TInterface> action)
        {
            if (Peer is TInterface i)
            {
                try
                {
                    InvokeSync(() => action(i));
                }
                catch (AggregateException e) when (e.InnerException is ElementNotEnabledException)
                {
                    throw new COMException(e.Message, UiaCoreProviderApi.UIA_E_ELEMENTNOTENABLED);
                }
            }
        }

        [return: MaybeNull]
        protected TResult InvokeSync<TInterface, TResult>(Func<TInterface, TResult> func)
        {
            if (Peer is TInterface i)
            {
                try
                {
                    return InvokeSync(() => func(i));
                }
                catch (AggregateException e) when (e.InnerException is ElementNotEnabledException)
                {
                    throw new COMException(e.Message, UiaCoreProviderApi.UIA_E_ELEMENTNOTENABLED);
                }
            }

            return default;
        }

        protected void RaiseFocusChanged(AutomationNode? focused)
        {
            if (_raiseFocusChanged > 0)
            {
                UiaCoreProviderApi.UiaRaiseAutomationEvent(
                    focused,
                    (int)UiaEventId.AutomationFocusChanged);
            }
        }

        protected virtual void UpdateCore()
        {
            UpdateProperty(UiaPropertyId.BoundingRectangle, ref _boundingRect, Peer.GetBoundingRectangle());
            UpdateProperty(UiaPropertyId.ClassName, ref _className, Peer.GetClassName());
            UpdateProperty(UiaPropertyId.ControlType, ref _controlType, RoleToControlType(Peer.GetRole()));
            UpdateProperty(UiaPropertyId.IsControlElement, ref _isControlElement, Peer.IsControlElement());
            UpdateProperty(UiaPropertyId.IsEnabled, ref _isEnabled, Peer.IsEnabled());
            UpdateProperty(UiaPropertyId.IsKeyboardFocusable, ref _isKeyboardFocusable, Peer.IsKeyboardFocusable());
            UpdateProperty(UiaPropertyId.LocalizedControlType, ref _localizedControlType, Peer.GetLocalizedControlType());
            UpdateProperty(UiaPropertyId.Name, ref _name, Peer.GetName());
            UpdateFocus();
            UpdateExpandCollapse();
            UpdateRangeValue();
            UpdateScroll();
            UpdateSelection();
            UpdateToggle();
            UpdateValue();
        }

        private void UpdateProperty<T>(UiaPropertyId id, ref T _field, T value)
        {
            if (!EqualityComparer<T>.Default.Equals(_field, value))
            {
                _field = value;
                if (_raisePropertyChanged > 0)
                    UiaCoreProviderApi.UiaRaiseAutomationPropertyChangedEvent(this, (int)id, null, null);
            }
        }

        private void EnsureChildren()
        {
            if (!_childrenValid)
            {
                InvokeSync(() => LoadChildren());
                _childrenValid = true;
            }
        }

        private void LoadChildren()
        {
            var childPeers = InvokeSync(() => Peer.GetChildren());

            _children?.Clear();

            if (childPeers is null)
                return;

            foreach (var childPeer in childPeers)
            {
                _children ??= new List<AutomationNode>();

                if (childPeer.Node is AutomationNode child)
                {
                    _children.Add(child);
                }
                else
                {
                    throw new AvaloniaInternalException(
                        "AutomationPeer platform implementation not recognised.");
                }
            }
        }

        private AutomationNode? GetParent()
        {
            if (!_parentValid)
            {
                _parent = InvokeSync(() => Peer.GetParent())?.Node as AutomationNode;
                _parentValid = true;
            }

            return _parent;
        }

        private AutomationNode? GetSibling(AutomationNode child, int direction)
        {
            EnsureChildren();

            var index = _children?.IndexOf(child) ?? -1;

            if (index >= 0)
            {
                index += direction;

                if (index >= 0 && index < _children!.Count)
                {
                    return _children[index];
                }
            }

            return null;
        }

        private AutomationPeer GetPeer(Control control)
        {
            return ControlAutomationPeer.GetOrCreatePeer(Factory, control);
        }

        private WindowBaseAutomationPeer? GetRootPeer(AutomationPeer peer)
        {
            if (peer is WindowBaseAutomationPeer peerAsRoot)
            {
                return peerAsRoot;
            }
            else if (peer is ControlAutomationPeer controlPeer &&
                controlPeer.Owner.GetVisualRoot() is Control rootControl &&
                GetPeer(rootControl) is WindowBaseAutomationPeer rootPeer)
            {
                return rootPeer;
            }

            return null;
        }

        private WindowImpl? GetRootWindowImpl(AutomationPeer peer)
        {
            return (GetRootPeer(peer)?.Node as RootAutomationNode)?.WindowImpl;
        }

        private static UiaControlTypeId RoleToControlType(AutomationRole role)
        {
            return role switch
            {
                AutomationRole.Button => UiaControlTypeId.Button,
                AutomationRole.CheckBox => UiaControlTypeId.CheckBox,
                AutomationRole.ComboBox => UiaControlTypeId.ComboBox,
                AutomationRole.Edit => UiaControlTypeId.Edit,
                AutomationRole.Group => UiaControlTypeId.Group,
                AutomationRole.List => UiaControlTypeId.List,
                AutomationRole.ListItem => UiaControlTypeId.ListItem,
                AutomationRole.Menu => UiaControlTypeId.Menu,
                AutomationRole.MenuItem => UiaControlTypeId.MenuItem,
                AutomationRole.Slider => UiaControlTypeId.Slider,
                AutomationRole.TabControl => UiaControlTypeId.Tab,
                AutomationRole.TabItem => UiaControlTypeId.TabItem,
                AutomationRole.Text => UiaControlTypeId.Text,
                AutomationRole.Toggle => UiaControlTypeId.Button,
                AutomationRole.Window => UiaControlTypeId.Window,
                _ => UiaControlTypeId.Custom,
            };
        }
    }
}

using System.ComponentModel;
using Avalonia.Controls.Automation.Platform;
using Avalonia.Input;
using Avalonia.Platform;
using Avalonia.VisualTree;

#nullable enable

namespace Avalonia.Controls.Automation.Peers
{
    public class WindowBaseAutomationPeer : ControlAutomationPeer, IRootAutomationPeer
    {
        private Control? _focus;

        public WindowBaseAutomationPeer(IAutomationNodeFactory factory, WindowBase owner)
            : base(factory, owner, AutomationRole.Window)
        {
        }

        public ITopLevelImpl PlatformImpl => ((WindowBase)Owner).PlatformImpl;

        public AutomationPeer? GetFocus() => _focus is object ? GetOrCreatePeer(_focus) : null;

        public AutomationPeer? GetPeerFromPoint(Point p)
        {
            var hit = Owner.GetVisualAt(p)?.FindAncestorOfType<Control>(includeSelf: true);
            return hit is object ? GetOrCreatePeer(hit) : null;
        }

        protected override string GetNameCore() => Owner.GetValue(Window.TitleProperty);

        protected void StartTrackingFocus()
        {
            KeyboardDevice.Instance.PropertyChanged += KeyboardDevicePropertyChanged;
            OnFocusChanged(KeyboardDevice.Instance.FocusedElement);
        }

        protected void StopTrackingFocus()
        {
            KeyboardDevice.Instance.PropertyChanged -= KeyboardDevicePropertyChanged;
        }

        private void OnFocusChanged(IInputElement? focus)
        {
            var oldFocus = _focus;
            
            _focus = focus?.VisualRoot == Owner ? focus as Control : null;
            
            if (_focus != oldFocus)
            {
                var peer = _focus is object ? GetOrCreatePeer(_focus) : null;
                ((IRootAutomationNode)Node).FocusChanged(peer);
            }
        }

        private void KeyboardDevicePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(KeyboardDevice.FocusedElement))
            {
                OnFocusChanged(KeyboardDevice.Instance.FocusedElement);
            }
        }
    }
}



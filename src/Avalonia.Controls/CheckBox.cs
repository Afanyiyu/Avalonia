using Avalonia.Controls.Automation.Peers;
using Avalonia.Controls.Automation.Platform;
using Avalonia.Controls.Primitives;

namespace Avalonia.Controls
{
    /// <summary>
    /// A check box control.
    /// </summary>
    public class CheckBox : ToggleButton
    {
        protected override AutomationPeer OnCreateAutomationPeer(IAutomationNodeFactory factory)
        {
            return new ToggleButtonAutomationPeer(factory, this, AutomationRole.CheckBox);
        }
    }
}

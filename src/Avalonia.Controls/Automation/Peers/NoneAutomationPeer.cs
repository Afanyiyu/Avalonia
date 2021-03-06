using Avalonia.Controls.Automation.Platform;

#nullable enable

namespace Avalonia.Controls.Automation.Peers
{
    /// <summary>
    /// An automation peer which represents an element that is exposed to automation as non-
    /// interactive or as not contributing to the logical structure of the application.
    /// </summary>
    public class NoneAutomationPeer : ControlAutomationPeer
    {
        public NoneAutomationPeer(IAutomationNodeFactory factory, Control owner)
            : base(factory, owner, AutomationRole.None) 
        { 
        }

        protected override bool IsControlElementCore() => false;
    }
}


namespace Avalonia.Automation.Provider
{
    /// <summary>
    /// Exposes methods and properties to support access by a UI Automation client to a control
    /// that acts as a scrollable container for a collection of child objects. 
    /// </summary>
    public interface IScrollProvider
    {
        Size GetExtent();
        Vector GetOffset();
        Size GetViewport();
        void SetOffset(Vector value);
        void PageUp();
        void PageDown();
        void PageLeft();
        void PageRight();
        void LineUp();
        void LineDown();
        void LineLeft();
        void LineRight();
    }
}

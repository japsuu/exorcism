using UnityEngine.EventSystems;

namespace Helpers
{
    /// <summary>
    /// Helper methods for mouse input.
    /// </summary>
    public static class MouseHelpers
    {
        /// <summary>
        /// Check if the mouse pointer is on a UI element.
        /// </summary>
        /// <returns></returns>
        public static bool IsMousePointerOnUiElement()
        {
            return EventSystem.current.IsPointerOverGameObject();
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ErisJGDK.Base.Extensions
{
    public static class EventSystemExtensions
    {
        /// <summary>
        /// Returns an array of UI objects hovered by mouse.
        /// </summary>
        public static GameObject[] GetHoveredUIElements(this EventSystem eventSystem)
        {
            PointerEventData eventData = new(eventSystem)
            {
                position = Input.mousePosition
            };

            List<RaycastResult> results = new();
            eventSystem.RaycastAll(eventData, results);

            return results.Select(x => x.gameObject).ToArray();
        }
    }
}

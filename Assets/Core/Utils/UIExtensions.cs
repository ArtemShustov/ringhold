using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Core.Utils {
	public class UIExtensions {
		public static bool IsOverUI(Vector2 screenPoint) {
			var eventData = new PointerEventData(EventSystem.current) {
				position = screenPoint
			};

			return IsOverUI(eventData);
		}
		public static bool IsOverUI(PointerEventData eventData) {
			List<RaycastResult> results = new List<RaycastResult>();
			EventSystem.current?.RaycastAll(eventData, results);

			return results.Count > 0;
		}
	}
}
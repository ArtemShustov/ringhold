using UnityEngine;

namespace Ringhold.World {
	public class Ring: MonoBehaviour {
		[field: Header("Settings")]
		[field: SerializeField] public float Gravity { get; set; } = 9.81f;

		public float Radius { get; set; }
		
		public Vector3 GetGravityAt(Vector3 point) {
			var center = transform.position;
			var toPoint = point - center;

			var localToPoint = transform.InverseTransformDirection(toPoint);

			// Проецируем точку на плоскость кольца (обнуляем Z)
			var localOnPlane = new Vector3(localToPoint.x, localToPoint.y, 0f);

			if (localOnPlane.sqrMagnitude < Mathf.Epsilon) {
				return Vector3.zero;
			}

			// Ближайшая точка на кольце — только в плоскости, без Z
			var localNearest = localOnPlane.normalized * Radius;

			// Гравитация от спроецированной точки к ближайшей на кольце
			var localGravity = (localNearest - localOnPlane).normalized;

			return transform.TransformDirection(localGravity) * Gravity;
		}
	}
}
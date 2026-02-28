using UnityEngine;

namespace Ringhold.Scripts.World {
	public class Ring: MonoBehaviour {
		[field: Header("Settings")]
		[field: SerializeField] public float Gravity { get; set; } = 9.81f;

		public float Radius { get; set; }
		
		public Vector3 GetGravityAt(Vector3 point) {
			var center = transform.position;
			var toPoint = point - center;
    
			var onPlane = new Vector3(toPoint.x, toPoint.y, 0f);
    
			if (onPlane.sqrMagnitude < Mathf.Epsilon) {
				return Vector3.zero;
			}
    
			var nearestOnRing = center + onPlane.normalized * Radius;
			return (nearestOnRing - point).normalized * Gravity;
		}
	}
}
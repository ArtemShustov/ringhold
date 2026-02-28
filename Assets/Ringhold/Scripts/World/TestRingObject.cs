using UnityEngine;

namespace Ringhold.Scripts.World {
	public class TestRingObject: MonoBehaviour {
		[SerializeField] private Ring _ring;

		private void OnDrawGizmos() {
			var gravity = _ring.GetGravityAt(transform.position);
			
			Gizmos.DrawSphere(transform.position, 0.5f);
			Gizmos.DrawLine(transform.position, transform.position + gravity);
		}
	}
}
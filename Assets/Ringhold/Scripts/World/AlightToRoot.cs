using Core.DependencyInjection;
using UnityEngine;

namespace Ringhold.World {
	[ExecuteAlways]
	public class AlightToRoot: MonoBehaviour {
		[Inject] private WorldRoot _world;

		private void Update() {
			#if UNITY_EDITOR
			if (_world == null) {
				_world = FindFirstObjectByType<WorldRoot>();
			}
			if (_world == null) {
				return;
			}
			#endif
			UpdateRotation();
		}

		private void UpdateRotation() {
			var center = _world.Center;
			center.y = transform.position.y;
			var toCenter = (center - transform.position).normalized;
			
			transform.rotation = Quaternion.LookRotation(toCenter, Vector3.up);
		}

		private void OnEnable() {
			UpdateRotation();
		}

		private void OnDrawGizmosSelected() {
			if (_world == null) {
				_world = FindFirstObjectByType<WorldRoot>();
			}
			if (_world == null) {
				return;
			}
			
			var center = _world.Center;
			center.y = transform.position.y;
			var toCenter = (center - transform.position).normalized;
			Gizmos.DrawLine(transform.position, transform.position + toCenter);
		}
	}
}
using Core.DependencyInjection;
using UnityEngine;

namespace Ringhold.World {
	public class AlightToRoot: MonoBehaviour {
		[Inject] private WorldRoot _world;

		private void Update() {
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
	}
}
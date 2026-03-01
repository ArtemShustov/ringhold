using UnityEngine;

namespace Ringhold.Scripts.World {
	public class WorldRoot: MonoBehaviour {
		[field: SerializeField] private Transform _center;
		[field: SerializeField] private Vector2 _walkableRadius = new Vector2(16, 26);
		
		public Vector3 Center => _center.position;
		public Vector2 WalkableRadius => _walkableRadius;
	}
}
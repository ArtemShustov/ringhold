using UnityEngine;

namespace Ringhold.World {
	public class WorldRoot: MonoBehaviour {
		[SerializeField] private Ring _ring;
		
		private static WorldRoot _instance;

		private void Awake() {
			if (_instance != null && _instance != this) {
				Debug.LogError($"{nameof(WorldRoot)} is already exists.");
			}
			_instance = this;
		}

		public static Vector3 GetGravityAt(Vector3 position) => _instance._ring.GetGravityAt(position);
	}
}
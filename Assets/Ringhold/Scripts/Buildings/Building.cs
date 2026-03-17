using UnityEngine;

namespace Ringhold.Buildings {
	public class Building: MonoBehaviour {
		[field: SerializeField] public BuildingDefinition Definition { get; set; }
	}
}
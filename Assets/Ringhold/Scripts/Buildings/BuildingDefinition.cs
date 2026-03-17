using Ringhold.CMS;
using UnityEngine;
using UnityEngine.Localization;

namespace Ringhold.Buildings {
	[CreateAssetMenu(menuName = "Ringhold/CMS/Building")]
	public class BuildingDefinition: EntityDefinition<Building> {
		[field: SerializeField] public LocalizedString Name { get; private set; }
	}
}
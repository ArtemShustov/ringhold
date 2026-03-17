using Ringhold.CMS;
using UnityEngine;
using UnityEngine.Localization;

namespace Ringhold.Buildings {
	[CreateAssetMenu(menuName = "Ringhold/CMS/Building")]
	public class BuildingDefinition: ScriptableObject, ICMSEntry {
		[field: SerializeField, HideInInspector] public string Id { get; private set; }
		[field: SerializeField] public LocalizedString Name { get; private set; }
		[field: Space]
		[field: SerializeField] public Building Prefab { get; private set; }
	}
}
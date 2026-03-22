using Ringhold.CMS;
using UnityEngine;
using UnityEngine.Localization;

namespace Ringhold.Items {
	[CreateAssetMenu(menuName = "Ringhold/CMS/Item")]
	public class ItemDefinition: ScriptableObject, ICMSEntry {
		[field: SerializeField, HideInInspector] public string Id { get; private set; }
		[field: SerializeField] public LocalizedString Name { get; private set; }
		[field: SerializeField] public Sprite Icon { get; private set; }
		[field: Space]
		[field: SerializeField] public DroppedItemDefinition Prefab { get; private set; }
	}
}
using UnityEngine;
using UnityEngine.Localization;

namespace Ringhold.Items {
	[CreateAssetMenu(menuName = "Ringhold/CMS/Item")]
	public class Item: ScriptableObject {
		[field: SerializeField] public string Id { get; private set; }
		[field: SerializeField] public LocalizedString Name { get; private set; }
	}
}
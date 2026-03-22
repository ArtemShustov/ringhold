using System.Collections.Generic;
using Ringhold.Buildings;
using Ringhold.CMS;
using Ringhold.Items;
using UnityEngine;
using UnityEngine.Localization;

namespace Ringhold.Construction {
	[CreateAssetMenu(menuName = "Ringhold/CMS/Construction scheme")]
	public class ConstructionSchemeDefinition: ScriptableObject, ICMSEntry {
		[field: Header("Basic info")]
		[field: SerializeField, HideInInspector] public string Id { get; private set; }
		[field: SerializeField] public Sprite Icon { get; private set; }
		[field: SerializeField] public LocalizedString Name { get; private set; }
		
		[field: Header("Scheme")]
		[field: SerializeField] public string Category { get; private set; }
		[field: SerializeField] public ConstructionGhost Prefab { get; private set; }
		[field: Space]
		[field: SerializeField] public BuildingDefinition Building { get; private set; }
		[SerializeField] private List<FastItemStack> _requiredItems;
		
		public IReadOnlyList<FastItemStack> RequiredItems => _requiredItems;
	}
}
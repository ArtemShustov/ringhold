using System.Collections.Generic;
using Ringhold.Buildings;
using Ringhold.CMS;
using Ringhold.Items;
using UnityEngine;

namespace Ringhold.Construction {
	[CreateAssetMenu(menuName = "Ringhold/CMS/Construction scheme")]
	public class ConstructionSchemeDefinition: ScriptableObject, ICMSEntry {
		[field: SerializeField, HideInInspector] public string Id { get; private set; }
		[Space]
		[SerializeField] private List<FastItemStack> _requiredItems;
		[field: SerializeField] public BuildingDefinition Building { get; private set; }
		
		public IReadOnlyList<FastItemStack> RequiredItems => _requiredItems;
	}
}
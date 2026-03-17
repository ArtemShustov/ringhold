using System;
using System.Linq;
using UnityEngine;

namespace Ringhold.Items.ItemFilters {
	[Serializable]
	public class WhitelistFilter: IItemFilter {
		[SerializeField] private ItemDefinition[] _whitelist;
		
		public bool Accept(ItemDefinition item) {
			return _whitelist.Contains(item);
		}
	}
}
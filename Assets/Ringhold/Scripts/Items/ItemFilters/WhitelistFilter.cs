using System;
using System.Linq;
using UnityEngine;

namespace Ringhold.Items.ItemFilters {
	[Serializable]
	public class WhitelistFilter: IItemFilter {
		[SerializeField] private Item[] _whitelist;
		
		public bool Accept(Item item) {
			return _whitelist.Contains(item);
		}
	}
}
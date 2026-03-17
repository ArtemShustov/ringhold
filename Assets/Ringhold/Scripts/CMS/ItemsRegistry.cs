using Ringhold.Items;

namespace Ringhold.CMS {
	public class ItemsRegistry {
		public DroppedItem GetDroppedItem(ItemDefinition item, int count = 1) {
			var instance = GameResources.EntityRegistry.Pool.Get(item.Prefab);
			instance.Set(item, count);
			return instance;
		}
	}
}
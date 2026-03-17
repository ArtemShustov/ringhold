using Core.DependencyInjection;
using Core.Utils;
using Ringhold.Items;

namespace Ringhold.CMS {
	public class ItemsPool {
		[ClearOnReload] private static ItemsPool _instance;
		public static ItemsPool Instance => GetOrCreateInstance();
		
		private static ItemsPool GetOrCreateInstance() {
			_instance ??= new ItemsPool();
			return _instance;
		}
		
		public DroppedItem GetDroppedItem(ItemDefinition item, int count = 1) {
			var instance = Injecting.Instantiate(item.Prefab, SceneContext.Current.Container);
			instance.Set(item, count);
			return instance;
		}
	}
}
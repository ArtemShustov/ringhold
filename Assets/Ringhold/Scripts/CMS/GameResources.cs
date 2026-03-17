using Core.Utils;

namespace Ringhold.CMS {
	public class GameResources {
		[ClearOnReload] private static GameResources _instance;
		public static GameResources Instance {
			get {
				_instance ??= new GameResources();
				return _instance;
			}
		}
		
		public readonly EntityRegistry Entities = new EntityRegistry();
		public static EntityRegistry EntityRegistry => Instance.Entities;
		
		public readonly ItemsRegistry Items = new ItemsRegistry();
		public static ItemsRegistry ItemsRegistry => Instance.Items;
	}
}
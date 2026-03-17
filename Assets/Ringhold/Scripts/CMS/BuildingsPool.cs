using Core.DependencyInjection;
using Core.Utils;
using Ringhold.Buildings;

namespace Ringhold.CMS {
	public class BuildingsPool {
		[ClearOnReload] private static BuildingsPool _instance;
		public static BuildingsPool Instance => GetOrCreateInstance();
		
		private static BuildingsPool GetOrCreateInstance() {
			_instance ??= new BuildingsPool();
			return _instance;
		}
		
		public Building GetInstance(BuildingDefinition definition) {
			var instance = Injecting.Instantiate(definition.Prefab, SceneContext.Current.Container);
			instance.Definition = definition;
			return instance;
		}
	}
}
using Core.DependencyInjection;
using UnityEngine;

namespace Ringhold.CMS {
	public interface IEntityPool {
		T Get<T>(EntityDefinition<T> definition) where T: Component;
	}
	public class EntityPool: IEntityPool {
		public T Get<T>(EntityDefinition<T> definition) where T: Component {
			var instance = Injecting.Instantiate(definition.Prefab, SceneContext.Current.Container);
			var entity = instance.gameObject.GetComponent<Entity>() ?? instance.gameObject.AddComponent<Entity>();
			entity.Definition = definition;
			entity.Pool = this;
			return instance;
		}
	}
}
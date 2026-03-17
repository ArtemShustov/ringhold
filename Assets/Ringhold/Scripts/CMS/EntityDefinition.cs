using UnityEngine;

namespace Ringhold.CMS {
	[CreateAssetMenu(menuName = "Ringhold/CMS/Entity")]
	public class EntityDefinition: EntityDefinition<Entity> { }

	public interface IEntityDefinition: ICMSEntry { }
	public abstract class EntityDefinition<T>: ScriptableObject, IEntityDefinition where T: Component {
		[field: SerializeField, HideInInspector] public string Id { get; private set; }
		[field: SerializeField] public T Prefab { get; private set; }
	}
}
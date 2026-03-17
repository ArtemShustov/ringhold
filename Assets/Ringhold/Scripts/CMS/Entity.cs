using UnityEngine;

namespace Ringhold.CMS {
	public class Entity: MonoBehaviour {
		public IEntityDefinition Definition { get; set; }
		public IEntityPool Pool { get; set; }
		
		public bool Is<T>(out T component) => gameObject.TryGetComponent<T>(out component);
		public T As<T>() => gameObject.GetComponent<T>();
		
		public void ReturnToPool() => Destroy(gameObject);
		public virtual void OnReturnToPool() { }
		public virtual void OnTakeFromPool() { }
	}
}
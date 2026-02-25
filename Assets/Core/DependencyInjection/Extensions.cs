using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core.DependencyInjection {
	public static class Extensions {
		public static void InjectTree(this GameObject gameObject, DIContainer container) {
			Injecting.InjectTree(gameObject, container);
		}
		public static void Inject(this GameObject gameObject, DIContainer container) {
			Injecting.Inject(gameObject, container);
		}
		public static void Inject(this MonoBehaviour monoBehaviour, DIContainer container) {
			Injecting.Inject(monoBehaviour, container);
		}
		public static void Inject(this object obj, DIContainer container) {
			Injecting.Inject(obj, container);
		}
		public static void Inject(this Scene scene, DIContainer container) {
			var roots = scene.GetRootGameObjects();
			foreach (var root in roots) {
				Injecting.InjectTree(root, container);
			}
		}
	}
}
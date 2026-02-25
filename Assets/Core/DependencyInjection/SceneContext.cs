using UnityEngine;

namespace Core.DependencyInjection {
	[DefaultExecutionOrder(-10_000)]
	public class SceneContext: MonoBehaviour {
		[SerializeField] private Registrator[] _registrators;
		public DIContainer Container { get; private set; }

		private void Awake() {
			if (_instance != null) {
				Debug.LogWarning("Multiple instances of SceneContext!!!");
			}
			_instance = this;
			
			Container = new DIContainer(GameContext.Container);
			foreach (var registrator in _registrators) {
				registrator.RegisterAll(Container);
			}
			
			Injecting.InjectAllOnScene(Container);
		}

		private static SceneContext _instance;
		public static SceneContext Current => CreateDefault();
		private static SceneContext CreateDefault() {
			if (_instance != null) {
				return _instance;
			}
			_instance = new GameObject("SceneContext", typeof(SceneContext)).GetComponent<SceneContext>();
			return _instance;
		}
	}
}
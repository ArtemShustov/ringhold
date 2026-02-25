using UnityEngine;

namespace Core.DependencyInjection {
	public class GameContext {
		private static GameContext _instance = new GameContext();
		
		private readonly DIContainer _container = new DIContainer();
		public static DIContainer Container => _instance._container;

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void Initialize() {
			_instance = new GameContext();
		}
	}
}
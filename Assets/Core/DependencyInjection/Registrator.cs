using UnityEngine;

namespace Core.DependencyInjection {
	public abstract class Registrator: MonoBehaviour {
		public abstract void RegisterAll(DIContainer container);
	}
}
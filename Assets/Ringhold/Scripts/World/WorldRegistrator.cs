using Core.DependencyInjection;
using UnityEngine;

namespace Ringhold.Scripts.World {
	public class WorldRegistrator: Registrator {
		[SerializeField] private WorldRoot _world;
		
		public override void RegisterAll(DIContainer container) {
			container.RegisterInstance(_world);
		}
	}
}
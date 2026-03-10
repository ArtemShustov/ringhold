using Core.DependencyInjection;
using UnityEngine;

namespace Ringhold.World {
	public class WorldRegistrator: Registrator {
		[SerializeField] private WorldRoot _world;
		[SerializeField] private WorldTicker _ticker;
		
		public override void RegisterAll(DIContainer container) {
			container.RegisterInstance(_world);
			
			container.RegisterInstance(_ticker);
			container.RegisterInstance<ITickGroup>(_ticker.Main);
		}
	}
}
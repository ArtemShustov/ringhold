using Core.DependencyInjection;
using Ringhold.Characters;
using Ringhold.Characters.Inputs;
using Ringhold.Inputs;
using UnityEngine;

namespace Ringhold.World {
	public class WorldEntryPoint: MonoBehaviour {
		[SerializeField] private Character _character;
		[Inject] private InputPlayers _players;
		private PlayerDriver _driver;
		
		private void Awake() {
			_driver = new PlayerDriver(_players.MainPlayer);
			_character.InputContainer.Select(_driver);
			_driver.Enable();
		}

		private void OnDestroy() {
			_driver?.Dispose();
		}
	}
}
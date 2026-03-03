using Core.DependencyInjection;
using UnityEngine;

namespace Ringhold.Inputs {
	public class InputPlayersRegistrator: Registrator {
		[SerializeField] private bool _debug;
		
		public override void RegisterAll(DIContainer container) {
			var players = new InputPlayers();
			container.RegisterInstance(players);
			container.RegisterInstance(players.MainPlayer);
			container.RegisterInstance(players.MainPlayer, "main");

			if (_debug) {
				players.Added += player => Debug.Log($"Player {player.User.id} added!");
				players.Removed += player => Debug.Log($"Player {player.User.id} removed!");
				players.MainPlayer.DeviceChanged += device => Debug.Log($"MainPlayer device changed to {device}.");
			}
		}
	}
}
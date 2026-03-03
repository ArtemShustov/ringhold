using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;

namespace Ringhold.Inputs {
	public class InputPlayers: IDisposable {
		private readonly List<InputPlayer> _players = new List<InputPlayer>();
		private bool _waitingForNewPlayer;
		
		public InputPlayer MainPlayer => _players.FirstOrDefault();

		public event Action<InputPlayer> Added;
		public event Action<InputPlayer> Removed;
		
		public InputPlayers() {
			InputUser.listenForUnpairedDeviceActivity += 1;
			InputUser.onUnpairedDeviceUsed += OnUnpairedDeviceUsed;
			InputUser.onChange += OnUserChange;
			
			_players.Add(new InputPlayer(true));
		}
		public void Dispose() {
			InputUser.listenForUnpairedDeviceActivity -= 1;
			InputUser.onUnpairedDeviceUsed -= OnUnpairedDeviceUsed;
			InputUser.onChange -= OnUserChange;
		}

		public void WaitingForNewPlayers(bool active) {
			_waitingForNewPlayer = active;
		}
		
		private void HandleUnpairedDevice(InputDevice device) {
			var isDeviceUsed = _players.Any(p => p.IsPaired(device));
			if (isDeviceUsed) {
				return;
			}
			
			var isKeyboardOrMouse = device is Keyboard or Mouse;

			if (_waitingForNewPlayer && !isKeyboardOrMouse) {
				var player = new InputPlayer();
				player.PairDevice(device);
				_players.Add(player);
				Added?.Invoke(player);
				return;
			}
			
			MainPlayer.UnpairDevices();
			if (isKeyboardOrMouse) {
				MainPlayer.PairDevice(Keyboard.current);
				MainPlayer.PairDevice(Mouse.current);
			} else {
				MainPlayer.PairDevice(device);
			}
		}
		private void OnUserChange(InputUser user, InputUserChange change, InputDevice device) {
			switch (change) {
				case InputUserChange.DeviceLost: {
					var localPlayer = _players.FirstOrDefault(p => p.IsPaired(device));
					if (localPlayer == null) {
						return;
					}
					localPlayer.UnpairDevices();
					if (!localPlayer.IsMain) {
						_players.Remove(localPlayer);
						Removed?.Invoke(localPlayer);
						localPlayer.Dispose();
					}
					break;
				}
			}
		}
		
		private void OnUnpairedDeviceUsed(InputControl control, InputEventPtr ptr) {
			HandleUnpairedDevice(control.device);
		}
	}
}
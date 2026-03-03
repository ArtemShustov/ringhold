using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

namespace Ringhold.Inputs {
	public class InputPlayer: IDisposable {
		private readonly DefaultActions _actions;
		private readonly InputUser _user;
		private InputDeviceType _currentDevice;

		public DefaultActions Actions => _actions;
		public InputUser User => _user;
		public InputDeviceType Current => _currentDevice;
		public bool IsMain { get; }

		public event Action<InputDeviceType> DeviceChanged; 
		public event Action<InputControlScheme> ControlSchemeChanged;

		public InputPlayer(bool isMain = false) {
			IsMain = isMain;
			_actions = new DefaultActions();
			_actions.Enable();
			_user = InputUser.CreateUserWithoutPairedDevices();
			_user.AssociateActionsWithUser(_actions.asset);
		}
		public void Dispose() {
			_actions.Disable();
			_actions.Dispose();
            
			_user.UnpairDevicesAndRemoveUser();
		}

		public void PairDevice(InputDevice device) {
			InputUser.PerformPairingWithDevice(device, _user);

			var controlScheme = InputControlScheme.FindControlSchemeForDevices(_user.pairedDevices, _actions.controlSchemes, device);
			if (controlScheme.HasValue) {
				_user.ActivateControlScheme(controlScheme.Value);
				ControlSchemeChanged?.Invoke(controlScheme.Value);
			} else {
				Debug.LogWarning($"No control scheme found for {device.displayName}!");
			}

			var devType = device switch {
				Gamepad => InputDeviceType.Gamepad,
				Touchscreen => InputDeviceType.Touch,
				_ => InputDeviceType.Keyboard
			};
			if (devType != _currentDevice) {
				_currentDevice = devType;
				DeviceChanged?.Invoke(devType);
			}
		}
		public void UnpairDevices() {
			_user.UnpairDevices();
		}
		public bool IsPaired(InputDevice device) {
			return _user.pairedDevices.Contains(device);
		}
		
		public void TryPairVirtualMouse() {
			var virtualMouse = InputSystem.devices.FirstOrDefault(d => d is Mouse m && m.name.Contains("Virtual"));
			if (virtualMouse != null && !IsPaired(virtualMouse)) {
				InputUser.PerformPairingWithDevice(virtualMouse, _user);
				Debug.Log("Virtual mouse paired");
			}
		}
	}
}
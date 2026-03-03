using UnityEngine.InputSystem;

namespace Ringhold.Inputs {
	public enum InputDeviceType {
		Keyboard,
		Gamepad,
		Touch,
	}

	public static class InputDeviceTypeExtensions {
		public static InputDeviceType ToInputDeviceType(this InputDevice device) => device switch {
			Keyboard or Mouse => InputDeviceType.Keyboard,
			Gamepad => InputDeviceType.Gamepad,
			Touchscreen => InputDeviceType.Touch,
			_ => InputDeviceType.Keyboard
		};
	}
}
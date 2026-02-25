using System;
using Ringhold.Inputs;
using UnityEngine;

namespace Ringhold.Characters {
	public class CharacterInput: MonoBehaviour {
		private Camera _camera;
		private DefaultActions _actions;

		public Vector2 Move => GetRelatedMove();

		private void Awake() {
			_actions = new DefaultActions();
			_camera = Camera.main;
		}

		private Vector2 GetRelatedMove() {
			if (_actions == null) {
				return Vector2.zero;
			}
			
			var input = _actions.Player.Move.ReadValue<Vector2>();

			var directionAngle = Mathf.Atan2(input.x, input.y) * Mathf.Rad2Deg;
			if (directionAngle < 0) {
				directionAngle += 360;
			}
			directionAngle += _camera.transform.eulerAngles.y;
			if (directionAngle > 360) {
				directionAngle -= 360;
			}
			var forward = Quaternion.Euler(0, directionAngle, 0) * Vector3.forward;
			var result = forward * Mathf.Clamp01(input.magnitude);
			return new Vector2(result.x, result.z);
		}
		
		private void OnEnable() {
			_actions.Player.Enable();
		}
		private void OnDisable() {
			_actions.Player.Disable();
		}
	}
}
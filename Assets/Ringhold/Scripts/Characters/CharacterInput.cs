using System;
using Ringhold.Inputs;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Ringhold.Characters {
	public class CharacterInput: MonoBehaviour {
		private Camera _camera;
		private DefaultActions _actions;

		public Vector2 Move => GetRelatedMove();
		public event Action Interact;

		private void Awake() {
			_actions = new DefaultActions();
			_camera = Camera.main;
		}

		private Vector2 GetRelatedMove() {
			if (_actions == null) {
				return Vector2.zero;
			}
    
			var input = _actions.Player.Move.ReadValue<Vector2>();
			if (input.sqrMagnitude < 0.0001f) {
				return Vector2.zero;
			}

			var up = transform.up;
			var camForward = Vector3.ProjectOnPlane(_camera.transform.forward, up);
			var camRight = Vector3.ProjectOnPlane(_camera.transform.right, up);

			if (camForward.sqrMagnitude < 0.0001f) {
				camForward = Vector3.ProjectOnPlane(_camera.transform.up, up);
			}

			camForward.Normalize();
			camRight.Normalize();

			var worldMove = camForward * input.y + camRight * input.x;
			worldMove = worldMove.normalized * Mathf.Clamp01(input.magnitude);

			var localForward = transform.InverseTransformDirection(worldMove);
			return new Vector2(localForward.x, localForward.z);
		}
		
		private void OnInteractPerformed(InputAction.CallbackContext context) => Interact?.Invoke();
		private void OnEnable() {
			_actions.Player.Enable();
			_actions.Player.Interact.performed += OnInteractPerformed;
		}
		private void OnDisable() {
			_actions.Player.Disable();
			_actions.Player.Interact.performed -= OnInteractPerformed;
		}
	}
}
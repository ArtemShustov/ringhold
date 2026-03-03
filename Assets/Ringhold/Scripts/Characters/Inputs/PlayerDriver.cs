using System;
using Ringhold.Inputs;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Ringhold.Characters.Inputs {
	public class PlayerDriver: ICharacterInput, IDisposable {
		private readonly Camera _camera;
		private readonly DefaultActions _actions;
		public InputPlayer Player { get; }

		public Vector2 Move => GetRelatedMove();
		public event Action Interact;

		public PlayerDriver(InputPlayer player) {
			_camera = Camera.main;
			Player = player;
			_actions = Player.Actions;
			Subscribe();
		}
		public void Dispose() {
			Disable();
			Unsubscribe();
		}
		
		public void Enable() => _actions.Player.Enable();
		public void Disable() => _actions.Player.Disable();

		private Vector2 GetRelatedMove() {
			if (_actions == null) {
				return Vector2.zero;
			}
    
			var input = _actions.Player.Move.ReadValue<Vector2>();
			if (input.sqrMagnitude < Mathf.Epsilon) {
				return Vector2.zero;
			}

			var up = _camera.transform.up;
			var camForward = Vector3.ProjectOnPlane(_camera.transform.forward, up);
			var camRight = Vector3.ProjectOnPlane(_camera.transform.right, up);

			if (camForward.sqrMagnitude < 0.0001f) {
				camForward = Vector3.ProjectOnPlane(_camera.transform.up, up);
			}

			camForward.Normalize();
			camRight.Normalize();

			var worldMove = camForward * input.y + camRight * input.x;
			worldMove = worldMove.normalized * Mathf.Clamp01(input.magnitude);

			var localForward = _camera.transform.InverseTransformDirection(worldMove);
			return new Vector2(localForward.x, localForward.z);
		}
		
		private void Subscribe() {
			_actions.Player.Interact.performed += OnInteractPerformed;
		}
		private void Unsubscribe() {
			_actions.Player.Interact.performed -= OnInteractPerformed;
		}
		
		private void OnInteractPerformed(InputAction.CallbackContext context) => Interact?.Invoke();
	}
}
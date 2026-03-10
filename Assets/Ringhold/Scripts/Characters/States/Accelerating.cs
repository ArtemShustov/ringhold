using Ringhold.Utils;
using UnityEngine;

namespace Ringhold.Characters.States {
	public class Accelerating: CharacterState {
		[SerializeField] private AnimationCurve _accelerationCurve = new AnimationCurve(
			new Keyframe(0f, 0f), 
			new Keyframe(1f, 1f)
		);
		[SerializeField] private float _duration = 0.5f;
		
		private float _progress;

		public override bool CheckTransition() {
			var internalCheck = base.CheckTransition();
			if (internalCheck) {
				return true;
			}

			if (_progress >= 1) {
				ParentMachine.Change<Move>();
				return true;
			}
			
			return false;
		}
		public override void OnUpdate() {
			_progress += Time.deltaTime / _duration;
			var input = Character.Input.Move.normalized;
			Character.Controller.LocalVelocity = new Vector3(input.x, 0, input.y) 
			                                     * (Character.Stats.MoveSpeed * _accelerationCurve.Evaluate(_progress));
		}

		public override void OnEnter(ICharacterState from) {
			var input = Character.Input.Move.normalized;
			var targetDirection = new Vector3(input.x, 0, input.y);

			var currentSpeedInTargetDirection = Vector3.Dot(
				Character.Controller.LocalVelocity,
				targetDirection.normalized
			);

			var normalizedProgress = Mathf.Clamp01(currentSpeedInTargetDirection / Character.Stats.MoveSpeed);
			_progress = _accelerationCurve.InverseSampleCurve(normalizedProgress); 
		}
	}
}
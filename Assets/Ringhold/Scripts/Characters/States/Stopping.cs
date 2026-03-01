using UnityEngine;

namespace Ringhold.Characters.States {
	public class Stopping: CharacterState {
		[SerializeField] private AnimationCurve _decelerationCurve = new AnimationCurve(
			new Keyframe(1f, 1f),
			new Keyframe(0f, 0f)
		);
		[SerializeField] private float _duration = 0.5f;

		private Vector3 _initialVelocity;
		private float _progress;

		public override bool CheckTransition() {
			var internalCheck = base.CheckTransition();
			if (internalCheck) {
				return true;
			}

			if (_progress >= 1) {
				ParentMachine.Change<Idle>();
				return true;
			}
			
			return false;
		}
		public override void OnUpdate() {
			_progress += Time.deltaTime / _duration;
			Character.Controller.LocalVelocity = _initialVelocity.normalized 
			                                     * (Character.Stats.MoveSpeed * _decelerationCurve.Evaluate(_progress));
		}

		public override void OnEnter(ICharacterState from) {
			_progress = 0;
			_initialVelocity = Character.Controller.LocalVelocity;
		}
	}
}
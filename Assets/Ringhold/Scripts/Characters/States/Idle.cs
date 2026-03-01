using UnityEngine;

namespace Ringhold.Characters.States {
	public class Idle: CharacterState {
		[SerializeField] private float _idleThreshold = Mathf.Epsilon;

		public override void OnEnter(ICharacterState from) {
			Character.View.PlayIdle();
			Character.Controller.Velocity = Vector3.zero;
		}
		
		public bool CheckIncomeTransition(ICharacterState current) {
			return Character.Input.Move.sqrMagnitude < _idleThreshold;
		}
	}
}
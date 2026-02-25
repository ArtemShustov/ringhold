using UnityEngine;

namespace Ringhold.Characters.States {
	public class Walk: CharacterState {
		public override bool CheckTransition() {
			if (Character.Input.Move.sqrMagnitude < Mathf.Epsilon) {
				ParentMachine.Change<Idle>();
				return true;
			}
			return false;
		}
		public override void OnUpdate() {
			var input = Character.Input.Move.normalized;
			var velocity = new Vector3(input.x, 0, input.y) * Character.Stats.MoveSpeed + Physics.gravity;
			Character.Controller.Move(velocity * Time.deltaTime);
		}

		public override void OnEnter(ICharacterState from) {
			Character.View.PlayWalk();
		}
	}
}
using UnityEngine;

namespace Ringhold.Characters.States {
	public class Idle: CharacterState {
		public override bool CheckTransition() {
			if (Character.Input.Move.sqrMagnitude >= Mathf.Epsilon) {
				ParentMachine.Change<Walk>();
				return true;
			}
			return false;
		}

		public override void OnEnter(ICharacterState from) {
			Character.View.PlayIdle();
		}
	}
}
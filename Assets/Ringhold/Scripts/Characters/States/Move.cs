using UnityEngine;

namespace Ringhold.Characters.States {
	public class Move: CharacterState {
		[SerializeField] private float _moveThreshold = Mathf.Epsilon;
		
		public override void OnUpdate() {
			var input = Character.Input.Move.normalized;
			Character.Controller.LocalVelocity = new Vector3(input.x, 0, input.y) * Character.Stats.MoveSpeed;
		}

		public override void OnEnter(ICharacterState from) {
			Character.View.PlayWalk();
		}
	}
}
using UnityEngine;

namespace Ringhold.Characters {
	[SelectionBase]
	public class Character: MonoBehaviour {
		[field: SerializeField] public CharacterBaseStats Stats { get; private set;}
		[field: SerializeField] public CharacterController Controller { get; private set; }
		[field: SerializeField] public CharacterView View { get; private set; }
		[field: SerializeField] public CharacterStateMachine StateMachine { get; private set; }
		[field: SerializeField] public CharacterInput Input { get; private set; }

		public void ChangeState<T>() where T: ICharacterState => StateMachine.Change<T>();
		public void ChangeState(ICharacterState state) => StateMachine.Change(state);
	}
}
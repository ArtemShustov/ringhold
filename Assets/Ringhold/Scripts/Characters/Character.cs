using UnityEngine;

namespace Ringhold.Characters {
	[SelectionBase]
	public class Character: MonoBehaviour {
		[field: SerializeField] public CharacterBaseStats Stats { get; private set;}
		[field: SerializeField] public CharacterMovementController Controller { get; private set; }
		[field: SerializeField] public CharacterView View { get; private set; }
		[field: SerializeField] public CharacterStateMachine StateMachine { get; private set; }
		[field: SerializeField] public CharacterInputContainer InputContainer { get; private set; }

		public ICharacterInput Input => InputContainer;

		public void ChangeState<T>() where T: ICharacterState => StateMachine.Change<T>();
		public void ChangeState(ICharacterState state) => StateMachine.Change(state);
	}
}
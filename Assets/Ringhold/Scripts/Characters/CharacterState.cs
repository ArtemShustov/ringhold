using UnityEngine;

namespace Ringhold.Characters {
	public interface ICharacterState {
		void Init(Character character, ICharacterStateMachine parent);
		
		void OnEnter(ICharacterState from);
		void OnExit(ICharacterState to);

		bool CheckTransition();
		void OnUpdate();
	}
	public abstract class CharacterState: MonoBehaviour, ICharacterState {
		public Character Character { get; private set; }
		public ICharacterStateMachine ParentMachine { get; private set; }
		
		public virtual void Init(Character character, ICharacterStateMachine parent) {
			Character = character;
			ParentMachine = parent;
		}

		public virtual void OnEnter(ICharacterState from) { }
		public virtual void OnExit(ICharacterState to) { }
		
		public virtual bool CheckTransition() => false;
		public virtual void OnUpdate() { }
	}
}
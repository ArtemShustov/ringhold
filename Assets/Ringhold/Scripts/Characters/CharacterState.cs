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
		[SerializeReference, SubclassSelector] private ITransition[] _transitions;
		
		public Character Character { get; private set; }
		public ICharacterStateMachine ParentMachine { get; private set; }
		
		public virtual void Init(Character character, ICharacterStateMachine parent) {
			Character = character;
			ParentMachine = parent;
		}

		public virtual void OnEnter(ICharacterState from) { }
		public virtual void OnExit(ICharacterState to) { }

		public virtual bool CheckTransition() {
			foreach (var transition in _transitions) {
				if (transition.Check(this)) {
					transition.Execute();
					return true;
				}
			}
			return false;
		}
		public virtual void OnUpdate() { }
	}
}
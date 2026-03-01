using System;
using System.Linq;
using UnityEngine;

namespace Ringhold.Characters.States {
	public class OnGround: CharacterState, ICharacterStateMachine {
		[SerializeField] private CharacterState[] _subStates;

		public ICharacterState Current { get; private set; }

		public override void Init(Character character, ICharacterStateMachine parent) {
			base.Init(character, parent);
			foreach (var state in _subStates) {
				state.Init(character, this);
			}
		}
		
		public void Change<T>() where T: ICharacterState {
			var state = _subStates.FirstOrDefault(s => s.GetType() == typeof(T));
			
			if (state == null) {
				ParentMachine.Change<T>();
				return;
			}
			Change(state);
		}
		public void Change(ICharacterState state) {
			if (!_subStates.Contains(state)) {
				ParentMachine.Change(state);
			}
			
			var previous = Current;
			Current?.OnExit(state);
			Current = state;
			Current?.OnEnter(previous);
		}

		public override bool CheckTransition() {
			return Current?.CheckTransition() ?? false;
		}
		public override void OnUpdate() {
			Current?.OnUpdate();
		}
		
		public override void OnEnter(ICharacterState from) {
			var state = _subStates.FirstOrDefault();
			Current = state;
			Current?.OnEnter(from);
		}
		public override void OnExit(ICharacterState to) {
			Current?.OnExit(to);
			Current = null;
		}
		
		[Serializable]
		public class ToIdleTransition: ITransition {
			public OnGround GroundState;

			public bool Check(ICharacterState current) {
				return GroundState.Character.Input.Move.sqrMagnitude < Mathf.Epsilon;
			}
			public void Execute() {
				GroundState.Change<Stopping>();
			}
		}
		[Serializable]
		public class ToMoveTransition: ITransition {
			public OnGround GroundState;
		
			public bool Check(ICharacterState current) {
				return GroundState.Character.Input.Move.sqrMagnitude >= Mathf.Epsilon;
			}
			public void Execute() {
				GroundState.Change<Accelerating>();
			}
		}
	}
}
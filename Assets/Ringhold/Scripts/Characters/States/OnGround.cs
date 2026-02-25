using System;
using UnityEngine;

namespace Ringhold.Characters.States {
	public class OnGround: CharacterState, ICharacterStateMachine {
		[SerializeField] private float _walkThreshold = Mathf.Epsilon;
		[Space]
		[SerializeField] private Idle _idle;
		[SerializeField] private Walk _walk;

		private ICharacterState _current;

		public override void Init(Character character, ICharacterStateMachine parent) {
			base.Init(character, parent);
			_idle.Init(character, this);
			_walk.Init(character, this);
		}
		
		public void Change<T>() where T: ICharacterState {
			var type = typeof(T);
			ICharacterState state = type switch {
				_ when type == typeof(Idle) => _idle,
				_ when type == typeof(Walk) => _walk,
				_ => throw new ArgumentException($"State {typeof(T)} is not handled by {nameof(OnGround)}")
			};
			Change(state);
		}
		public void Change(ICharacterState state) {
			var previous = _current;
			_current?.OnExit(state);
			_current = state;
			_current?.OnEnter(previous);
		}

		public override bool CheckTransition() {
			var nextState = SelectInternalState();
			if (_current != nextState) {
				Change(nextState);
			}

			return false;
		}
		public override void OnUpdate() {
			_current?.OnUpdate();
		}

		private ICharacterState SelectInternalState() {
			var input = Character.Input.Move.magnitude;
			ICharacterState nextState = input >= _walkThreshold ? _walk : _idle;
			return nextState;
		}
		
		public override void OnEnter(ICharacterState from) {
			var state = SelectInternalState();
			_current = state;
			_current?.OnEnter(from);
		}
		public override void OnExit(ICharacterState to) {
			_current?.OnExit(to);
			_current = null;
		}
	}
}
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ringhold.Characters {
	public interface ICharacterStateMachine {
		void Change<T>() where T: ICharacterState;
		void Change(ICharacterState state);
	}
	[RequireComponent(typeof(Character))]
	public class CharacterStateMachine: MonoBehaviour, ICharacterStateMachine {
		[SerializeField] private CharacterState[] _topStates;
		
		private readonly Dictionary<Type, ICharacterState> _states = new Dictionary<Type, ICharacterState>();
		private ICharacterState _currentState;
		private Character _character;

		private void Awake() {
			_character = GetComponent<Character>();
			
			foreach (var state in _topStates) {
				_states.Add(state.GetType(), state);
				state.Init(_character, this);
			}
		}
		private void Start() {
			if (_topStates.Length > 0) {
				Change(_topStates[0]);
			}
		}

		public void Change<T>() where T: ICharacterState {
			Change(_states[typeof(T)]);
		}
		public void Change(ICharacterState state) {
			var previous = _currentState;
			_currentState?.OnExit(state);
			_currentState = state;
			_currentState.OnEnter(previous);
		}

		private void Update() {
			_currentState?.CheckTransition();
			_currentState?.OnUpdate();
		}
	}
}
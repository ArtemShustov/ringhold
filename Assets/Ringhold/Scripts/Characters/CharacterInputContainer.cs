using System;
using UnityEngine;

namespace Ringhold.Characters {
	public class CharacterInputContainer: MonoBehaviour, ICharacterInput {
		[SerializeField] private GameObject _default;

		private ICharacterInput _defaultDriver;
		private ICharacterInput _current;
		private bool _subscribed = false;

		public Vector2 Move => _current?.Move ?? Vector2.zero;
		public event Action Interact;

		private void Awake() {
			if (_defaultDriver != null) {
				_defaultDriver = _default.GetComponent<ICharacterInput>();
				if (_defaultDriver == null) {
					Debug.LogWarning("Default input driver not found.");
				}
				_current = _defaultDriver;
			}
		}

		public void Select(ICharacterInput input) {
			UnsubscribeAll();
			_current = input;
			SubscribeAll();
		}
		public void SelectDefault() {
			Select(_defaultDriver);
		}
		
		private void SubscribeAll() {
			if (_subscribed || _current == null) {
				return;
			}
			_subscribed = true;
			_current.Interact += OnInteract;
		}
		private void UnsubscribeAll() {
			if (!_subscribed || _current == null) {
				return;
			}
			_subscribed = false;
			_current.Interact -= OnInteract;
		}
		
		private void OnInteract() => Interact?.Invoke();

		private void OnEnable() {
			SubscribeAll();
		}
		private void OnDisable() {
			UnsubscribeAll();
		}
	}
}
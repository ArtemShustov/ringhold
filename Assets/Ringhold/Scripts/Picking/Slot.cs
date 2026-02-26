using Ringhold.Interactions;
using UnityEngine;
using UnityEngine.Events;

namespace Ringhold.Picking {
	public class Slot: MonoBehaviour, IInteraction {
		[SerializeField] private Transform _root;
		[field: SerializeField] public UnityEvent<InteractionHighlightState> StateChanged { get; private set; }

		private IPickupable _current;
		
		public void Interact(InteractionContext context) {
			if (!CanInteract(context)) {
				return;
			}
			if (_current == null) {
				_current = context.Hand.Clear(); ;
				PickVisual();
			} else {
				context.Hand.Pick(_current);
				_current = null;
			}
		}
		public bool CanInteract(InteractionContext context) {
			return (context.Hand.Current != null && _current == null)
				|| (context.Hand.Current == null && _current != null);
		}
		public void SetInteractionState(InteractionHighlightState state) {
			StateChanged?.Invoke(state);
		}
		
		private void PickVisual() {
			if (_current is MonoBehaviour mono) {
				var item = mono.transform;
				item.SetParent(_root);
				item.localPosition = Vector3.zero;
				item.localRotation = Quaternion.identity;
			}
		}
	}
}
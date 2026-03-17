using Ringhold.Interactions;
using UnityEngine;
using UnityEngine.Events;

namespace Ringhold.Picking {
	public class Pickupable: MonoBehaviour, IPickupable, IInteraction {
		[field: Header("Settings")]
		[field: SerializeField] public InteractionPriority Priority { get; set; } = InteractionPriority.Normal;
		
		[field: Header("Events")]
		[field: SerializeField] public UnityEvent Picked { get; private set; }
		[field: SerializeField] public UnityEvent Dropped { get; private set; }
		[field: SerializeField] public UnityEvent<InteractionHighlightState> StateChanged { get; private set; }
		
		public virtual void Interact(InteractionContext context) {
			if (!CanInteract(context)) {
				return;
			}
			context.Hand.Pick(this);
		}
		public virtual bool CanInteract(InteractionContext context) {
			return context.Hand.Current == null;
		}
		public virtual void SetInteractionState(InteractionHighlightState state) {
			StateChanged?.Invoke(state);
		}
		
		public void OnPickup() {
			Picked?.Invoke();
		}
		public void OnDrop() {
			Dropped?.Invoke();
		}
	}
}
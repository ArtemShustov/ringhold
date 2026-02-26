namespace Ringhold.Interactions {
	public interface IInteraction {
		void Interact(InteractionContext context);
		bool CanInteract(InteractionContext context);
		
		void SetInteractionState(InteractionHighlightState state);
	}

	public enum InteractionHighlightState {
		None, Visible, Selected
	}
}
namespace Ringhold.Interactions {
	public interface IInteraction {
		InteractionPriority Priority => InteractionPriority.Normal;
		
		void Interact(InteractionContext context);
		bool CanInteract(InteractionContext context);
		
		void SetInteractionState(InteractionHighlightState state);
	}

	public enum InteractionHighlightState {
		None, Visible, Selected
	}
	
	public enum InteractionPriority {
		Lowest, Low, Normal, High, Critical
	}
}
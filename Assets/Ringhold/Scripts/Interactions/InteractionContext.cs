using System;
using Ringhold.Characters;
using Ringhold.Picking;

namespace Ringhold.Interactions {
	[Serializable]
	public struct InteractionContext {
		public Character Character;
		public Hand Hand;

		public InteractionContext(Character character, Hand hand) {
			Character = character;
			Hand = hand;
		}
	}
}
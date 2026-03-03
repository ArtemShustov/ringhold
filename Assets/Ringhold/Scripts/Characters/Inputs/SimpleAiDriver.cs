using System;
using UnityEngine;

namespace Ringhold.Characters.Inputs {
	public class SimpleAiDriver: MonoBehaviour, ICharacterInput {
		public Vector2 Move => Vector2.zero;
		public event Action Interact;
	}
}
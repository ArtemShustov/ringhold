using System;
using UnityEngine;

namespace Ringhold.Characters {
	public interface ICharacterInput {
		public Vector2 Move { get; }
		
		public event Action Interact;
	}
}
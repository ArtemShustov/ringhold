using System;
using UnityEngine;

namespace Ringhold.Characters {
	[Serializable]
	public class CharacterBaseStats {
		[field: SerializeField] public float MoveSpeed { get; private set; } = 5;
	}
}
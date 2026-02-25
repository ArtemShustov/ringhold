using UnityEngine;

namespace Ringhold.Characters {
	public class CharacterView: MonoBehaviour {
		[SerializeField] private Animator _animator;
		
		public void PlayIdle(float fixedTransitionDuration = 0.1f) => _animator.CrossFadeInFixedTime("Idle", fixedTransitionDuration);
		public void PlayWalk(float fixedTransitionDuration = 0.1f) => _animator.CrossFadeInFixedTime("Walk", fixedTransitionDuration);
		public void PlayFall(float fixedTransitionDuration = 0.1f) => _animator.CrossFadeInFixedTime("Fall", fixedTransitionDuration);
	}
}
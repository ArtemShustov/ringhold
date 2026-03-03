using System;
using UnityEngine;
using UnityEngine.AI;

namespace Ringhold.Characters.Inputs {
	public class FollowAiDriver: MonoBehaviour, ICharacterInput {
		[SerializeField] private Transform _followTarget;
		[SerializeField] private NavMeshAgent _agent;
		[SerializeField] private float _minDist = 2f;
		[SerializeField] private float _maxDist = 5f;
       
		public Vector2 Move { get; private set; }
		public event Action Interact;

		private bool _isFollowing = false;

		private void Awake() {
			_agent.updatePosition = false;
			_agent.updateRotation = false;
			_agent.isStopped = false;
			_agent.stoppingDistance = _minDist;
		}

		private void Update() {
			if (_followTarget == null) {
				Move = Vector2.zero;
				return;
			}

			float distanceToTarget = Vector3.Distance(transform.position, _followTarget.position);

			if (distanceToTarget >= _maxDist) {
				_isFollowing = true;
			}

			if (distanceToTarget <= _minDist) {
				_isFollowing = false;
			}

			_agent.nextPosition = transform.position;

			if (_isFollowing) {
				_agent.isStopped = false;
				_agent.SetDestination(_followTarget.position);

				var desiredVelocity = transform.InverseTransformDirection(_agent.desiredVelocity);
				Move = new Vector2(desiredVelocity.x, desiredVelocity.z).normalized;
			} else {
				_agent.isStopped = true;
				Move = Vector2.zero;
			}
		}
	}
}
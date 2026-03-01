using Core.DependencyInjection;
using KinematicCharacterController;
using Ringhold.World;
using UnityEngine;

namespace Ringhold.Characters {
	public class CharacterMovementController: MonoBehaviour, ICharacterController {
		[SerializeField] private KinematicCharacterMotor _motor;
		[Inject] private WorldRoot _world;

		public Vector3 Velocity { get; set; }
		public Vector3 LocalVelocity {
			get => transform.InverseTransformDirection(Velocity);
			set => Velocity = transform.TransformDirection(value);
		}
		
		private void Awake() {
			_motor.CharacterController = this;
		}

		public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime) {
			var walkable = _world.WalkableRadius;
			var center = _world.Center;
    
			var toPlayer = transform.position - center;
			var flatOffset = new Vector3(toPlayer.x, 0f, toPlayer.z);
    
			currentVelocity = Velocity;
    
			var nextPosition = transform.position + currentVelocity * deltaTime;
			var nextOffset = new Vector3(nextPosition.x - center.x, 0f, nextPosition.z - center.z);
			var nextDistance = nextOffset.magnitude;
    
			if (nextDistance < walkable.x) {
				var pushDir = nextOffset.sqrMagnitude > 0.0001f 
					? nextOffset.normalized 
					: flatOffset.normalized;
				currentVelocity += pushDir * ((walkable.x - nextDistance) / deltaTime);
			} else if (nextDistance > walkable.y) {
				var clampedPos = center + nextOffset.normalized * walkable.y;
				clampedPos.y = nextPosition.y;
				var correction = (clampedPos - nextPosition) / deltaTime;
				currentVelocity += new Vector3(correction.x, 0f, correction.z);
			}
    
			if (!_motor.GroundingStatus.IsStableOnGround) {
				currentVelocity += Physics.gravity;
			}
		}
		public void UpdateRotation(ref Quaternion currentRotation, float deltaTime) {
			var center = _world.Center;
			center.y = transform.position.y;
			var toCenter = (center - transform.position).normalized;
			
			currentRotation = Quaternion.LookRotation(toCenter, Vector3.up);
		}

		public void BeforeCharacterUpdate(float deltaTime) { }
		public void PostGroundingUpdate(float deltaTime) { }
		public void AfterCharacterUpdate(float deltaTime) { }
		public bool IsColliderValidForCollisions(Collider coll) => true;
		public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport) { }
		public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport) { }
		public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport) { }
		public void OnDiscreteCollisionDetected(Collider hitCollider) { }
	}
}
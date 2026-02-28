using KinematicCharacterController;
using Ringhold.World;
using UnityEngine;

namespace Ringhold.Characters {
    public class CharacterMovementController: MonoBehaviour, ICharacterController {
        [SerializeField] private KinematicCharacterMotor _motor;
        [SerializeField] private float _normalSmoothSpeed = 10f;
        [SerializeField] private float _normalSnapThreshold = 0.1f;

        public Vector3 Velocity { get; set; }

        private Vector3 _smoothedNormal = Vector3.up;

        private void Awake() {
            _motor.CharacterController = this;
        }

        public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime) {
            currentVelocity = Velocity; 
        }
        public void UpdateRotation(ref Quaternion currentRotation, float deltaTime) {
            var targetNormal = _motor.GroundingStatus.FoundAnyGround ? _motor.GroundingStatus.GroundNormal : -WorldRoot.GetGravityAt(transform.position).normalized;
            _smoothedNormal = Vector3.Slerp(_smoothedNormal, targetNormal, _normalSmoothSpeed * deltaTime);
            currentRotation = Quaternion.LookRotation(Vector3.forward, _smoothedNormal);
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
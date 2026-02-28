using KinematicCharacterController;
using Ringhold.World;
using UnityEngine;

namespace Ringhold.Characters {
    public class CharacterMovementController: MonoBehaviour, ICharacterController {
        [SerializeField] private KinematicCharacterMotor _motor;
        [SerializeField] private float _orientationSharpness = 10f;

        public Vector3 Velocity { get; set; }

        private void Awake() {
            _motor.CharacterController = this;
        }

        public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime) {
            currentVelocity = Velocity;
        }
        public void UpdateRotation(ref Quaternion currentRotation, float deltaTime) {
            var gravity = WorldRoot.GetGravityAt(transform.position);
            var currentUp = currentRotation * Vector3.up;

            if (_motor.GroundingStatus.IsStableOnGround) {
                var initialCharacterBottomHemiCenter = _motor.TransientPosition + (currentUp * _motor.Capsule.radius);

                var smoothedGroundNormal = Vector3.Slerp(_motor.CharacterUp, _motor.GroundingStatus.GroundNormal, 1f - Mathf.Exp(-_orientationSharpness * deltaTime));
                currentRotation = Quaternion.FromToRotation(currentUp, smoothedGroundNormal) * currentRotation;

                _motor.SetTransientPosition(initialCharacterBottomHemiCenter + (currentRotation * Vector3.down * _motor.Capsule.radius));
            } else {
                var smoothedGravityDir = Vector3.Slerp(currentUp, -gravity.normalized, 1f - Mathf.Exp(-_orientationSharpness * deltaTime));
                currentRotation = Quaternion.FromToRotation(currentUp, smoothedGravityDir) * currentRotation;
            }
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
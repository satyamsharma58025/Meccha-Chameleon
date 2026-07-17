using System.Collections.Generic;
using HueSeek.Core;
using HueSeek.Paint;
using HueSeek.Taunt;
using UnityEngine;

namespace HueSeek.Player
{
    /// <summary>
    /// Controls the Clayling mascot — original clay-golem design, unpainted at round start.
    /// </summary>
    [RequireComponent(typeof(PaintSystem))]
    public class ClaylingController : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed = 4f;
        [SerializeField] private CharacterController _controller;
        [SerializeField] private PaintSystem _paintSystem;
        [SerializeField] private DetectionRiskTracker _riskTracker;
        [SerializeField] private TauntSystem _tauntSystem;

        // Static registry to avoid O(n²) FindObjectsByType calls
        public static List<ClaylingController> ActiveSeekers { get; } = new();

        public PlayerRole Role { get; private set; } = PlayerRole.None;
        public PlayerPose CurrentPose { get; private set; } = PlayerPose.Stand;
        public int PlayerId { get; set; }
        public bool IsLocked { get; private set; }

        public PaintSystem Paint => _paintSystem;

        public void Initialize(CharacterController controller, PaintSystem paintSystem, DetectionRiskTracker riskTracker, TauntSystem tauntSystem)
        {
            _controller = controller;
            _paintSystem = paintSystem;
            _riskTracker = riskTracker;
            _tauntSystem = tauntSystem;
        }

        public void AssignRole(PlayerRole role)
        {
            // Update seeker registry
            if (Role == PlayerRole.Seeker)
                ActiveSeekers.Remove(this);
            
            Role = role;
            IsLocked = role == PlayerRole.Seeker;
            
            if (role == PlayerRole.Seeker)
                ActiveSeekers.Add(this);
        }

        public void SetLocked(bool locked) => IsLocked = locked;

        public void Move(Vector2 input, float deltaTime)
        {
            if (IsLocked || Role == PlayerRole.Spectator) return;

            var motion = new Vector3(input.x, 0f, input.y) * _moveSpeed * deltaTime;
            _controller?.Move(motion);

            if (motion.sqrMagnitude > 0.0001f)
                _riskTracker?.RegisterMovement();
        }

        public void SetPose(PlayerPose pose)
        {
            if (CurrentPose == pose) return;
            CurrentPose = pose;
            _riskTracker?.RegisterPoseChange();
            // Animation layer drives pose blend shapes / rig states.
        }

        public void SuggestPoseFromProximity(Collider nearbyGeometry)
        {
            if (nearbyGeometry == null) return;

            var bounds = nearbyGeometry.bounds;
            var verticalExtent = bounds.size.y;
            var isWallLike = bounds.size.y > bounds.size.x * 2f;
            var suggestedPose = isWallLike
                ? PlayerPose.ClingWall
                : verticalExtent < 0.5f
                    ? PlayerPose.LieFlat
                    : bounds.size.y > 1.5f && bounds.size.z < 0.8f
                        ? PlayerPose.Sit
                        : PlayerPose.Crouch;

            SetPose(suggestedPose);
        }

        public void ResetForRound()
        {
            CurrentPose = PlayerPose.Stand;
            _paintSystem.ResetPaintState();
            IsLocked = false;
        }
    }
}

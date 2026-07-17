using HueSeek.Core;
using UnityEngine;
using UnityEngine.Events;

namespace HueSeek.Player
{
    /// <summary>
    /// Tracks movement/repaint/pose tells when Seekers have line-of-sight.
    /// </summary>
    public class DetectionRiskTracker : MonoBehaviour
    {
        [SerializeField] private float _seekerSightRange = 15f;
        [SerializeField] private LayerMask _obstructionMask = ~0;

        public UnityEvent<float> OnShimmerTriggered = new();

        private float _lastChangeMagnitude;

        public void RegisterMovement() => RegisterChange(0.4f);
        public void RegisterRepaint(float pressure) => RegisterChange(0.3f + pressure * 0.5f);
        public void RegisterPoseChange() => RegisterChange(0.6f);

        private void RegisterChange(float magnitude)
        {
            _lastChangeMagnitude = magnitude;
            if (AnySeekerHasLineOfSight())
                TriggerShimmer(magnitude);
        }

        private bool AnySeekerHasLineOfSight()
        {
            var origin = transform.position + Vector3.up * 0.5f;
            var thisPlayerId = GetComponent<ClaylingController>()?.PlayerId ?? 0;

            // Use cached seeker list instead of FindObjectsByType O(n²)
            foreach (var seeker in ClaylingController.ActiveSeekers)
            {
                if (seeker.PlayerId == thisPlayerId)
                    continue;

                var target = seeker.transform.position + Vector3.up * 0.5f;
                var dir = target - origin;
                if (dir.magnitude > _seekerSightRange) continue;
                if (!Physics.Raycast(origin, dir.normalized, out _, dir.magnitude, _obstructionMask))
                    return true;
            }

            return false;
        }

        private void TriggerShimmer(float magnitude)
        {
            OnShimmerTriggered?.Invoke(magnitude * GameConstants.DetectionShimmerDuration);
        }
    }
}

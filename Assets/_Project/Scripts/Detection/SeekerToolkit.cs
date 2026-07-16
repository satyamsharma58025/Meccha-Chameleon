using HueSeek.Core;
using HueSeek.Player;
using HueSeek.Scoring;
using UnityEngine;
using UnityEngine.Events;

namespace HueSeek.Detection
{
    /// <summary>
    /// Seeker inspect + tag actions. No default flashlight/UV — observation-based detection.
    /// </summary>
    public class SeekerToolkit : MonoBehaviour
    {
        [SerializeField] private float _inspectRange = 3f;
        [SerializeField] private float _tagRange = 1.5f;
        [SerializeField] private LayerMask _interactableMask = ~0;
        [SerializeField] private MatchScorer _scorer;

        public UnityEvent<ClaylingController> OnTagged = new();
        public UnityEvent<Vector3> OnInspectStarted = new();

        private ClaylingController _localSeeker;

        public void BindSeeker(ClaylingController seeker) => _localSeeker = seeker;

        public bool TryInspect(Ray ray)
        {
            if (_localSeeker == null || _localSeeker.Role != PlayerRole.Seeker) return false;
            if (!Physics.Raycast(ray, out var hit, _inspectRange, _interactableMask)) return false;

            OnInspectStarted?.Invoke(hit.point);
            return true;
        }

        public bool TryTag(Ray ray)
        {
            if (_localSeeker == null || _localSeeker.Role != PlayerRole.Seeker) return false;
            if (!Physics.Raycast(ray, out var hit, _tagRange, _interactableMask)) return false;

            var hider = hit.collider.GetComponentInParent<ClaylingController>();
            if (hider == null || hider.Role != PlayerRole.Hider) return false;

            TagHider(hider);
            return true;
        }

        private void TagHider(ClaylingController hider)
        {
            hider.AssignRole(PlayerRole.Spectator);
            _scorer?.RecordTag(_localSeeker.PlayerId, hider.PlayerId);
            OnTagged?.Invoke(hider);
        }
    }
}

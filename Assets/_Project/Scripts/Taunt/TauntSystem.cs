using HueSeek.Player;
using HueSeek.Scoring;
using UnityEngine;
using UnityEngine.Events;

namespace HueSeek.Taunt
{
    public class TauntSystem : MonoBehaviour
    {
        [SerializeField] private MatchScorer _scorer;
        [SerializeField] private ClaylingController _owner;

        public UnityEvent OnTauntPerformed = new();

        public void Initialize(MatchScorer scorer, ClaylingController owner)
        {
            _scorer = scorer;
            _owner = owner;
        }

        public void PerformTaunt()
        {
            if (_owner == null || _owner.Role != PlayerRole.Hider) return;

            _scorer?.RecordTaunt(_owner.PlayerId);
            OnTauntPerformed?.Invoke();
        }
    }
}

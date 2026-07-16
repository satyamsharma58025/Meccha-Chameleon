using System.Collections.Generic;
using UnityEngine;

namespace HueSeek.Scoring
{
    public class MatchScorer : MonoBehaviour
    {
        private readonly Dictionary<int, int> _scores = new();
        private readonly HashSet<int> _recentTaunters = new();

        public void RecordSurvival(int playerId, float seconds) =>
            AddScore(playerId, Mathf.RoundToInt(seconds));

        public void RecordCloseCall(int playerId) => AddScore(playerId, 25);

        public void RecordTaunt(int playerId)
        {
            _recentTaunters.Add(playerId);
            AddScore(playerId, 50);
        }

        public void RecordTag(int seekerId, int hiderId)
        {
            var bonus = _recentTaunters.Contains(hiderId) ? 30 : 0;
            AddScore(seekerId, 100 + bonus);
            _recentTaunters.Remove(hiderId);
        }

        public void RecordCleverCamouflage(int playerId, int votes) =>
            AddScore(playerId, votes * 20);

        public IReadOnlyDictionary<int, int> GetScores() => _scores;

        public void Reset()
        {
            _scores.Clear();
            _recentTaunters.Clear();
        }

        private void AddScore(int playerId, int points)
        {
            _scores.TryGetValue(playerId, out var current);
            _scores[playerId] = current + points;
        }
    }
}

using System;
using UnityEngine;
using UnityEngine.Events;

namespace HueSeek.Core
{
    /// <summary>
    /// Authoritative round flow: LOBBY → ROLE → PREP → HUNT → END → REVEAL → REWARDS.
    /// Host/server advances phases; clients mirror via network events.
    /// </summary>
    public class RoundStateMachine : MonoBehaviour
    {
        [SerializeField] private RoundPhase _currentPhase = RoundPhase.Lobby;
        [SerializeField] private float _phaseTimer;
        [SerializeField] private float _phaseDuration;

        public RoundPhase CurrentPhase => _currentPhase;
        public float PhaseTimeRemaining => Mathf.Max(0f, _phaseDuration - _phaseTimer);
        public float PhaseProgress => _phaseDuration > 0f ? Mathf.Clamp01(_phaseTimer / _phaseDuration) : 1f;

        public UnityEvent<RoundPhase> OnPhaseEntered = new();
        public UnityEvent<RoundPhase> OnPhaseExited = new();

        public void EnterPhase(RoundPhase phase, float durationSeconds = 0f)
        {
            if (_currentPhase == phase) return;

            OnPhaseExited?.Invoke(_currentPhase);
            _currentPhase = phase;
            _phaseTimer = 0f;
            _phaseDuration = durationSeconds;
            OnPhaseEntered?.Invoke(_currentPhase);
        }

        private void Update()
        {
            if (_phaseDuration <= 0f) return;

            _phaseTimer += Time.deltaTime;
            if (_phaseTimer >= _phaseDuration)
            {
                AdvanceFromTimedPhase();
            }
        }

        private void AdvanceFromTimedPhase()
        {
            switch (_currentPhase)
            {
                case RoundPhase.Prep:
                    EnterPhase(RoundPhase.Hunt, GameConstants.HuntPhaseMinSeconds);
                    break;
                case RoundPhase.Hunt:
                    EnterPhase(RoundPhase.RoundEnd);
                    break;
                default:
                    break;
            }
        }

        public void ForceEndHunt()
        {
            if (_currentPhase == RoundPhase.Hunt)
            {
                EnterPhase(RoundPhase.RoundEnd);
            }
        }

        public void BeginPrepPhase(float durationSeconds)
        {
            EnterPhase(RoundPhase.Prep, durationSeconds);
        }

        public void BeginResultsReveal()
        {
            EnterPhase(RoundPhase.ResultsReveal);
        }

        public void BeginRewards()
        {
            EnterPhase(RoundPhase.Rewards);
        }
    }
}

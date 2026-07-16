using System.Collections.Generic;
using System.Linq;
using HueSeek.Core;
using HueSeek.Modes;
using HueSeek.Networking;
using HueSeek.Player;
using UnityEngine;

namespace HueSeek.Core
{
    /// <summary>
    /// Top-level match orchestrator binding round flow, mode rules, roles, and win conditions.
    /// </summary>
    public class MatchOrchestrator : MonoBehaviour
    {
        [SerializeField] private RoundStateMachine _stateMachine;
        [SerializeField] private GameModeType _modeType = GameModeType.Classic;
        [SerializeField] private float _mapScale = 1f;
        [SerializeField] private string _mapId = "sunroom_greenhouse";

        private IGameModeRules _modeRules;
        private readonly List<ClaylingController> _players = new();
        private int _tradePlacesRoundIndex;

        public string MapId => _mapId;
        public GameModeType ModeType => _modeType;

        private void Awake()
        {
            _modeRules = CreateModeRules(_modeType);
            if (_stateMachine != null)
                _stateMachine.OnPhaseEntered.AddListener(HandlePhaseEntered);
        }

        public void Initialize(RoundStateMachine stateMachine, GameModeType modeType)
        {
            _stateMachine = stateMachine;
            _modeType = modeType;
            _modeRules = CreateModeRules(_modeType);
            _stateMachine.OnPhaseEntered.AddListener(HandlePhaseEntered);
        }

        public void RegisterPlayer(ClaylingController player)
        {
            if (!_players.Contains(player))
                _players.Add(player);
        }

        public void StartMatch(IReadOnlyList<int> playerIds)
        {
            _modeRules = CreateModeRules(_modeType);
            var assignments = _modeRules.AssignRoles(playerIds, _tradePlacesRoundIndex);

            foreach (var assignment in assignments)
            {
                var player = _players.FirstOrDefault(p => p.PlayerId == assignment.PlayerId);
                player?.AssignRole(assignment.Role);
                player?.ResetForRound();
            }

            LockSeekersDuringPrep();

            var prepDuration = _modeRules.GetPrepDurationSeconds(playerIds.Count, _mapScale);
            _stateMachine.BeginPrepPhase(prepDuration);
        }

        private void LockSeekersDuringPrep()
        {
            foreach (var player in _players)
            {
                if (player.Role == PlayerRole.Seeker)
                    player.SetLocked(true);
                else
                    player.SetLocked(false);
            }
        }

        public void OnPrepPhaseEnded()
        {
            foreach (var player in _players)
                player.SetLocked(false);

            var huntDuration = _modeRules.GetHuntDurationSeconds(_players.Count);
            _stateMachine.EnterPhase(RoundPhase.Hunt, huntDuration);
        }

        public void OnHiderTagged(ClaylingController hider)
        {
            var role = hider.Role;
            _modeRules.OnHiderTagged(ref role, null);
            hider.AssignRole(role);

            EvaluateWinCondition();
        }

        public void OnHuntTimerExpired() => EvaluateWinCondition(endByTimer: true);

        private void HandlePhaseEntered(RoundPhase phase)
        {
            if (phase == RoundPhase.Hunt)
                OnPrepPhaseEnded();
        }

        private void EvaluateWinCondition(bool endByTimer = false)
        {
            var hiders = _players.Where(p => p.Role == PlayerRole.Hider).ToList();
            var surviving = hiders.Count;
            var totalHiders = _players.Count(p => p.Role != PlayerRole.Seeker && p.Role != PlayerRole.Spectator) + surviving;
            var timeRemaining = endByTimer ? 0f : _stateMachine.PhaseTimeRemaining;

            var hidersWin = _modeRules.CheckHidersWin(surviving, timeRemaining);
            var seekersWin = _modeRules.CheckSeekersWin(surviving, totalHiders);

            if (!hidersWin && !seekersWin) return;

            _stateMachine.ForceEndHunt();
            _stateMachine.BeginResultsReveal();

            if (_modeType == GameModeType.TradePlaces && _tradePlacesRoundIndex == 0)
            {
                _tradePlacesRoundIndex++;
                StartMatch(_players.Select(p => p.PlayerId).ToList());
                return;
            }

            _stateMachine.BeginRewards();
        }

        private static IGameModeRules CreateModeRules(GameModeType type) => type switch
        {
            GameModeType.Overrun => new OverrunModeRules(),
            GameModeType.TradePlaces => new TradePlacesModeRules(),
            GameModeType.LoneWolf => new LoneWolfModeRules(),
            _ => new ClassicModeRules()
        };
    }
}

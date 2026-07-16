using System;
using System.Collections.Generic;
using System.Linq;
using HueSeek.Core;
using HueSeek.Player;

namespace HueSeek.Modes
{
    public class ClassicModeRules : IGameModeRules
    {
        private const int MinSeekers = 1;
        private const int MaxSeekers = 3;

        public GameModeType ModeType => GameModeType.Classic;

        public float GetPrepDurationSeconds(int playerCount, float mapScale) =>
            Math.Clamp(GameConstants.PrepPhaseMinSeconds * mapScale, GameConstants.PrepPhaseMinSeconds, GameConstants.PrepPhaseMaxSeconds);

        public float GetHuntDurationSeconds(int playerCount) =>
            Math.Clamp(GameConstants.HuntPhaseMinSeconds + playerCount * 5f, GameConstants.HuntPhaseMinSeconds, GameConstants.HuntPhaseMaxSeconds);

        public IReadOnlyList<RoleAssignment> AssignRoles(IReadOnlyList<int> playerIds, int roundIndex = 0)
        {
            var seekerCount = Math.Clamp(playerIds.Count / 4, MinSeekers, MaxSeekers);
            var shuffled = playerIds.OrderBy(_ => Guid.NewGuid()).ToList();

            return shuffled.Select((id, i) => new RoleAssignment
            {
                PlayerId = id,
                Role = i < seekerCount ? PlayerRole.Seeker : PlayerRole.Hider
            }).ToList();
        }

        public bool CheckHidersWin(int survivingHiders, float timeRemaining) =>
            survivingHiders > 0 && timeRemaining <= 0f;

        public bool CheckSeekersWin(int survivingHiders, int totalHiders) =>
            survivingHiders <= 0;

        public void OnHiderTagged(ref PlayerRole taggedRole, IReadOnlyList<RoleAssignment> currentRoles) =>
            taggedRole = PlayerRole.Spectator;
    }
}

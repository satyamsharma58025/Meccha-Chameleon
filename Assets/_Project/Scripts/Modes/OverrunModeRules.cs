using System;
using System.Collections.Generic;
using System.Linq;
using HueSeek.Core;
using HueSeek.Player;

namespace HueSeek.Modes
{
    public class OverrunModeRules : IGameModeRules
    {
        public GameModeType ModeType => GameModeType.Overrun;

        public float GetPrepDurationSeconds(int playerCount, float mapScale) =>
            Math.Clamp(60f * mapScale, GameConstants.PrepPhaseMinSeconds, GameConstants.PrepPhaseMaxSeconds);

        public float GetHuntDurationSeconds(int playerCount) =>
            Math.Clamp(240f + playerCount * 4f, GameConstants.HuntPhaseMinSeconds, GameConstants.HuntPhaseMaxSeconds);

        public IReadOnlyList<RoleAssignment> AssignRoles(IReadOnlyList<int> playerIds, int roundIndex = 0)
        {
            if (playerIds.Count == 0) return Array.Empty<RoleAssignment>();

            var first = playerIds[Random.Shared.Next(playerIds.Count)];
            return playerIds.Select(id => new RoleAssignment
            {
                PlayerId = id,
                Role = id == first ? PlayerRole.Seeker : PlayerRole.Hider
            }).ToList();
        }

        public bool CheckHidersWin(int survivingHiders, float timeRemaining) =>
            survivingHiders > 0 && timeRemaining <= 0f;

        public bool CheckSeekersWin(int survivingHiders, int totalHiders) =>
            survivingHiders <= 0;

        public void OnHiderTagged(ref PlayerRole taggedRole, IReadOnlyList<RoleAssignment> currentRoles) =>
            taggedRole = PlayerRole.Seeker;
    }
}

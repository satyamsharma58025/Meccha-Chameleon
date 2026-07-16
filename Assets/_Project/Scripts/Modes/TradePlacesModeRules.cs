using System;
using System.Collections.Generic;
using System.Linq;
using HueSeek.Core;
using HueSeek.Player;

namespace HueSeek.Modes
{
    public class TradePlacesModeRules : IGameModeRules
    {
        public GameModeType ModeType => GameModeType.TradePlaces;

        public float GetPrepDurationSeconds(int playerCount, float mapScale) =>
            Math.Clamp(75f * mapScale, GameConstants.PrepPhaseMinSeconds, GameConstants.PrepPhaseMaxSeconds);

        public float GetHuntDurationSeconds(int playerCount) =>
            Math.Clamp(150f, GameConstants.HuntPhaseMinSeconds, GameConstants.HuntPhaseMaxSeconds);

        public IReadOnlyList<RoleAssignment> AssignRoles(IReadOnlyList<int> playerIds, int roundIndex = 0)
        {
            var half = playerIds.Count / 2;
            var shuffled = playerIds.OrderBy(_ => Guid.NewGuid()).ToList();
            var groupAIsHiding = roundIndex % 2 == 0;

            return shuffled.Select((id, i) => new RoleAssignment
            {
                PlayerId = id,
                Role = ResolveRole(i, half, groupAIsHiding)
            }).ToList();
        }

        private static PlayerRole ResolveRole(int index, int half, bool groupAIsHiding)
        {
            var inGroupA = index < half;
            if (groupAIsHiding)
                return inGroupA ? PlayerRole.Hider : PlayerRole.Seeker;
            return inGroupA ? PlayerRole.Seeker : PlayerRole.Hider;
        }

        public bool CheckHidersWin(int survivingHiders, float timeRemaining) =>
            survivingHiders > 0 && timeRemaining <= 0f;

        public bool CheckSeekersWin(int survivingHiders, int totalHiders) =>
            survivingHiders <= 0;

        public void OnHiderTagged(ref PlayerRole taggedRole, IReadOnlyList<RoleAssignment> currentRoles) =>
            taggedRole = PlayerRole.Spectator;
    }
}

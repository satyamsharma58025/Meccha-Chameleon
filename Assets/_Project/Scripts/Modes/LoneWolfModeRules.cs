using System;
using System.Collections.Generic;
using System.Linq;
using HueSeek.Core;
using HueSeek.Player;

namespace HueSeek.Modes
{
    public class LoneWolfModeRules : IGameModeRules
    {
        public GameModeType ModeType => GameModeType.LoneWolf;

        public float GetPrepDurationSeconds(int playerCount, float mapScale) =>
            Math.Clamp(45f * mapScale, 30f, 60f);

        public float GetHuntDurationSeconds(int playerCount) =>
            Math.Clamp(75f, 60f, 90f);

        public IReadOnlyList<RoleAssignment> AssignRoles(IReadOnlyList<int> playerIds, int roundIndex = 0)
        {
            if (playerIds.Count == 0) return Array.Empty<RoleAssignment>();

            var wolf = playerIds[UnityEngine.Random.Range(0, playerIds.Count)];
            return playerIds.Select(id => new RoleAssignment
            {
                PlayerId = id,
                Role = id == wolf ? PlayerRole.Seeker : PlayerRole.Hider
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

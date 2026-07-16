using System.Collections.Generic;
using HueSeek.Core;
using HueSeek.Player;

namespace HueSeek.Modes
{
    public struct RoleAssignment
    {
        public int PlayerId;
        public PlayerRole Role;
    }

    public interface IGameModeRules
    {
        GameModeType ModeType { get; }
        float GetPrepDurationSeconds(int playerCount, float mapScale);
        float GetHuntDurationSeconds(int playerCount);
        IReadOnlyList<RoleAssignment> AssignRoles(IReadOnlyList<int> playerIds, int roundIndex = 0);
        bool CheckHidersWin(int survivingHiders, float timeRemaining);
        bool CheckSeekersWin(int survivingHiders, int totalHiders);
        void OnHiderTagged(ref PlayerRole taggedRole, IReadOnlyList<RoleAssignment> currentRoles);
    }
}

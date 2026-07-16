using System;
using System.Threading.Tasks;
using HueSeek.Core;

namespace HueSeek.Networking
{
    public struct RoomConfig
    {
        public string RoomCode;
        public GameModeType Mode;
        public string MapId;
        public int MaxPlayers;
        public bool IsPrivate;
        public bool Ranked;
        public int StreamDelaySeconds;
    }

    public interface INetworkService
    {
        bool IsConnected { get; }
        bool IsHost { get; }

        Task ConnectAsync(string displayName);
        Task<string> CreateRoomAsync(RoomConfig config);
        Task JoinRoomAsync(string roomCode);
        Task LeaveRoomAsync();

        void SendPaintStroke(int playerId, Paint.PaintStroke stroke);
        void SendTagEvent(int seekerId, int hiderId);
        void SendPhaseChange(RoundPhase phase, float duration);

        event Action<int> OnPlayerJoined;
        event Action<int> OnPlayerLeft;
        event Action<RoundPhase, float> OnPhaseChanged;
        event Action<Paint.PaintStroke> OnRemoteStroke;
    }
}

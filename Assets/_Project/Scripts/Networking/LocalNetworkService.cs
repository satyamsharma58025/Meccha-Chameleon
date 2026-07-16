using System;
using System.Threading.Tasks;
using HueSeek.Core;
using HueSeek.Networking;
using UnityEngine;

namespace HueSeek.Networking
{
    /// <summary>
    /// Offline/local stub. Replace with Photon Fusion 2 or EOS implementation.
    /// </summary>
    public class LocalNetworkService : MonoBehaviour, INetworkService
    {
        public bool IsConnected { get; private set; }
        public bool IsHost => IsConnected;

        public event Action<int> OnPlayerJoined;
        public event Action<int> OnPlayerLeft;
        public event Action<RoundPhase, float> OnPhaseChanged;
        public event Action<Paint.PaintStroke> OnRemoteStroke;

        public Task ConnectAsync(string displayName)
        {
            IsConnected = true;
            OnPlayerJoined?.Invoke(1);
            return Task.CompletedTask;
        }

        public Task<string> CreateRoomAsync(RoomConfig config)
        {
            var code = string.IsNullOrEmpty(config.RoomCode)
                ? GenerateRoomCode()
                : config.RoomCode;
            return Task.FromResult(code);
        }

        public Task JoinRoomAsync(string roomCode) => Task.CompletedTask;

        public Task LeaveRoomAsync()
        {
            IsConnected = false;
            return Task.CompletedTask;
        }

        public void SendPaintStroke(int playerId, Paint.PaintStroke stroke) =>
            OnRemoteStroke?.Invoke(stroke);

        public void SendTagEvent(int seekerId, int hiderId) { }

        public void SendPhaseChange(RoundPhase phase, float duration) =>
            OnPhaseChanged?.Invoke(phase, duration);

        private static string GenerateRoomCode()
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            var code = new char[6];
            for (var i = 0; i < code.Length; i++)
                code[i] = chars[UnityEngine.Random.Range(0, chars.Length)];
            return new string(code);
        }
    }
}

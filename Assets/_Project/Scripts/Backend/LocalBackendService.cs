using System.Threading.Tasks;
using UnityEngine;

namespace HueSeek.Backend
{
    /// <summary>
    /// Firebase/Firestore stub. Wire to Firebase SDK in production builds.
    /// </summary>
    public class LocalBackendService : MonoBehaviour, IBackendService
    {
        private PlayerProfile _profile;

        public event System.Action<PlayerProfile> OnProfileUpdated;

        public Task<bool> SignInGuestAsync()
        {
            _profile = new PlayerProfile
            {
                UserId = System.Guid.NewGuid().ToString("N"),
                DisplayName = "Guest",
                Level = 1
            };
            OnProfileUpdated?.Invoke(_profile);
            return Task.FromResult(true);
        }

        public Task<bool> SignInGoogleAsync() => SignInGuestAsync();
        public Task<bool> SignInAppleAsync() => SignInGuestAsync();

        public Task SignOutAsync()
        {
            _profile = null;
            return Task.CompletedTask;
        }

        public Task<PlayerProfile> LoadProfileAsync(string userId) =>
            Task.FromResult(_profile);

        public Task SaveProfileAsync(PlayerProfile profile)
        {
            _profile = profile;
            OnProfileUpdated?.Invoke(_profile);
            return Task.CompletedTask;
        }

        public Task ReportMatchAsync(MatchResult result)
        {
            if (_profile == null) return Task.CompletedTask;
            _profile.Xp += result.XpEarned;
            _profile.Coins += result.CoinsEarned;
            return SaveProfileAsync(_profile);
        }

        public Task SyncInventoryAsync() => Task.CompletedTask;
    }
}

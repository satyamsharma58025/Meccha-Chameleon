using System;
using System.Threading.Tasks;

namespace HueSeek.Backend
{
    public interface IBackendService
    {
        Task<bool> SignInGuestAsync();
        Task<bool> SignInGoogleAsync();
        Task<bool> SignInAppleAsync();
        Task SignOutAsync();

        Task<PlayerProfile> LoadProfileAsync(string userId);
        Task SaveProfileAsync(PlayerProfile profile);

        Task ReportMatchAsync(MatchResult result);
        Task SyncInventoryAsync();

        event Action<PlayerProfile> OnProfileUpdated;
    }

    public struct MatchResult
    {
        public string MatchId;
        public int XpEarned;
        public int CoinsEarned;
        public bool Ranked;
    }
}

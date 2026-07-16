using System;
using System.Collections.Generic;

namespace HueSeek.Backend
{
    [Serializable]
    public class PlayerProfile
    {
        public string UserId;
        public string DisplayName;
        public int Level;
        public int Xp;
        public int Coins;
        public int PremiumCurrency;
        public int Mmr;
        public List<string> UnlockedCosmetics = new();
        public Dictionary<string, int> Stats = new();
    }
}

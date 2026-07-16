namespace HueSeek.Core
{
    /// <summary>
    /// Global tuning values for Hue &amp; Seek (codename: Project Mimic).
    /// </summary>
    public static class GameConstants
    {
        public const string GameTitle = "Hue & Seek";
        public const string Codename = "ProjectMimic";

        public const int MinPlayers = 2;
        public const int SweetSpotMaxPlayers = 10;
        public const int MaxPlayers = 24;

        public const float PrepPhaseMinSeconds = 45f;
        public const float PrepPhaseMaxSeconds = 90f;
        public const float HuntPhaseMinSeconds = 180f;
        public const float HuntPhaseMaxSeconds = 300f;

        public const int MaxPaletteSlots = 8;
        public const float SampleCooldownSeconds = 0.75f;
        public const float SampleRangeMeters = 2f;

        public const float DetectionShimmerDuration = 0.4f;
        public const float SeekerInspectZoomDuration = 1.5f;

        public const float TargetFpsMidRange = 60f;
        public const float TargetFpsLowEndFloor = 30f;
    }
}

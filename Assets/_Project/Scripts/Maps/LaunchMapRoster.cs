namespace HueSeek.Maps
{
    public enum MapDifficulty
    {
        Beginner,
        Intermediate,
        Expert
    }

    public struct MapDefinition
    {
        public string Id;
        public string DisplayName;
        public MapDifficulty Difficulty;
        public float Scale;
        public string SceneAddress;
    }

    public static class LaunchMapRoster
    {
        public static readonly MapDefinition[] Maps =
        {
            new() { Id = "sunroom_greenhouse", DisplayName = "Sunroom Greenhouse", Difficulty = MapDifficulty.Beginner, Scale = 1f, SceneAddress = "Maps/SunroomGreenhouse" },
            new() { Id = "neon_diner", DisplayName = "Neon Diner", Difficulty = MapDifficulty.Beginner, Scale = 1f, SceneAddress = "Maps/NeonDiner" },
            new() { Id = "attic_workshop", DisplayName = "Attic Workshop", Difficulty = MapDifficulty.Intermediate, Scale = 1.1f, SceneAddress = "Maps/AtticWorkshop" },
            new() { Id = "arcade_hall", DisplayName = "Arcade Hall", Difficulty = MapDifficulty.Intermediate, Scale = 1.15f, SceneAddress = "Maps/ArcadeHall" },
            new() { Id = "patio_bbq", DisplayName = "Patio BBQ", Difficulty = MapDifficulty.Intermediate, Scale = 1f, SceneAddress = "Maps/PatioBBQ" },
            new() { Id = "aquarium_wing", DisplayName = "Aquarium Wing", Difficulty = MapDifficulty.Expert, Scale = 1.2f, SceneAddress = "Maps/AquariumWing" }
        };
    }
}

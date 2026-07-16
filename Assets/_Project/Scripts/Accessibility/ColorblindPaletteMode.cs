using UnityEngine;

namespace HueSeek.Accessibility
{
    /// <summary>
    /// Mandatory colorblind-assist palette remapping for the core color-matching loop.
    /// </summary>
    public static class ColorblindPaletteMode
    {
        public enum Mode
        {
            Off,
            Deuteranopia,
            Protanopia,
            Tritanopia,
            HighContrast
        }

        public static Mode Current { get; private set; } = Mode.Off;

        public static void SetMode(Mode mode) => Current = mode;

        public static Color Remap(Color input)
        {
            return Current switch
            {
                Mode.Deuteranopia => ApplyMatrix(input, DeuteranopiaMatrix),
                Mode.Protanopia => ApplyMatrix(input, ProtanopiaMatrix),
                Mode.Tritanopia => ApplyMatrix(input, TritanopiaMatrix),
                Mode.HighContrast => HighContrastRemap(input),
                _ => input
            };
        }

        private static readonly float[,] DeuteranopiaMatrix =
        {
            { 0.625f, 0.375f, 0f },
            { 0.7f, 0.3f, 0f },
            { 0f, 0.3f, 0.7f }
        };

        private static readonly float[,] ProtanopiaMatrix =
        {
            { 0.567f, 0.433f, 0f },
            { 0.558f, 0.442f, 0f },
            { 0f, 0.242f, 0.758f }
        };

        private static readonly float[,] TritanopiaMatrix =
        {
            { 0.95f, 0.05f, 0f },
            { 0f, 0.433f, 0.567f },
            { 0f, 0.475f, 0.525f }
        };

        private static Color ApplyMatrix(Color c, float[,] m)
        {
            return new Color(
                c.r * m[0, 0] + c.g * m[0, 1] + c.b * m[0, 2],
                c.r * m[1, 0] + c.g * m[1, 1] + c.b * m[1, 2],
                c.r * m[2, 0] + c.g * m[2, 1] + c.b * m[2, 2],
                c.a);
        }

        private static Color HighContrastRemap(Color c)
        {
            var lum = 0.299f * c.r + 0.587f * c.g + 0.114f * c.b;
            return lum > 0.5f ? Color.white : Color.black;
        }
    }
}

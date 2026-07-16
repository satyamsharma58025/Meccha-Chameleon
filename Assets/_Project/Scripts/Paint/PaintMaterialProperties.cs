using System;
using UnityEngine;

namespace HueSeek.Paint
{
    [Serializable]
    public struct PaintMaterialProperties
    {
        [Range(0f, 1f)] public float Metallic;
        [Range(0f, 1f)] public float Roughness;
        [Range(0f, 1f)] public float PatternComplexity;

        /// <summary>
        /// Automatic imperfection noise — always non-zero so paint is never pixel-perfect.
        /// </summary>
        [Range(0.05f, 0.25f)] public float ImperfectionNoise;

        public static PaintMaterialProperties Default => new()
        {
            Metallic = 0.1f,
            Roughness = 0.55f,
            PatternComplexity = 0.2f,
            ImperfectionNoise = 0.12f
        };
    }
}

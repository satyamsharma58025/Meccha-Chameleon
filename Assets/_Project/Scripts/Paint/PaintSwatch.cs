using System;
using UnityEngine;

namespace HueSeek.Paint
{
    [Serializable]
    public struct PaintSwatch
    {
        public Color DominantColor;
        public string TextureSwatchId;
        public PaintMaterialProperties Material;
        public float SampledAtTime;

        public bool IsValid => !string.IsNullOrEmpty(TextureSwatchId);
    }
}

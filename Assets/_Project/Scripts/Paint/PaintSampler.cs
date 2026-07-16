using System;
using HueSeek.Core;
using UnityEngine;

namespace HueSeek.Paint
{
    /// <summary>
    /// Eyedropper sampling: tap-and-hold within range to pull dominant color + texture swatch.
    /// </summary>
    public class PaintSampler : MonoBehaviour
    {
        [SerializeField] private LayerMask _sampleableLayers = ~0;
        [SerializeField] private float _sampleRange = GameConstants.SampleRangeMeters;

        private float _lastSampleTime = float.NegativeInfinity;

        public bool CanSample => Time.time - _lastSampleTime >= GameConstants.SampleCooldownSeconds;

        public float CooldownRemaining =>
            Mathf.Max(0f, GameConstants.SampleCooldownSeconds - (Time.time - _lastSampleTime));

        public bool TrySample(Ray ray, out PaintSwatch swatch, out RaycastHit hit)
        {
            swatch = default;
            hit = default;

            if (!CanSample) return false;
            if (!Physics.Raycast(ray, out hit, _sampleRange, _sampleableLayers)) return false;

            var renderer = hit.collider.GetComponentInParent<Renderer>();
            if (renderer == null) return false;

            swatch = BuildSwatchFromHit(renderer, hit);
            _lastSampleTime = Time.time;
            return swatch.IsValid;
        }

        private static PaintSwatch BuildSwatchFromHit(Renderer renderer, RaycastHit hit)
        {
            var color = SampleDominantColor(renderer, hit);
            var swatchId = GenerateTextureSwatchId(renderer, hit);

            return new PaintSwatch
            {
                DominantColor = color,
                TextureSwatchId = swatchId,
                Material = PaintMaterialProperties.Default,
                SampledAtTime = Time.time
            };
        }

        private static Color SampleDominantColor(Renderer renderer, RaycastHit hit)
        {
            // MVP: material color at hit. Production: texture mip readback or pre-baked swatch atlas.
            var block = new MaterialPropertyBlock();
            renderer.GetPropertyBlock(block);
            if (block.HasColor("_BaseColor"))
                return block.GetColor("_BaseColor");
            return renderer.sharedMaterial != null
                ? renderer.sharedMaterial.GetColor("_BaseColor")
                : Color.gray;
        }

        private static string GenerateTextureSwatchId(Renderer renderer, RaycastHit hit)
        {
            var tex = renderer.sharedMaterial?.GetTexture("_BaseMap");
            var uv = hit.textureCoord;
            return $"{renderer.GetInstanceID()}_{tex?.GetInstanceID() ?? 0}_{uv.x:F3}_{uv.y:F3}";
        }
    }
}

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
            var material = SampleMaterialProperties(renderer, hit);
            var swatchId = GenerateTextureSwatchId(renderer, hit);

            return new PaintSwatch
            {
                DominantColor = color,
                TextureSwatchId = swatchId,
                Material = material,
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

        private static PaintMaterialProperties SampleMaterialProperties(Renderer renderer, RaycastHit hit)
        {
            var block = new MaterialPropertyBlock();
            renderer.GetPropertyBlock(block);

            var metallic = block.HasFloat("_Metallic") ? block.GetFloat("_Metallic") : renderer.sharedMaterial?.GetFloat("_Metallic") ?? 0f;
            var roughness = block.HasFloat("_Roughness") ? block.GetFloat("_Roughness") : renderer.sharedMaterial?.GetFloat("_Roughness") ?? 0.55f;
            var baseColor = block.HasColor("_BaseColor") ? block.GetColor("_BaseColor") : renderer.sharedMaterial?.GetColor("_BaseColor") ?? Color.gray;
            var brightness = (baseColor.r + baseColor.g + baseColor.b) / 3f;

            return new PaintMaterialProperties
            {
                Metallic = Mathf.Clamp01(metallic + (brightness > 0.6f ? 0.05f : 0f)),
                Roughness = Mathf.Clamp01(roughness + (brightness < 0.3f ? 0.08f : 0f)),
                PatternComplexity = 0.2f,
                ImperfectionNoise = 0.12f
            };
        }

        private static string GenerateTextureSwatchId(Renderer renderer, RaycastHit hit)
        {
            var tex = renderer.sharedMaterial?.GetTexture("_BaseMap");
            var uv = hit.textureCoord;
            var texId = tex != null ? tex.GetEntityId().ToString().GetHashCode() : 0;
            return $"{renderer.GetEntityId()}_{texId}_{uv.x:F3}_{uv.y:F3}";
        }
    }
}

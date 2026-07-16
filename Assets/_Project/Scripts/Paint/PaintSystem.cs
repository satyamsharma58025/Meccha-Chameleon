using System;
using System.Collections.Generic;
using HueSeek.Core;
using HueSeek.Player;
using UnityEngine;
using UnityEngine.Events;

namespace HueSeek.Paint
{
    /// <summary>
    /// World-space 3D painting via raycast strokes. Paint stays live through Hunt phase.
    /// Strokes are discrete events for server validation and anti-cheat.
    /// </summary>
    public class PaintSystem : MonoBehaviour
    {
        [SerializeField] private PaintSampler _sampler;
        [SerializeField] private Renderer _avatarRenderer;
        [SerializeField] private PaintTextureAccumulator _paintAccumulator;
#pragma warning disable CS0414
        [SerializeField] private int _paintTextureResolution = 512;
#pragma warning restore CS0414

        private readonly PaintPalette _palette = new();
        private readonly List<PaintStroke> _pendingStrokes = new();
        private int _strokeSequence;
        private int _paintLayerMask = Physics.AllLayers;

        public PaintPalette Palette => _palette;
        public UnityEvent<PaintStroke> OnStrokeApplied = new();
        public UnityEvent<PaintSwatch> OnSwatchSampled = new();

        public bool IsPaintModeActive { get; private set; }
        public BrushTool ActiveTool { get; private set; } = BrushTool.Freehand;
        public PatternStampType ActivePattern { get; private set; } = PatternStampType.Stripes;
        public PaintMaterialProperties ActiveMaterial { get; private set; } = PaintMaterialProperties.Default;

        public void SetPaintMode(bool active) => IsPaintModeActive = active;

        public void SetActiveTool(BrushTool tool) => ActiveTool = tool;

        public void SetActivePattern(PatternStampType pattern)
        {
            ActivePattern = pattern;
            ActiveTool = BrushTool.PatternStamp;
        }

        public void SetMaterialProperties(PaintMaterialProperties material) => ActiveMaterial = material;

        public void SetMetallic(float metallic)
        {
            ActiveMaterial = new PaintMaterialProperties
            {
                Metallic = Mathf.Clamp01(metallic),
                Roughness = ActiveMaterial.Roughness,
                PatternComplexity = ActiveMaterial.PatternComplexity,
                ImperfectionNoise = ActiveMaterial.ImperfectionNoise
            };
        }

        public void SetRoughness(float roughness)
        {
            ActiveMaterial = new PaintMaterialProperties
            {
                Metallic = ActiveMaterial.Metallic,
                Roughness = Mathf.Clamp01(roughness),
                PatternComplexity = ActiveMaterial.PatternComplexity,
                ImperfectionNoise = ActiveMaterial.ImperfectionNoise
            };
        }

        public void AdjustMetallic(float delta) => SetMetallic(ActiveMaterial.Metallic + delta);

        public void AdjustRoughness(float delta) => SetRoughness(ActiveMaterial.Roughness + delta);

        public bool TrySampleColor(Ray ray)
        {
            if (!_sampler.TrySample(ray, out var swatch, out _)) return false;

            _palette.StoreSwatch(_palette.ActiveIndex, swatch);
            OnSwatchSampled?.Invoke(swatch);
            return true;
        }

        public bool TryApplyStroke(Ray ray, float brushRadius, float pressure, int playerId)
        {
            if (!IsPaintModeActive) return false;

            var layerMask = _paintLayerMask >= 0 ? _paintLayerMask : Physics.AllLayers;
            if (!Physics.Raycast(ray, out var hit, Mathf.Infinity, layerMask)) return false;

            var renderer = hit.collider.GetComponentInParent<Renderer>();
            if (renderer == null || renderer != _avatarRenderer) return false;

            var baseColor = _palette.ActiveSwatch.IsValid ? _palette.ActiveSwatch.DominantColor : Color.white;
            var sampledMaterial = _palette.ActiveSwatch.IsValid ? _palette.ActiveSwatch.Material : ActiveMaterial;
            var stroke = new PaintStroke
            {
                PlayerId = playerId,
                TimestampMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                WorldHitPoint = hit.point,
                WorldNormal = hit.normal,
                Color = ApplyImperfection(baseColor, sampledMaterial),
                Material = new PaintMaterialProperties
                {
                    Metallic = sampledMaterial.Metallic,
                    Roughness = sampledMaterial.Roughness,
                    PatternComplexity = sampledMaterial.PatternComplexity,
                    ImperfectionNoise = sampledMaterial.ImperfectionNoise
                },
                Tool = ActiveTool,
                Pattern = ActivePattern,
                BrushRadius = brushRadius,
                BrushPressure = pressure,
                SequenceNumber = ++_strokeSequence
            };

            ApplyStrokeLocally(stroke, hit.textureCoord);
            _pendingStrokes.Add(stroke);
            OnStrokeApplied?.Invoke(stroke);
            return true;
        }

        private void ApplyStrokeLocally(PaintStroke stroke, Vector2 uv)
        {
            if (_paintAccumulator != null)
            {
                if (ActiveTool == BrushTool.PatternStamp)
                    ApplyPatternStamp(stroke, uv);
                else if (ActiveTool == BrushTool.BucketFill)
                    ApplyPatternStamp(stroke, uv, 8);
                else
                    _paintAccumulator.ApplyStroke(stroke, uv);
            }

            if (_avatarRenderer == null) return;

            var block = new MaterialPropertyBlock();
            _avatarRenderer.GetPropertyBlock(block);
            block.SetColor("_PaintColor", stroke.Color);
            block.SetFloat("_Metallic", stroke.Material.Metallic);
            block.SetFloat("_Roughness", stroke.Material.Roughness);
            block.SetFloat("_Imperfection", stroke.Material.ImperfectionNoise);
            _avatarRenderer.SetPropertyBlock(block);
        }

        private void ApplyPatternStamp(PaintStroke stroke, Vector2 uv, int count = 4)
        {
            if (_paintAccumulator == null) return;

            var offsets = ActivePattern switch
            {
                PatternStampType.Dots => CreateDotPattern(uv, count),
                PatternStampType.Checker => CreateCheckerPattern(uv, count),
                PatternStampType.WoodGrain => CreateWoodGrainPattern(uv, count),
                _ => CreateStripePattern(uv, count)
            };

            foreach (var offset in offsets)
                _paintAccumulator.ApplyStroke(stroke, offset);
        }

        private static List<Vector2> CreateStripePattern(Vector2 uv, int count)
        {
            var pattern = new List<Vector2>();
            for (var i = 0; i < count; i++)
                pattern.Add(new Vector2(uv.x + (i % 2 == 0 ? -0.01f : 0.01f), uv.y + i * 0.015f));
            return pattern;
        }

        private static List<Vector2> CreateDotPattern(Vector2 uv, int count)
        {
            var pattern = new List<Vector2>();
            for (var i = 0; i < count; i++)
            {
                var x = uv.x + ((i % 2) * 0.02f) - 0.01f;
                var y = uv.y + ((i / 2) * 0.02f) - 0.01f;
                pattern.Add(new Vector2(x, y));
            }
            return pattern;
        }

        private static List<Vector2> CreateCheckerPattern(Vector2 uv, int count)
        {
            var pattern = new List<Vector2>();
            for (var i = 0; i < count; i++)
            {
                var x = uv.x + (i % 2 == 0 ? 0.015f : -0.015f);
                var y = uv.y + (i % 3 == 0 ? 0.015f : -0.015f);
                pattern.Add(new Vector2(x, y));
            }
            return pattern;
        }

        private static List<Vector2> CreateWoodGrainPattern(Vector2 uv, int count)
        {
            var pattern = new List<Vector2>();
            for (var i = 0; i < count; i++)
                pattern.Add(new Vector2(uv.x + Mathf.Sin(i * 0.8f) * 0.015f, uv.y + i * 0.012f));
            return pattern;
        }

        private static Color ApplyImperfection(Color baseColor, PaintMaterialProperties material)
        {
            var noise = Mathf.Lerp(0.01f, 0.07f, material.ImperfectionNoise);
            return new Color(
                Mathf.Clamp01(baseColor.r + UnityEngine.Random.Range(-noise, noise)),
                Mathf.Clamp01(baseColor.g + UnityEngine.Random.Range(-noise, noise)),
                Mathf.Clamp01(baseColor.b + UnityEngine.Random.Range(-noise, noise)),
                1f);
        }

        public IReadOnlyList<PaintStroke> ConsumePendingStrokes()
        {
            var copy = new List<PaintStroke>(_pendingStrokes);
            _pendingStrokes.Clear();
            return copy;
        }

        public void ResetPaintState()
        {
            _pendingStrokes.Clear();
            _strokeSequence = 0;
            if (_avatarRenderer != null)
                _avatarRenderer.SetPropertyBlock(null);

            _paintAccumulator?.Clear();
        }

        public void Configure(PaintSampler sampler, Renderer avatarRenderer, PaintTextureAccumulator paintAccumulator, int avatarLayer)
        {
            _sampler = sampler;
            _avatarRenderer = avatarRenderer;
            _paintAccumulator = paintAccumulator;
            _paintLayerMask = 1 << avatarLayer;
        }
    }
}

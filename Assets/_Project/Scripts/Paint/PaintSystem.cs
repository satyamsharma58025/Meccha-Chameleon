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
        [SerializeField] private int _paintTextureResolution = 512;

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

        public void SetActivePattern(PatternStampType pattern) => ActivePattern = pattern;

        public void SetMaterialProperties(PaintMaterialProperties material) => ActiveMaterial = material;

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

            var stroke = new PaintStroke
            {
                PlayerId = playerId,
                TimestampMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                WorldHitPoint = hit.point,
                WorldNormal = hit.normal,
                Color = _palette.ActiveSwatch.IsValid ? _palette.ActiveSwatch.DominantColor : Color.white,
                Material = ActiveMaterial,
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
                _paintAccumulator.ApplyStroke(stroke, uv);

            if (_avatarRenderer == null) return;

            var block = new MaterialPropertyBlock();
            _avatarRenderer.GetPropertyBlock(block);
            block.SetColor("_PaintColor", stroke.Color);
            block.SetFloat("_Metallic", stroke.Material.Metallic);
            block.SetFloat("_Roughness", stroke.Material.Roughness);
            block.SetFloat("_Imperfection", stroke.Material.ImperfectionNoise);
            _avatarRenderer.SetPropertyBlock(block);
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

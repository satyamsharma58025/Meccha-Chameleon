# Paint System — Technical Design

## Overview

The paint system is the signature mechanic of Hue & Seek. Players sample colors from world surfaces and apply strokes directly onto their Clayling avatar in 3D space via raycast — not a flat 2D unwrap UI.

## Components

| Class | Responsibility |
|-------|----------------|
| `PaintSampler` | Eyedropper: raycast within 2 m, 0.75 s cooldown |
| `PaintPalette` | 8 swatch slots, mid-round swapping |
| `PaintSystem` | Brush/bucket/stamp application, stroke emission |
| `PaintStroke` | Discrete network event (position, color, material, sequence) |
| `ServerPaintValidator` | Server-side stroke pacing and cheat detection |
| `ClaylingPaint.shader` | URP accumulation into `_PaintMap` RenderTexture |

## Sampling Pipeline

1. Player long-presses → raycast from camera.
2. Hit surface within `SampleRangeMeters` (2 m).
3. Extract dominant color from `_BaseColor` or texture UV (production: mip readback).
4. Generate `TextureSwatchId` from renderer + texture + UV for pattern reference.
5. Store in active palette slot; fire `OnSwatchSampled`.

## Painting Pipeline

1. Paint Mode toggled via dedicated mobile button.
2. Touch-drag emits raycasts against own avatar mesh collider.
3. Each frame of drag produces a `PaintStroke` with monotonic `SequenceNumber`.
4. Client applies locally to `_PaintMap` RenderTexture (GPU blit / compute stamp).
5. Strokes queued in `ConsumePendingStrokes()` for network upload.

### Brush Tools

- **Freehand** — variable-radius circular stamp along ray hit normal.
- **Bucket Fill** — flood-fill on mesh UV island (connectivity-limited for mobile perf).
- **Pattern Stamp** — stripes, dots, checker, woodgrain texture overlays.

### Material Properties

```csharp
struct PaintMaterialProperties {
    float Metallic;      // 0 = matte, 1 = reflective
    float Roughness;     // 0 = smooth, 1 = textured
    float PatternComplexity;
    float ImperfectionNoise; // always ≥ 0.05 — no perfect invisibility
}
```

Imperfection noise is **always applied** in the shader so skilled hand-painting wins over auto-match but never achieves pixel-perfect hiding.

## RenderTexture Accumulation (Production)

```
Avatar Mesh
    ↓ raycast hit → UV
PaintStroke Buffer (CPU)
    ↓ network → server validate → broadcast
GPU Blit Pass
    ↓ stamp brush kernel into _PaintMap (512–1024² per device tier)
ClaylingPaint.shader
    ↓ lerp(base, paintColor, paintMask.r) × imperfection
Final shaded output
```

### Device Tiers

| Tier | Paint Map Res | Max Strokes/sec |
|------|---------------|-----------------|
| Low (2 GB) | 256² | 15 |
| Mid (4 GB) | 512² | 25 |
| High | 1024² | 30 |

## Hunt-Phase Repaint Risk

Repaint while a Seeker has line-of-sight triggers `DetectionRiskTracker.RegisterRepaint()` → shimmer particle scaled by brush pressure. Standing still with completed paint = zero tell.

## Pose Integration

Pose changes register as high-magnitude tells (0.6). Pose menu suggests context poses from nearby geometry collider bounds.

## Server Authority

The server is the source of truth for live paint state:

- Reject out-of-order strokes (`SequenceNumber`).
- Rate-limit strokes per second per player.
- Reject instant full-canvas texture uploads — only discrete strokes accepted.
- Broadcast validated strokes to all clients for RenderTexture replay.

See [anti-cheat-paint-validation.md](anti-cheat-paint-validation.md).

## Future: Map Editor Paint Preview

Community map editor uses the same `PaintSampler` against placed geometry props so creators can tag "sample-friendly" surfaces and difficulty tiers.

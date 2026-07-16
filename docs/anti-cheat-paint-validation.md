# Anti-Cheat — Paint Validation

## Threat Model

Primary exploit: external tools that scan the background/environment and auto-generate a near-perfect paint texture in one upload, bypassing the intended hand-painting skill loop.

Secondary threats: stroke flooding, replay injection, out-of-order sequence replay.

## Mitigations

### 1. Server-Authoritative Strokes

Clients never upload full texture blobs. They send discrete `PaintStroke` events:

```csharp
struct PaintStroke {
    int PlayerId;
    long TimestampMs;
    Vector3 WorldHitPoint;
    Vector3 WorldNormal;
    Color Color;
    PaintMaterialProperties Material;
    BrushTool Tool;
    int SequenceNumber;  // monotonic per player per round
}
```

Server validates, then broadcasts accepted strokes. Clients reconstruct `_PaintMap` locally from the stroke log.

### 2. Rate Limiting

| Rule | Value |
|------|-------|
| `MaxStrokesPerSecond` | 30 |
| Reject duplicate/out-of-order `SequenceNumber` | immediate |

### 3. Statistical Improbability Flagging

`ServerPaintValidator` tracks per-player sessions:

| Metric | Flag Condition |
|--------|----------------|
| Coverage | > 85% body |
| Fidelity | ≥ 0.92 vs nearest environment sample |
| Time | < 1.0 second elapsed |
| Stroke count | < 12 strokes |

All four together → `FlaggedForReview = true` (stroke still accepted in casual; blocked in ranked pending review).

### 4. Fidelity Scoring (Server)

For each stroke batch, server computes:

```
fidelity = 1 - normalizedColorDistance(strokeColor, nearestEnvSampleAtPose)
coverageDelta = brushArea / totalPaintableArea
```

Environment samples are pre-baked per map zone (dominant colors + texture IDs at spawn-adjacent surfaces).

### 5. Network Security

- TLS for all backend traffic
- Encrypted game traffic via Photon/EOS transport
- Firebase Auth tokens for backend API
- Standard rate limiting on auth and match endpoints

### 6. Replay Review Pipeline

Flagged sessions stored with:

- Full stroke log
- Player pose timeline
- Prep-phase duration
- Map ID + spawn zone

Manual review queue for ranked mode; automated temp-ban threshold after N flags.

## Automated Tests

Edit-mode tests in `Assets/_Project/Tests/EditMode/ServerPaintValidatorTests.cs`:

- Normal stroke sequences accepted
- Out-of-order sequences rejected
- Impossibly fast perfect jobs flagged

Run via Unity Test Runner → EditMode.

## QA Cheat Test Cases

See [qa-checklist.md](qa-checklist.md) section "Anti-Cheat Validation".

## Ranked vs Casual

| Behavior | Casual | Ranked |
|----------|--------|--------|
| Flagged stroke | Accepted + logged | Accepted + MMR hold |
| Repeated flags | Warning | Temp queue ban |
| Confirmed cheat | Report only | Season ban |

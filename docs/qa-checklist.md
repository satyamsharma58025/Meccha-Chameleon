# QA & Performance Checklist

## Device Tiers

Test on representative devices:

| Tier | Example Devices | FPS Target |
|------|-----------------|------------|
| Low | 2 GB RAM Android | ≥ 30 FPS |
| Mid | 4 GB RAM Android | ≥ 60 FPS |
| High | Flagship Android / iPhone 13+ | 60 FPS stable |
| Tablet | iPad / Galaxy Tab | 60 FPS, UI scale |

## Paint Shader Benchmarks

- [ ] 256² paint map: ≥ 30 FPS on low tier during active painting
- [ ] 512² paint map: ≥ 60 FPS on mid tier
- [ ] 10 simultaneous players repainting: no frame drops > 16 ms spike
- [ ] Bucket fill completes < 100 ms on mid tier
- [ ] Shader imperfection noise visible at all quality tiers

## Gameplay Flow

- [ ] Full round: Lobby → Prep → Hunt → Reveal → Rewards
- [ ] Seekers locked during Prep (no camera, countdown visible)
- [ ] Paint persists and is editable during Hunt
- [ ] Shimmer tell on movement/repaint in Seeker LOS
- [ ] Taunt bonus + Seeker punish bonus on post-taunt tag
- [ ] Full reveal shows all hiding spots
- [ ] Spectator free-cam after tag

## Game Modes

- [ ] Classic: 1–3 Seekers, timer/survival win conditions
- [ ] Overrun: tagged Hider becomes Seeker
- [ ] Trade Places: two halves, combined scoring
- [ ] Lone Wolf: 60–90 s timer, single Seeker

## Multiplayer

- [ ] 2–10 player sweet spot pacing
- [ ] 16–24 player stress test (Overrun)
- [ ] Reconnect mid-round (grace period)
- [ ] Cross-platform Android ↔ iOS
- [ ] Private room code + public queue

## Anti-Cheat Validation

- [ ] Normal 30-stroke paint job: accepted, not flagged
- [ ] 5-stroke 95% coverage in < 1 s: flagged
- [ ] Out-of-order sequence: rejected
- [ ] 40 strokes/sec flood: rate-limited
- [ ] Full texture upload attempt: rejected (no endpoint)

## Accessibility

- [ ] Colorblind modes: Deuteranopia, Protanopia, Tritanopia, High Contrast
- [ ] UI scale slider functional
- [ ] Haptic toggle
- [ ] Subtitle support for audio cues

## Streamer Features

- [ ] Room code overlay toggle
- [ ] Nameplate toggle
- [ ] Stream delay option (3–10 s)
- [ ] Legible round timer at 720p capture

## Monetization (F2P)

- [ ] Rewarded ads opt-in only
- [ ] No gameplay advantage from cosmetics
- [ ] Battle Pass progression server-authoritative

## Localization Smoke Test

- [ ] EN, JA, ES, zh-CN, KO, PT-BR string tables load
- [ ] No overflow on longest German/French strings (future)

## Crash & Analytics

- [ ] Crashlytics symbol upload
- [ ] DAU/retention events firing
- [ ] `flagged_paint_job_rate` dashboard metric

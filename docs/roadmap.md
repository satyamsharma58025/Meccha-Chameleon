# Implementation Roadmap

## Phase 1 — Foundation ✅ (Current)

- [x] Unity 6 project structure (URP manifest, gitignore)
- [x] Modular C# architecture (Paint, Detection, Modes, Networking, Backend)
- [x] Round state machine + match orchestrator
- [x] All four game mode rule sets
- [x] Paint system MVP (sampler, palette, strokes, validator)
- [x] ClaylingPaint URP shader stub
- [x] Anti-cheat validation + edit-mode tests
- [x] Technical documentation

## Phase 2 — Playable Vertical Slice

- [ ] Bootstrap scene with one greybox map (Sunroom Greenhouse)
- [ ] Clayling placeholder mesh + animations (idle, crouch, cling)
- [ ] RenderTexture paint accumulation GPU pass
- [ ] Mobile touch UI (joystick, paint overlay, pose radial)
- [ ] Local multiplayer hot-seat or split test with 2 Claylings
- [ ] Prep/Hunt phase UI with countdown
- [ ] Full reveal camera sequence
- [ ] Basic SFX (brush, sample, tag, taunt)

## Phase 3 — Online Multiplayer

- [ ] Photon Fusion 2 or EOS `INetworkService` implementation
- [ ] Room codes, matchmaking, reconnect
- [ ] Server-hosted `ServerPaintValidator` on dedicated/host
- [ ] Firebase auth (Google, Apple, Guest) + Firestore profiles
- [ ] Cross-platform Android ↔ iOS test builds

## Phase 4 — Content & Progression

- [ ] All 6 launch maps (art pass)
- [ ] XP, coins, levels, daily/weekly missions
- [ ] Cosmetic shop + Battle Pass (no pay-to-win)
- [ ] Achievement system
- [ ] Ranked mode + hidden MMR

## Phase 5 — Map Editor & Community

- [ ] In-app geometry placement tool
- [ ] Spawn zones + sightline tagging
- [ ] Publish / browse / rate / subscribe pipeline
- [ ] Moderation queue for uploaded maps

## Phase 6 — Live Ops & Ship

- [ ] Rewarded + interstitial ads integration
- [ ] Push notifications
- [ ] Localization (EN, JA, ES, zh-CN, KO, PT-BR)
- [ ] Streamer overlay pack
- [ ] Google Play + App Store submission
- [ ] Remote config + analytics dashboards

## Phase 7 — Stretch

- [ ] WebGL build
- [ ] Steam Deck-compatible desktop build
- [ ] EOS Voice / Vivox integration
- [ ] Seasonal events + map reskins

## Engineering Priorities

1. **Paint shader + RenderTexture** — highest technical risk; schedule first in Phase 2.
2. **Server paint validation** — ship with first online build; never defer.
3. **Full reveal sequence** — core virality moment; polish early.
4. **Map editor** — major retention driver; start Phase 5 design during Phase 3.

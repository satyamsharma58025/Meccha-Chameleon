# Hue & Seek — Project Mimic

**Hue & Seek** is an original mobile multiplayer 3D hide-and-seek party game where players paint their Clayling mascot to blend into the environment. Codename: **Project Mimic**.

## Quick Start

### Requirements

- Unity 6 (6000.0.x) with **Universal Render Pipeline (URP)**
- Android SDK / Xcode for mobile builds
- Optional: Photon Fusion 2 or Epic Online Services SDK
- Optional: Firebase Unity SDK

### Open the Project

1. Install [Unity Hub](https://unity.com/download) and Unity 6 with Android + iOS modules.
2. **Add** → select this repository folder.
3. Open `Assets/_Project/Scenes/` once bootstrap scenes are added (Phase 2).
4. Press Play with `LocalNetworkService` + `LocalBackendService` for offline iteration.

## Architecture

```
Assets/_Project/
├── Scripts/
│   ├── Core/           Round state machine, match orchestrator, constants
│   ├── Paint/          Sampling, strokes, palette, server validation
│   ├── Player/         Clayling controller, poses, detection risk
│   ├── Detection/      Seeker inspect + tag
│   ├── Modes/          Classic, Overrun, Trade Places, Lone Wolf
│   ├── Networking/     INetworkService (Photon/EOS adapters)
│   ├── Backend/        IBackendService (Firebase adapter)
│   ├── Input/          Mobile touch mapping
│   ├── Scoring/        XP/coin event scoring
│   ├── Maps/           Launch map roster definitions
│   ├── Taunt/          Risk/reward taunt system
│   └── Accessibility/  Mandatory colorblind palette modes
├── Shaders/            ClaylingPaint URP shader (MVP)
└── Tests/              Edit-mode anti-cheat validation tests
```

## Game Identity

| Field | Value |
|-------|-------|
| **Ship name** | Hue & Seek |
| **Codename** | Project Mimic |
| **Mascot** | Claylings — small clay-golem creatures (not reptile-themed) |
| **Genre** | Multiplayer party / stealth / hidden-object |

## Round Flow

```
LOBBY → ROLE ASSIGNMENT → PREP (45–90s) → HUNT (3–5 min) → ROUND END → FULL REVEAL → REWARDS
```

- **Prep:** Hiders paint freely; Seekers locked out.
- **Hunt:** Paint stays live; movement/repaint near Seekers triggers shimmer tells.
- **Reveal:** Every hiding spot shown — core teaching moment.

## Game Modes

| Mode | Description |
|------|-------------|
| **Classic** | 1–3 Seekers vs Hiders |
| **Overrun** | Tagged Hiders become Seekers |
| **Trade Places** | Two halves, roles reverse |
| **Lone Wolf** | One Seeker, 60–90s timer |

## Launch Maps

- Sunroom Greenhouse (Beginner)
- Neon Diner (Beginner)
- Attic Workshop (Intermediate)
- Arcade Hall (Intermediate)
- Patio BBQ (Intermediate)
- Aquarium Wing (Expert)

## Networking & Backend

| Layer | Recommended | Current Stub |
|-------|-------------|--------------|
| Netcode | Photon Fusion 2 or EOS | `LocalNetworkService` |
| Backend | Firebase / Firestore | `LocalBackendService` |
| Auth | Google, Apple, Guest | Guest stub |
| Voice | Vivox or EOS Voice | Not yet integrated |

## Performance Targets

- **60 FPS** on mid-range Android (4 GB RAM)
- **30 FPS floor** on low-end (2 GB RAM)
- Sweet spot: **2–10 players**; up to **24** for Overrun/Lone Wolf

## Documentation

- [Paint System](docs/paint-system.md)
- [Anti-Cheat Validation](docs/anti-cheat-paint-validation.md)
- [Deployment Guide](docs/deployment-guide.md)
- [QA Checklist](docs/qa-checklist.md)
- [IP Safety Checklist](docs/ip-safety-checklist.md)
- [Implementation Roadmap](docs/roadmap.md)

## IP Safety

Before any public build, complete every item in [docs/ip-safety-checklist.md](docs/ip-safety-checklist.md).

## License

Proprietary — all assets and code are original IP for Hue & Seek.

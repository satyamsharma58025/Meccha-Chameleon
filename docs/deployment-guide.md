# Deployment Guide

## Android (Google Play)

1. Install Unity Android Build Support module.
2. **Edit → Project Settings → Player → Android:**
   - Minimum API Level: 24 (Android 7.0)
   - Target API Level: latest stable
   - Scripting Backend: IL2CPP
   - Target Architectures: ARM64 (required for Play Store)
3. Add `google-services.json` to `Assets/StreamingAssets/` (gitignored).
4. Configure keystore under Publishing Settings.
5. **File → Build Settings → Android → Build App Bundle (.aab)**.
6. Upload AAB to Google Play Console → Internal testing track first.

## iOS (App Store)

1. Install Unity iOS Build Support + Xcode on macOS build machine.
2. **Player → iOS:** Bundle ID `com.yourstudio.hueseek`
3. Add `GoogleService-Info.plist` for Firebase.
4. Enable Sign in with Apple capability.
5. Build Xcode project → Archive → Upload to App Store Connect.
6. TestFlight before production release.

## Firebase Setup

1. Create Firebase project → add Android + iOS apps.
2. Enable Authentication (Google, Apple, Anonymous/Guest).
3. Enable Firestore, Cloud Functions (optional), Crashlytics, Remote Config.
4. Replace `LocalBackendService` with Firebase adapter (Phase 3).

## Networking Setup

### Option A: Photon Fusion 2

1. Import Fusion 2 from Photon dashboard.
2. Implement `INetworkService` as `FusionNetworkService`.
3. Configure App ID in Photon dashboard; region selection for matchmaking.

### Option B: Epic Online Services

1. Register product on Epic Developer Portal.
2. Enable EOS Sessions, P2P, Voice, Leaderboards.
3. Implement `INetworkService` as `EosNetworkService`.

## Remote Config Keys

| Key | Default | Purpose |
|-----|---------|---------|
| `prep_duration_scale` | 1.0 | Live tuning prep phase |
| `hunt_duration_scale` | 1.0 | Live tuning hunt phase |
| `max_strokes_per_second` | 30 | Anti-cheat throttle |
| `ads_enabled` | true | Rewarded/interstitial toggle |
| `ranked_enabled` | false | Gradual ranked rollout |

## CI/CD (Recommended)

- GitHub Actions or Unity Cloud Build
- Edit-mode tests on every PR
- Android AAB artifact on `main` merge
- Firebase App Distribution for internal QA builds

## Environment Secrets (Never Commit)

- `google-services.json`
- `GoogleService-Info.plist`
- Keystore + passwords
- Photon App ID / EOS credentials
- Firebase service account (CI only)

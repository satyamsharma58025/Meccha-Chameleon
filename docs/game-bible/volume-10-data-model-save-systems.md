# Volume 10 — Data Model & Save Systems

## 1. Core Data Models

- PlayerProfile
- MatchResult
- PaintStroke
- RoleAssignment
- RoundState
- TauntEvent

## 2. Persistence

- Local save for profile and settings
- Session state for resumed play
- Optional cloud sync later

## 3. Save Rules

- Save only durable state
- Avoid saving transient visual effects
- Keep data versioned for future migrations

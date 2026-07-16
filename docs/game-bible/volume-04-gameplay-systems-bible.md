# Volume 4 — Gameplay Systems Bible

## 1. Paint System

- Sample color and material approximation from surfaces
- Apply solid-color paint with optional pattern stamp overlays
- Maintain imperfection noise to avoid perfect matches

## 2. Detection System

- Inspect nearby objects
- Tag hidden players when within range
- Trigger shimmer risk feedback when state changes are visible to Seekers

## 3. Role System

- Hider: survive and camouflage
- Seeker: inspect, tag, and close the round
- Spectator: eliminated or out-of-round

## 4. State Machines

- Round state machine: Lobby → RoleAssign → Prep → Hunt → Results
- Player state: Stand, Crouch, LieFlat, ClingWall, ClingCeiling, Sit, ContortSilhouette

# Volume 5 — Multiplayer & Networking Architecture

## 1. Goals

Support local and networked test sessions with a clear path to dedicated multiplayer services.

## 2. Architecture

- Local test service for early iteration
- Network adapter interface for future Photon or EOS integration
- Event-based broadcasting of round and paint actions

## 3. Key Messages

- Round phase change
- Role assignment
- Paint stroke event
- Tag event
- Taunt event

## 4. Failure Handling

- Reconcile out-of-order strokes
- Gracefully recover from disconnected clients
- Preserve round state consistency

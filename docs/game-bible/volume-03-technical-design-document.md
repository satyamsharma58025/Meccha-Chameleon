# Volume 3 — Technical Design Document

## 1. Architecture Goals

- Modular gameplay systems
- Event-driven interactions
- Clear separation between simulation and presentation
- Mobile-first runtime performance

## 2. Runtime Layers

- Input layer
- Gameplay simulation layer
- Networking layer
- Presentation layer
- Persistence layer

## 3. Core Systems

- RoundStateMachine
- MatchOrchestrator
- PaintSystem
- DetectionRiskTracker
- SeekerToolkit
- TauntSystem
- MatchScorer

## 4. Data Ownership

- Match state is owned by the orchestrator.
- Player state is owned by each player controller.
- UI state is driven from game state events.

## 5. Performance Budgets

- Frame budget: 16.6ms target
- Memory budget: keep runtime allocations low in hot paths
- Network traffic: limit broadcast frequency for paint events

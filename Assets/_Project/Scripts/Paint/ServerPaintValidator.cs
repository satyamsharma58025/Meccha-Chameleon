#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HueSeek.Paint
{
    /// <summary>
    /// Server-authoritative paint validation. Flags statistically improbable instant-perfect jobs.
    /// See docs/anti-cheat-paint-validation.md for full algorithm.
    /// </summary>
    public class ServerPaintValidator
    {
        public const float SuspiciousCompletionSeconds = 1.0f;
        public const float MinStrokesForFullCoverage = 12;
        public const float MaxStrokesPerSecond = 30f;
        public const float HighFidelityThreshold = 0.92f;

        public struct ValidationResult
        {
            public bool Accepted;
            public bool FlaggedForReview;
            public string Reason;
        }

        private readonly Dictionary<int, PlayerPaintSession> _sessions = new();
        private readonly Func<float> _getTime;

        private class PlayerPaintSession
        {
            public int StrokeCount;
            public float FirstStrokeTime;
            public float LastStrokeTime;
            public float EstimatedCoverage;
            public float EstimatedFidelity;
            public int LastSequenceNumber;
        }

        public ServerPaintValidator(Func<float>? timeProvider = null)
        {
            _getTime = timeProvider ?? (() => Time.time);
        }

        public ValidationResult ValidateStroke(int playerId, PaintStroke stroke, float estimatedCoverageDelta, float estimatedFidelity)
        {
            var now = _getTime();

            if (!_sessions.TryGetValue(playerId, out var session))
            {
                session = new PlayerPaintSession { FirstStrokeTime = now };
                _sessions[playerId] = session;
            }

            if (stroke.SequenceNumber <= session.LastSequenceNumber)
            {
                return Reject("Out-of-order or duplicate stroke sequence.");
            }

            session.LastSequenceNumber = stroke.SequenceNumber;
            session.StrokeCount++;
            session.LastStrokeTime = now;
            session.EstimatedCoverage = Mathf.Clamp01(session.EstimatedCoverage + estimatedCoverageDelta);
            session.EstimatedFidelity = Mathf.Max(session.EstimatedFidelity, estimatedFidelity);

            var elapsed = session.LastStrokeTime - session.FirstStrokeTime;
            var strokeRate = session.StrokeCount / Mathf.Max(elapsed, 0.001f);

            if (strokeRate > MaxStrokesPerSecond)
                return Reject("Stroke rate exceeds server limit.");

            if (session.EstimatedCoverage > 0.85f &&
                session.EstimatedFidelity >= HighFidelityThreshold &&
                elapsed < SuspiciousCompletionSeconds &&
                session.StrokeCount < MinStrokesForFullCoverage)
            {
                return Flag("Near-perfect camouflage completed too quickly with too few strokes.");
            }

            return Accept();
        }

        public void ResetPlayer(int playerId) => _sessions.Remove(playerId);

        public void ResetAll() => _sessions.Clear();

        public IReadOnlyList<int> GetFlaggedPlayers() =>
            _sessions.Where(kvp => IsSessionSuspicious(kvp.Value)).Select(kvp => kvp.Key).ToList();

        private static bool IsSessionSuspicious(PlayerPaintSession session)
        {
            var elapsed = session.LastStrokeTime - session.FirstStrokeTime;
            return session.EstimatedCoverage > 0.85f &&
                   session.EstimatedFidelity >= HighFidelityThreshold &&
                   elapsed < SuspiciousCompletionSeconds &&
                   session.StrokeCount < MinStrokesForFullCoverage;
        }

        private static ValidationResult Accept() => new() { Accepted = true };

        private static ValidationResult Reject(string reason) =>
            new() { Accepted = false, Reason = reason };

        private static ValidationResult Flag(string reason) =>
            new() { Accepted = true, FlaggedForReview = true, Reason = reason };
    }
}

using HueSeek.Scoring;
using NUnit.Framework;

namespace HueSeek.Tests
{
    public class MatchScorerTests
    {
        [Test]
        public void TauntAndTagGrantBonusPoints()
        {
            var scorer = new MatchScorer();
            scorer.RecordTaunt(7);
            scorer.RecordTag(8, 7);

            var scores = scorer.GetScores();
            Assert.AreEqual(80, scores[8]);
            Assert.AreEqual(50, scores[7]);
        }
    }
}

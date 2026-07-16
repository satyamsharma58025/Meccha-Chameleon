using System.Linq;
using HueSeek.Paint;
using NUnit.Framework;

namespace HueSeek.Tests
{
    public class ServerPaintValidatorTests
    {
        private ServerPaintValidator _validator;
        private float _currentTime;

        [SetUp]
        public void SetUp()
        {
            _currentTime = 0f;
            _validator = new ServerPaintValidator(() => _currentTime);
        }

        [Test]
        public void AcceptsNormalStrokeSequence()
        {
            AdvanceTime(0.05f);
            var stroke = CreateStroke(1, 0.05f);
            var result = _validator.ValidateStroke(1, stroke, 0.05f, 0.4f);
            Assert.IsTrue(result.Accepted);
            Assert.IsFalse(result.FlaggedForReview);
        }

        [Test]
        public void RejectsOutOfOrderSequence()
        {
            AdvanceTime(0.05f);
            _validator.ValidateStroke(1, CreateStroke(2, 0.1f), 0.1f, 0.3f);
            AdvanceTime(0.05f);
            var result = _validator.ValidateStroke(1, CreateStroke(1, 0.1f), 0.1f, 0.3f);
            Assert.IsFalse(result.Accepted);
        }

        [Test]
        public void FlagsImpossiblyFastPerfectJob()
        {
            for (var i = 1; i <= 5; i++)
            {
                AdvanceTime(0.05f);
                var result = _validator.ValidateStroke(1, CreateStroke(i, 0.2f), 0.2f, 0.95f);
                if (i < 5)
                    Assert.IsTrue(result.Accepted);
            }

            var flagged = _validator.GetFlaggedPlayers();
            Assert.Contains(1, flagged.ToArray());
        }

        private static PaintStroke CreateStroke(int sequence, float coverageDelta) => new()
        {
            PlayerId = 1,
            SequenceNumber = sequence,
            TimestampMs = sequence * 50,
            BrushRadius = 0.08f,
            BrushPressure = 0.5f,
            Color = UnityEngine.Color.green,
            Material = PaintMaterialProperties.Default
        };

        private void AdvanceTime(float seconds) => _currentTime += seconds;
    }
}

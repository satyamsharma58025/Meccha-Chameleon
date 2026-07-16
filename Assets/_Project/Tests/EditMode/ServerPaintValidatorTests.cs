using System.Linq;
using HueSeek.Paint;
using NUnit.Framework;

namespace HueSeek.Tests
{
    public class ServerPaintValidatorTests
    {
        private ServerPaintValidator _validator;

        [SetUp]
        public void SetUp() => _validator = new ServerPaintValidator();

        [Test]
        public void AcceptsNormalStrokeSequence()
        {
            var stroke = CreateStroke(1, 0.05f);
            var result = _validator.ValidateStroke(1, stroke, 0.05f, 0.4f);
            Assert.IsTrue(result.Accepted);
            Assert.IsFalse(result.FlaggedForReview);
        }

        [Test]
        public void RejectsOutOfOrderSequence()
        {
            _validator.ValidateStroke(1, CreateStroke(2, 0.1f), 0.1f, 0.3f);
            var result = _validator.ValidateStroke(1, CreateStroke(1, 0.1f), 0.1f, 0.3f);
            Assert.IsFalse(result.Accepted);
        }

        [Test]
        public void FlagsImpossiblyFastPerfectJob()
        {
            for (var i = 1; i <= 5; i++)
            {
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
    }
}

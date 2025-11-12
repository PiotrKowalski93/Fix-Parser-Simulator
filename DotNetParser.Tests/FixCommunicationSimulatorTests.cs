using Exchange;

namespace FixSimulator.Tests
{
    internal class FixCommunicationSimulatorTests
    {
        private ExchangeServer _simulator;

        [SetUp]
        public void Setup()
        {
            _simulator = new ExchangeServer();
        }

        [Test]
        public void PrepareTags_ShouldReturnDictionaryWithCorrectValues()
        {
            // Arrange
            List<string> input = new List<string>() { "35=8", "49=Sender", "56=Target" };

            // Act
            var result = _simulator.PrepareTags(input);

            // Assert
            Assert.That(result.Count, Is.EqualTo(3));
            Assert.That(result["35"], Is.EqualTo("8"));
            Assert.That(result["49"], Is.EqualTo("Sender"));
            Assert.That(result["56"], Is.EqualTo("Target"));
        }

        [Test]
        public void PrepareTags_ShouldReturnEmptyDictionary_WhenInputIsEmpty()
        {
            // Arrange
            List<string> input = new List<string>() { };

            // Act
            var result = _simulator.PrepareTags(input);

            // Assert
            Assert.That(result != null);
            Assert.That(result.Count, Is.EqualTo(0));
        }

        [Test]
        public void PrepareTags_ShouldThrow_WhenKeyIsDuplicated()
        {
            // Arrange
            List<string> input = new List<string>() { "35=8", "35=9" };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _simulator.PrepareTags(input));
        }

        [Test]
        public void PrepareTags_ShouldThrow_WhenSectionHasNoEqualsSign()
        {
            // Arrange
            List<string> input = new List<string>() { "35=8", "InvalidTag" };

            // Act & Assert
            Assert.Throws<IndexOutOfRangeException>(() => _simulator.PrepareTags(input));
        }

        [Test]
        public void PrepareTags_ShouldHandleEmptyValue()
        {
            // Arrange
            List<string> input = new List<string>() { "35=" };

            // Act
            var result = _simulator.PrepareTags(input);

            // Assert
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result["35"], Is.EqualTo(""));
        }
    }
}

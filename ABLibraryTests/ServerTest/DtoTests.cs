using System.Collections.Generic;
using System.Text.Json;
using Testing.DTO;
using Xunit;

namespace ABProjectTests.ServerTests
{
    public class DtoTests
    {
        [Fact]
        public void TestDto_JsonRoundTrip_ShouldPreserveValues()
        {
            // Arrange
            var expected = new TestDto
            {
                Id = "test_123",
                ApplicationId = "app_456",
                Name = "Тестовый эксперимент",
                Description = "Описание",
                Enabled = true
            };

            // Act
            var json = JsonSerializer.Serialize(expected);
            var actual = JsonSerializer.Deserialize<TestDto>(json);

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(expected.ApplicationId, actual.ApplicationId);
            Assert.Equal(expected.Name, actual.Name);
            Assert.Equal(expected.Description, actual.Description);
            Assert.Equal(expected.Enabled, actual.Enabled);
        }

        [Fact]
        public void TestWithVariantsDto_ShouldPreserveNestedObjects()
        {
            // Arrange
            var expected = new TestWithVariantsDto
            {
                Test = new TestDto
                {
                    Id = "test_1",
                    Name = "Основной тест"
                },
                Variants = new List<VariantDto>
                {
                    new() { Id = "variant_a", Name = "Вариант A" },
                    new() { Id = "variant_b", Name = "Вариант B" }
                }
            };

            // Act
            var json = JsonSerializer.Serialize(expected);
            var actual =
                JsonSerializer.Deserialize<TestWithVariantsDto>(json);

            // Assert
            Assert.NotNull(actual);
            Assert.NotNull(actual.Test);
            Assert.Equal("test_1", actual.Test.Id);
            Assert.Equal(2, actual.Variants.Count);
            Assert.Equal("Вариант A", actual.Variants[0].Name);
            Assert.Equal("Вариант B", actual.Variants[1].Name);
        }

        [Fact]
        public void AnalyticDto_WhenCreated_ShouldHaveExpectedDefaults()
        {
            // Arrange & Act
            var dto = new AnalyticDto();

            // Assert
            Assert.Equal(0, dto.TotalTests);
            Assert.Equal(0, dto.ActiveTests);
            Assert.Equal(0, dto.TotalVariants);
            Assert.Equal(0, dto.TotalUsers);
            Assert.Equal(0, dto.AvgUsersPerTest);
        }
    }
}
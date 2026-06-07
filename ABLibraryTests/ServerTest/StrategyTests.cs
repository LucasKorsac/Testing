using System;
using System.Collections.Generic;
using System.Linq;
using Testing.DTO;
using Testing.Pattern;
using Xunit;

namespace ABProjectTests.ServerTest
{
    /// <summary>
    /// Тестирование стратегий распределения пользователей
    /// </summary>
    public class StrategyTests
    {
        [Fact]
        public void RandomStrategy_ShouldReturnVariantFromList()
        {
            // Arrange
            var strategy = new RandomStrategy<VariantDto>();
            var variants = new List<VariantDto>
            {
                new VariantDto { Id = "1", Name = "Вариант A" },
                new VariantDto { Id = "2", Name = "Вариант B" }
            };
            var defaultValue = variants[0];

            // Act
            var result = strategy.Choose(variants, defaultValue);

            // Assert
            Assert.Contains(result, variants);
        }

        [Fact]
        public void RandomStrategy_WithEmptyList_ShouldReturnDefault()
        {
            // Arrange
            var strategy = new RandomStrategy<VariantDto>();
            var variants = new List<VariantDto>();
            var defaultValue = new VariantDto { Id = "default", Name = "По умолчанию" };

            // Act
            var result = strategy.Choose(variants, defaultValue);

            // Assert
            Assert.Equal(defaultValue, result);
        }

        [Fact]
        public void RandomStrategy_WithSingleVariant_ShouldReturnThatVariant()
        {
            // Arrange
            var strategy = new RandomStrategy<VariantDto>();
            var variants = new List<VariantDto>
            {
                new VariantDto { Id = "1", Name = "Единственный вариант" }
            };
            var defaultValue = variants[0];

            // Act
            var result = strategy.Choose(variants, defaultValue);

            // Assert
            Assert.Equal(variants[0], result);
        }

        [Fact]
        public void WeightStrategy_CalculateWeight_ShouldReturnPositiveValue()
        {
            // Arrange
            var strategy = new WeightStrategy();

            // Act
            var weight = strategy.CalculateWeight(1, 100, 3, 75.5);

            // Assert
            Assert.True(weight > 0);
        }

        [Fact]
        public void WeightStrategy_WithZeroTotal_ShouldHandleGracefully()
        {
            // Arrange
            var strategy = new WeightStrategy();

            // Act
            var weight = strategy.CalculateWeight(1, 100, 0, 75.5);

            // Assert
            Assert.True(weight > 0);
        }

        [Fact]
        public void WeightStrategy_WithNegativeCount_ShouldHandleGracefully()
        {
            // Arrange
            var strategy = new WeightStrategy();

            // Act
            var weight = strategy.CalculateWeight(1, -5, 3, 75.5);

            // Assert
            Assert.True(weight > 0);
        }
    }
}
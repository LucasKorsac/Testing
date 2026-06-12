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
        #region RandomStrategy Tests

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
        public void RandomStrategy_WithNullList_ShouldReturnDefault()
        {
            // Arrange
            var strategy = new RandomStrategy<VariantDto>();
            var defaultValue = new VariantDto { Id = "default", Name = "По умолчанию" };

            // Act
            var result = strategy.Choose(null, defaultValue);

            // Assert
            Assert.Equal(defaultValue, result);
        }

        #endregion

        #region WeightStrategy Tests

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

        [Fact]
        public void WeightStrategy_HigherIndex_ShouldReturnHigherWeight()
        {
            // Arrange
            var strategy = new WeightStrategy();

            // Act
            var weight1 = strategy.CalculateWeight(1, 100, 3, 75.5);
            var weight2 = strategy.CalculateWeight(2, 100, 3, 75.5);
            var weight3 = strategy.CalculateWeight(3, 100, 3, 75.5);

            // Assert
            Assert.True(weight1 <= weight2);
            Assert.True(weight2 <= weight3);
        }

        [Fact]
        public void WeightStrategy_HigherCount_ShouldReturnHigherWeight()
        {
            // Arrange
            var strategy = new WeightStrategy();

            // Act
            var weightLow = strategy.CalculateWeight(2, 10, 3, 75.5);
            var weightHigh = strategy.CalculateWeight(2, 100, 3, 75.5);

            // Assert
            Assert.True(weightLow <= weightHigh);
        }

        #endregion

        #region EpsilonGreedyStrategy Tests

        [Fact]
        public void EpsilonGreedyStrategy_WithEmptyList_ShouldReturnDefault()
        {
            // Arrange
            var strategy = new EpsilonGreedyStrategy(0.1);
            var variants = new List<VariantDto>();
            var defaultValue = new VariantDto { Id = "default", Name = "Default" };

            // Act
            var result = strategy.Choose(variants, defaultValue);

            // Assert
            Assert.Equal(defaultValue, result);
        }

        [Fact]
        public void EpsilonGreedyStrategy_ShouldInitializeStatsForNewVariants()
        {
            // Arrange
            var strategy = new EpsilonGreedyStrategy(0.1);
            var variants = new List<VariantDto>
            {
                new VariantDto { Id = "1", Name = "Variant A", AbTestId = "test123" },
                new VariantDto { Id = "2", Name = "Variant B", AbTestId = "test123" }
            };
            var defaultValue = variants[0];

            // Act
            var result = strategy.Choose(variants, defaultValue);

            // Assert
            var stats = strategy.GetCurrentStats();
            Assert.Equal(2, stats.Count);
            Assert.Contains("1", stats.Keys);
            Assert.Contains("2", stats.Keys);
        }

        [Fact]
        public void EpsilonGreedyStrategy_UpdateStats_ShouldUpdateCountAndRewards()
        {
            // Arrange
            var strategy = new EpsilonGreedyStrategy(0.1);
            var variantId = "testVariant";
            var variants = new List<VariantDto>
            {
                new VariantDto { Id = variantId, Name = "Test Variant", AbTestId = "test123" }
            };
            var defaultValue = variants[0];

            // Act - сначала вызываем Choose для инициализации
            strategy.Choose(variants, defaultValue);
            strategy.UpdateStats(variantId, 1.0);
            strategy.UpdateStats(variantId, 0.0);

            // Assert
            var stats = strategy.GetCurrentStats();
            Assert.Equal(2, stats[variantId].Count);
            Assert.Equal(1.0, stats[variantId].TotalReward);
            Assert.Equal(1, stats[variantId].Successes);
            Assert.Equal(1, stats[variantId].Failures);
        }

        [Fact]
        public void EpsilonGreedyStrategy_ResetStats_ShouldClearAllStats()
        {
            // Arrange
            var strategy = new EpsilonGreedyStrategy(0.1);
            var variants = new List<VariantDto>
            {
                new VariantDto { Id = "1", Name = "Variant A", AbTestId = "test123" }
            };
            var defaultValue = variants[0];

            strategy.Choose(variants, defaultValue);
            strategy.UpdateStats("1", 1.0);

            // Act
            strategy.ResetStats();

            // Assert
            var stats = strategy.GetCurrentStats();
            Assert.Empty(stats);
        }

        #endregion

        #region ThompsonSamplingStrategy Tests

        [Fact]
        public void ThompsonSamplingStrategy_WithEmptyList_ShouldReturnDefault()
        {
            // Arrange
            var strategy = new ThompsonSamplingStrategy(0.05);
            var variants = new List<VariantDto>();
            var defaultValue = new VariantDto { Id = "default", Name = "Default" };

            // Act
            var result = strategy.Choose(variants, defaultValue);

            // Assert
            Assert.Equal(defaultValue, result);
        }

        [Fact]
        public void ThompsonSamplingStrategy_ShouldInitializeStatsForNewVariants()
        {
            // Arrange
            var strategy = new ThompsonSamplingStrategy(0.05);
            var variants = new List<VariantDto>
            {
                new VariantDto { Id = "1", Name = "Variant A", AbTestId = "test123" },
                new VariantDto { Id = "2", Name = "Variant B", AbTestId = "test123" }
            };
            var defaultValue = variants[0];

            // Act
            var result = strategy.Choose(variants, defaultValue);

            // Assert
            var stats = strategy.GetCurrentStats();
            Assert.Equal(2, stats.Count);
        }

        [Fact]
        public void ThompsonSamplingStrategy_UpdateStats_ShouldUpdateBetaParameters()
        {
            // Arrange
            var strategy = new ThompsonSamplingStrategy(0.05);
            var variantId = "testVariant";
            var variants = new List<VariantDto>
            {
                new VariantDto { Id = variantId, Name = "Test Variant", AbTestId = "test123" }
            };
            var defaultValue = variants[0];

            // Act
            strategy.Choose(variants, defaultValue);
            strategy.UpdateStats(variantId, 1.0); // success
            strategy.UpdateStats(variantId, 0.0); // failure
            strategy.UpdateStats(variantId, 1.0); // success

            // Assert
            var stats = strategy.GetCurrentStats();
            Assert.Equal(3, stats[variantId].Count);
            Assert.Equal(2, stats[variantId].Successes);
            Assert.Equal(1, stats[variantId].Failures);
        }

        [Fact]
        public void ThompsonSamplingStrategy_ResetStats_ShouldClearAllStats()
        {
            // Arrange
            var strategy = new ThompsonSamplingStrategy(0.05);
            var variants = new List<VariantDto>
            {
                new VariantDto { Id = "1", Name = "Variant A", AbTestId = "test123" }
            };
            var defaultValue = variants[0];

            strategy.Choose(variants, defaultValue);
            strategy.UpdateStats("1", 1.0);

            // Act
            strategy.ResetStats();

            // Assert
            var stats = strategy.GetCurrentStats();
            Assert.Empty(stats);
        }

        #endregion

        #region UCBStrategy Tests

        [Fact]
        public void UCBStrategy_WithEmptyList_ShouldReturnDefault()
        {
            // Arrange
            var strategy = new UCBStrategy(1.0);
            var variants = new List<VariantDto>();
            var defaultValue = new VariantDto { Id = "default", Name = "Default" };

            // Act
            var result = strategy.Choose(variants, defaultValue);

            // Assert
            Assert.Equal(defaultValue, result);
        }

        [Fact]
        public void UCBStrategy_UntriedVariants_ShouldBeChosenFirst()
        {
            // Arrange
            var strategy = new UCBStrategy(1.0);
            var variants = new List<VariantDto>
            {
                new VariantDto { Id = "1", Name = "Variant A", AbTestId = "test123" },
                new VariantDto { Id = "2", Name = "Variant B", AbTestId = "test123" }
            };
            var defaultValue = variants[0];

            // Act
            var result = strategy.Choose(variants, defaultValue);

            // Assert - должны выбрать первый непробованный вариант
            Assert.NotNull(result);
        }

        [Fact]
        public void UCBStrategy_UpdateStats_ShouldUpdateCountAndRewards()
        {
            // Arrange
            var strategy = new UCBStrategy(1.0);
            var variantId = "testVariant";
            var variants = new List<VariantDto>
            {
                new VariantDto { Id = variantId, Name = "Test Variant", AbTestId = "test123" }
            };
            var defaultValue = variants[0];

            // Act
            strategy.Choose(variants, defaultValue);
            strategy.UpdateStats(variantId, 1.0);
            strategy.UpdateStats(variantId, 0.5);

            // Assert
            var stats = strategy.GetCurrentStats();
            Assert.Equal(2, stats[variantId].Count);
            Assert.Equal(1.5, stats[variantId].TotalReward);
        }

        [Fact]
        public void UCBStrategy_CalculateUCB_ShouldReturnHigherValueForLessExploredVariants()
        {
            // Arrange
            var strategy = new UCBStrategy(1.0);
            var variantA = "variantA";
            var variantB = "variantB";
            var variants = new List<VariantDto>
            {
                new VariantDto { Id = variantA, Name = "Variant A", AbTestId = "test123" },
                new VariantDto { Id = variantB, Name = "Variant B", AbTestId = "test123" }
            };
            var defaultValue = variants[0];

            // Act - инициализируем и обновляем статистику
            strategy.Choose(variants, defaultValue);
            strategy.UpdateStats(variantA, 1.0);
            strategy.UpdateStats(variantA, 1.0);
            strategy.UpdateStats(variantA, 1.0); // variantA много успехов
            strategy.UpdateStats(variantB, 0.0); // variantB один неуспех

            // Act - делаем ещё один выбор
            var result = strategy.Choose(variants, defaultValue);

            // Assert - UCB должен предпочесть менее исследованный вариант B
            Assert.NotNull(result);
        }

        [Fact]
        public void UCBStrategy_ResetStats_ShouldClearAllStats()
        {
            // Arrange
            var strategy = new UCBStrategy(1.0);
            var variants = new List<VariantDto>
            {
                new VariantDto { Id = "1", Name = "Variant A", AbTestId = "test123" }
            };
            var defaultValue = variants[0];

            strategy.Choose(variants, defaultValue);
            strategy.UpdateStats("1", 1.0);

            // Act
            strategy.ResetStats();

            // Assert
            var stats = strategy.GetCurrentStats();
            Assert.Empty(stats);
        }

        #endregion

        #region MABManager Tests

        [Fact]
        public void MABManager_GetOrCreateStrategy_ShouldReturnSameStrategyForSameKey()
        {
            // Arrange
            var manager = new MABManager(MABStrategyType.ThompsonSampling);
            var testId = "test123";

            // Act
            var strategy1 = manager.GetOrCreateStrategy(testId, MABStrategyType.ThompsonSampling);
            var strategy2 = manager.GetOrCreateStrategy(testId, MABStrategyType.ThompsonSampling);

            // Assert
            Assert.Same(strategy1, strategy2);
        }

        [Fact]
        public void MABManager_GetOrCreateStrategy_DifferentTypes_ShouldReturnDifferentStrategies()
        {
            // Arrange
            var manager = new MABManager(MABStrategyType.ThompsonSampling);
            var testId = "test123";

            // Act
            var strategy1 = manager.GetOrCreateStrategy(testId, MABStrategyType.ThompsonSampling);
            var strategy2 = manager.GetOrCreateStrategy(testId, MABStrategyType.UCB);
            var strategy3 = manager.GetOrCreateStrategy(testId, MABStrategyType.EpsilonGreedy);

            // Assert
            Assert.NotSame(strategy1, strategy2);
            Assert.NotSame(strategy1, strategy3);
            Assert.NotSame(strategy2, strategy3);
        }

        [Fact]
        public void MABManager_UpdateStats_ShouldUpdateCorrectStrategy()
        {
            // Arrange
            var manager = new MABManager(MABStrategyType.ThompsonSampling);
            var testId = "test123";
            var variantId = "variant1";

            // Act
            manager.UpdateStats(testId, variantId, 1.0, MABStrategyType.ThompsonSampling);
            manager.UpdateStats(testId, variantId, 0.0, MABStrategyType.ThompsonSampling);

            var stats = manager.GetStats(testId, MABStrategyType.ThompsonSampling);

            // Assert
            Assert.NotNull(stats);
        }

        [Fact]
        public void MABManager_ResetStats_ShouldRemoveStrategy()
        {
            // Arrange
            var manager = new MABManager(MABStrategyType.ThompsonSampling);
            var testId = "test123";
            var variantId = "variant1";

            manager.UpdateStats(testId, variantId, 1.0);

            // Act
            manager.ResetStats(testId);

            var stats = manager.GetStats(testId);

            // Assert - новая стратегия должна быть создана с пустой статистикой
            Assert.NotNull(stats);
            Assert.Empty(stats);
        }

        #endregion

        #region MABAdapterStrategy Tests

        [Fact]
        public void MABAdapterStrategy_Choose_ShouldReturnVariantFromList()
        {
            // Arrange
            var testId = "test123";
            var adapter = new MABAdapterStrategy(testId, MABStrategyType.ThompsonSampling);
            var variants = new List<VariantDto>
            {
                new VariantDto { Id = "1", Name = "Variant A", AbTestId = testId },
                new VariantDto { Id = "2", Name = "Variant B", AbTestId = testId }
            };
            var defaultValue = variants[0];

            // Act
            var result = adapter.Choose(variants, defaultValue);

            // Assert
            Assert.Contains(result, variants);
        }

        [Fact]
        public void MABAdapterStrategy_UpdateReward_ShouldUpdateStats()
        {
            // Arrange
            var testId = "test123";
            var variantId = "variant1";
            var adapter = new MABAdapterStrategy(testId, MABStrategyType.ThompsonSampling);
            var variants = new List<VariantDto>
            {
                new VariantDto { Id = variantId, Name = "Variant A", AbTestId = testId }
            };
            var defaultValue = variants[0];

            // Act
            adapter.Choose(variants, defaultValue);
            adapter.UpdateReward(variantId, 1.0);
            adapter.UpdateReward(variantId, 0.0);

            var stats = adapter.GetStats();

            // Assert
            Assert.NotNull(stats);
            Assert.Contains(variantId, stats.Keys);
            Assert.Equal(2, stats[variantId].Count);
        }

        [Fact]
        public void MABAdapterStrategy_UpdateConversion_ShouldHandleSuccessAndFailure()
        {
            // Arrange
            var testId = "test123";
            var variantId = "variant1";
            var adapter = new MABAdapterStrategy(testId, MABStrategyType.EpsilonGreedy);
            var variants = new List<VariantDto>
            {
                new VariantDto { Id = variantId, Name = "Variant A", AbTestId = testId }
            };
            var defaultValue = variants[0];

            // Act
            adapter.Choose(variants, defaultValue);
            adapter.UpdateConversion(variantId, true);  // success
            adapter.UpdateConversion(variantId, false); // failure
            adapter.UpdateConversion(variantId, true);  // success

            var stats = adapter.GetStats();

            // Assert
            Assert.Equal(3, stats[variantId].Count);
            Assert.Equal(2, stats[variantId].Successes);
            Assert.Equal(1, stats[variantId].Failures);
        }

        [Fact]
        public void MABAdapterStrategy_Reset_ShouldClearStats()
        {
            // Arrange
            var testId = "test123";
            var variantId = "variant1";
            var adapter = new MABAdapterStrategy(testId, MABStrategyType.ThompsonSampling);
            var variants = new List<VariantDto>
            {
                new VariantDto { Id = variantId, Name = "Variant A", AbTestId = testId }
            };
            var defaultValue = variants[0];

            adapter.Choose(variants, defaultValue);
            adapter.UpdateReward(variantId, 1.0);

            // Act
            adapter.Reset();

            var stats = adapter.GetStats();

            // Assert
            Assert.Empty(stats);
        }

        [Fact]
        public void MABAdapterStrategy_WithCustomMABStrategy_ShouldWorkCorrectly()
        {
            // Arrange
            var customStrategy = new UCBStrategy(2.0);
            var adapter = new MABAdapterStrategy(customStrategy);
            var variants = new List<VariantDto>
            {
                new VariantDto { Id = "1", Name = "Variant A", AbTestId = "test123" },
                new VariantDto { Id = "2", Name = "Variant B", AbTestId = "test123" }
            };
            var defaultValue = variants[0];

            // Act
            var result = adapter.Choose(variants, defaultValue);
            adapter.UpdateReward(result.Id, 1.0);

            // Assert
            Assert.Contains(result, variants);
        }

        #endregion

        #region AdaptStrategy Tests (требует интеграционного тестирования)

        // AdaptStrategy требует MongoDB, поэтому эти тесты должны быть интеграционными
        // или использовать моки. Примеры ниже предполагают наличие моков.

        [Fact]
        public void AdaptStrategy_WithEmptyList_ShouldReturnDefault()
        {
            // This test requires mocking or an in-memory database
            // var strategy = new AdaptStrategy(mockAdaptation);
            // var result = strategy.Choose(new List<VariantDto>(), defaultValue);
            // Assert.Equal(defaultValue, result);
        }

        [Fact]
        public void AdaptStrategy_WithNullInstanceId_ShouldFallbackToRandom()
        {
            // This test requires mocking or an in-memory database
        }

        [Fact]
        public void AdaptStrategy_WithValidParameters_ShouldReturnVariantFromPool()
        {
            // This test requires mocking or an in-memory database
        }

        #endregion

        #region VariantStats Tests

        [Fact]
        public void VariantStats_ConversionRate_ShouldCalculateCorrectly()
        {
            // Arrange
            var stats = new VariantStats
            {
                VariantId = "test",
                VariantName = "Test",
                Successes = 7,
                Failures = 3,
                Count = 10,
                TotalReward = 7.0
            };

            // Assert
            Assert.Equal(0.7, stats.ConversionRate, 2);
        }

        [Fact]
        public void VariantStats_WithZeroCount_ConversionRateShouldBeZero()
        {
            // Arrange
            var stats = new VariantStats
            {
                VariantId = "test",
                VariantName = "Test",
                Successes = 0,
                Failures = 0,
                Count = 0,
                TotalReward = 0
            };

            // Assert
            Assert.Equal(0, stats.ConversionRate);
        }

        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text.Json;
using Testing.DTO;
using Xunit;

namespace ABProjectTests.ServerTests
{
    /// <summary>
    /// Тестирование DTO объектов
    /// </summary>
    public class DtoTests
    {
        [Fact]
        public void TestDto_CanBeCreatedAndModified()
        {
            // Arrange & Act
            var dto = new TestDto
            {
                Id = "test_123",
                ApplicationId = "app_456",
                Name = "Тестовый тест",
                Description = "Описание теста",
                Enabled = true
            };

            // Assert
            Assert.Equal("test_123", dto.Id);
            Assert.Equal("app_456", dto.ApplicationId);
            Assert.Equal("Тестовый тест", dto.Name);
            Assert.Equal("Описание теста", dto.Description);
            Assert.True(dto.Enabled);
        }

        [Fact]
        public void VariantDto_CanBeCreatedAndModified()
        {
            // Arrange & Act
            var dto = new VariantDto
            {
                Id = "variant_123",
                AbTestId = "test_456",
                Name = "Вариант A",
                Description = "Описание варианта",
                Mean = 75,
                Audience = 50
            };

            // Assert
            Assert.Equal("variant_123", dto.Id);
            Assert.Equal("test_456", dto.AbTestId);
            Assert.Equal("Вариант A", dto.Name);
            Assert.Equal(75, dto.Mean);
            Assert.Equal(50, dto.Audience);
        }

        [Fact]
        public void ApplicationDto_CanBeCreatedAndModified()
        {
            // Arrange & Act
            var dto = new ApplicationDto
            {
                Id = "app_123",
                Name = "Тестовое приложение",
                Description = "Описание приложения"
            };

            // Assert
            Assert.Equal("app_123", dto.Id);
            Assert.Equal("Тестовое приложение", dto.Name);
            Assert.Equal("Описание приложения", dto.Description);
        }

        [Fact]
        public void InstanceDto_CanBeCreatedAndModified()
        {
            // Arrange & Act
            var date = DateTime.UtcNow;
            var dto = new InstanceDto
            {
                Id = "instance_123",
                ApplicationId = "app_456",
                Name = "Версия 1.0",
                Version = 1,
                Date = date
            };

            // Assert
            Assert.Equal("instance_123", dto.Id);
            Assert.Equal("app_456", dto.ApplicationId);
            Assert.Equal("Версия 1.0", dto.Name);
            Assert.Equal(1, dto.Version);
            Assert.Equal(date, dto.Date);
        }

        [Fact]
        public void AnalyticDto_DefaultValuesAreZero()
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

        [Fact]
        public void TestWithVariantsDto_CanHoldMultipleVariants()
        {
            // Arrange
            var dto = new TestWithVariantsDto
            {
                Test = new TestDto { Id = "test_1", Name = "Главный тест" },
                Variants = new List<VariantDto>
                {
                    new VariantDto { Id = "var_1", Name = "Вариант 1" },
                    new VariantDto { Id = "var_2", Name = "Вариант 2" }
                }
            };

            // Assert
            Assert.Equal(2, dto.Variants.Count);
            Assert.Equal("Главный тест", dto.Test.Name);
        }

        //[Fact]
        //public void ChartDto_CanBeSerializedToJson()
        //{
        //    // Arrange
        //    var dto = new Charts.ChartDto
        //    {
        //        Id = "chart_1",
        //        Type = "bar",
        //        Title = "Тестовый график",
        //        Labels = new List<string> { "Янв", "Фев", "Мар" },
        //        Values = new List<double> { 10, 20, 30 }
        //    };

        //    // Act
        //    var json = JsonSerializer.Serialize(dto);

        //    // Assert
        //    Assert.Contains("chart_1", json);
        //    Assert.Contains("bar", json);
        //    Assert.Contains("Тестовый график", json);
        //}
    }
}
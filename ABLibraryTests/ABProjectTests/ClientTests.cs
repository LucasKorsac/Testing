//using System;
//using System.Collections.Generic;
//using System.IO;
//using Testing.Pattern; 
//namespace FileStorageForUnity;
//using Xunit;

//    public sealed class FileStorageTests : IDisposable
//    {
//        private readonly string _testPath;
//        private readonly FileStorageForUnity _storage;

//        public FileStorageTests()
//        {
//            _testPath = Path.Combine(
//                Path.GetTempPath(),
//                "ABStorageTests",
//                Guid.NewGuid().ToString("N"));

//            Directory.CreateDirectory(_testPath);
//            _storage = new FileStorageForUnity(_testPath);
//        }

//        [Fact]
//        public void SaveAndLoad_ShouldPreserveAllData()
//        {
//            // Arrange
//            var expected = new List<TestEvent>
//            {
//                new()
//                {
//                    TestName = "color_test",
//                    Variant = "A",
//                    UserId = "user_1",
//                    EventType = "click"
//                },
//                new()
//                {
//                    TestName = "checkout_test",
//                    Variant = "B",
//                    UserId = "user_2",
//                    EventType = "conversion"
//                }
//            };

//            // Act
//            _storage.Save("events", expected);
//            var actual = _storage.Load<List<TestEvent>>("events");

//            // Assert
//            Assert.NotNull(actual);
//            Assert.Equal(expected.Count, actual.Count);

//            Assert.Collection(
//                actual,
//                first =>
//                {
//                    Assert.Equal("color_test", first.TestName);
//                    Assert.Equal("A", first.Variant);
//                    Assert.Equal("user_1", first.UserId);
//                    Assert.Equal("click", first.EventType);
//                },
//                second =>
//                {
//                    Assert.Equal("checkout_test", second.TestName);
//                    Assert.Equal("B", second.Variant);
//                    Assert.Equal("user_2", second.UserId);
//                    Assert.Equal("conversion", second.EventType);
//                });
//        }

//        [Fact]
//        public void Load_WhenFileDoesNotExist_ShouldReturnNull()
//        {
//            // Act
//            var actual = _storage.Load<List<TestEvent>>(
//                "missing_" + Guid.NewGuid().ToString("N"));

//            // Assert
//            Assert.Null(actual);
//        }

//        [Fact]
//        public void Save_WhenKeyAlreadyExists_ShouldReplaceStoredData()
//        {
//            // Arrange
//            _storage.Save(
//                "events",
//                new List<TestEvent>
//                {
//                    new() { TestName = "old_test" }
//                });

//            var expected = new List<TestEvent>
//            {
//                new() { TestName = "new_test" }
//            };

//            // Act
//            _storage.Save("events", expected);
//            var actual = _storage.Load<List<TestEvent>>("events");

//            // Assert
//            Assert.NotNull(actual);
//            var item = Assert.Single(actual);
//            Assert.Equal("new_test", item.TestName);
//        }

//        public void Dispose()
//        {
//            if (Directory.Exists(_testPath))
//            {
//                Directory.Delete(_testPath, recursive: true);
//            }
//        }
//    }
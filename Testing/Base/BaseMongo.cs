using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Testing.Base
{
    public class BaseMongo
    {
        // Компания
        public class Companies
        {
            [BsonId]
            public ObjectId Id { get; set; }

            public string Name { get; set; } = "";
        }

        // Роль
        public class Roles
        {
            [BsonId]
            public ObjectId Id { get; set; }

            public string Name { get; set; } = "";
        }

        // Разработчик
        public class Developers
        {
            [BsonId]
            public ObjectId Id { get; set; }

            public ObjectId CompanyId { get; set; } // FK → Company
            public ObjectId RoleId { get; set; }    // FK → Role

            public string Login { get; set; } = "";         
            public string Password { get; set; } = "";   
            public string PasswordHash { get; set; } = ""; // Хэш пароля
        }

        // Приложение
        public class Applications
        {
            [BsonId]
            public ObjectId Id { get; set; }

            public ObjectId CompanyId { get; set; } // FK → Company

            public string Name { get; set; } = "";
            public string Description { get; set; } = "";
        }

        // Тип метрики
        public class MetricTypes
        {
            [BsonId]
            public ObjectId Id { get; set; }

            public string Name { get; set; } = "";
        }

        // Метрика
        public class Metrics
        {
            [BsonId]
            public ObjectId Id { get; set; }

            public ObjectId MetricTypeId { get; set; } // FK → MetricType

            public string Name { get; set; } = "";
        }

        // Экземпляр приложения
        public class Instances
        {
            [BsonId]
            public ObjectId Id { get; set; }

            public ObjectId ApplicationId { get; set; } // FK → Application
            public ObjectId MetricId { get; set; }      // FK → Metric

            public int Version { get; set; }            // Версия
            public string Name { get; set; } = "";
        }

        // Атрибут
        public class Attributes
        {
            [BsonId]
            public ObjectId Id { get; set; }

            public string Environment { get; set; } = "";   // Окружение
            public string Recommendation { get; set; } = "";
        }

        // Значение
        public class Values
        {
            [BsonId]
            public ObjectId Id { get; set; }

            public ObjectId InstanceId { get; set; }  // FK → Instance
            public ObjectId AttributeId { get; set; } // FK → Attribute

            public DateTime Date { get; set; }        // Дата записи
            public double MetricValue { get; set; }   // Значение
        }

        // Описание A/B теста
        public class ABDescriptions
        {
            [BsonId]
            public ObjectId Id { get; set; }

            public string Target { get; set; } = "";     // Цель
            public string Description { get; set; } = "";   //Описание
        }

        // A/B тест
        public class ABTests
        {
            [BsonId]
            public ObjectId Id { get; set; }

            public ObjectId DescriptionId { get; set; } // FK → ABDescription

            public string Name { get; set; } = "";
        }

        // Вариант
        public class Variants
        {
            [BsonId]
            public ObjectId Id { get; set; }

            public ObjectId AbTestId { get; set; } // FK → AbTest

            public string Name { get; set; } = "";
            public string Description { get; set; } = "";
        }

        // Результат
        public class AbResults
        {
            [BsonId]
            public ObjectId Id { get; set; }

            public ObjectId InstanceId { get; set; } // FK → Instance
            public ObjectId VariantId { get; set; }  // FK → Variant
        }

        //Ивент 25.04
        public class AbEvent
        {
            public ObjectId Id { get; set; }
            public string TestName { get; set; }
            public string VariantName { get; set; }
            public string EventType { get; set; }
            public DateTime Time { get; set; }
            public string UserId { get; set; }
        }
    }
}
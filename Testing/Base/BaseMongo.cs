using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Testing.Base
{
    public class BaseMongo
    {
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

            public string Login { get; set; } = "";         
            public string PasswordHash { get; set; } = ""; // Хэш пароля
        }

        // Приложение
        public class Applications
        {
            [BsonId]
            public ObjectId Id { get; set; }
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
            public ObjectId ApplicationId { get; set; }      // FK → Application

            public double Meaning { get; set; }     //значение
        }

        // Экземпляр приложения
        public class Instances
        {
            [BsonId]
            public ObjectId Id { get; set; }

            public ObjectId ApplicationId { get; set; } // FK → Application

            public int Version { get; set; }            // Версия
            public string Name { get; set; } = "";
            public DateTime Date {get; set; }     //Дата
        }

        // Параметр оборудоваиня
        public class EquipParam
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
            public ObjectId ParamId { get; set; } // FK → EquipParam
            public double MetricValue { get; set; }   // Значение
        }

        // A/B тест
        public class ABTests
        {
            [BsonId]
            public ObjectId Id { get; set; }

            public ObjectId DescriptionId { get; set; } // FK → ABDescription

            public string Name { get; set; } = "";
            public string Description {get; set; }
            public bool Enabled { get; set; }   //Включен ли тест
        }

        // Вариант
        public class Variants
        {
            [BsonId]
            public ObjectId Id { get; set; }

            public ObjectId AbTestId { get; set; } // FK → AbTest

            public string Name { get; set; } = "";
            public string Description { get; set; } = "";
            public int Mean { get; set; }
            public int Audience { get; set; }
        }

        // Результат
        public class AbResults
        {
            [BsonId]
            public ObjectId Id { get; set; }

            public ObjectId InstanceId { get; set; } // FK → Instance
            public ObjectId VariantId { get; set; }  // FK → Variant
        }

        // Слабая сущность Приложение/Разработчик/Роль
        public class DevelopRoleApplic
        {
            [BsonId]
            public ObjectId Id { get; set; }

            public ObjectId DeveloperId { get;set; } //FK → Developer
            public ObjectId RoleId { get; set; } //FK → Role
            public ObjectId Application {  get; set; } //FK → Application
        }
    }
}
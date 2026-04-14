using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Testing.Base
{
    public class BaseMongo
    {
        // Компания
        public class Company
        {
            [BsonId]
            public ObjectId Id { get; set; }
            public string Name { get; set; }

            [BsonExtraElements]
            public BsonDocument Extra { get; set; }
        }

        //Роль пользователя
        public class Role
        {
            [BsonId]
            public ObjectId Id { get; set; }

            public string Name { get; set; }

            [BsonExtraElements]
            public BsonDocument Extra { get; set; }
        }

        //Разработчик

        public class Developer
        {
            [BsonId]
            public ObjectId Id { get; set; }

            public ObjectId CompanyId { get; set; } // FK → Company
            public ObjectId RoleId { get; set; } // FK → Role
            public string Login { get; set; }
            public string Name { get; set; }
            public string Password { get; set; }

            [BsonExtraElements]
            public BsonDocument Extra { get; set; }
        }


        // Приложение

        public class Application
        {
            [BsonId]
            public ObjectId Id { get; set; }

            public ObjectId CompanyId { get; set; } // FK → Company

            public string Description { get; set; }
            public string Name { get; set; }

            [BsonExtraElements]
            public BsonDocument Extra { get; set; }
        }

        //Тип метрики
        public class MetricType
        {
            [BsonId]
            public ObjectId Id { get; set; }

            public string Name { get; set; }

            [BsonExtraElements]
            public BsonDocument Extra { get; set; }
        }

        // Метрика

        public class Metric
        {
            [BsonId]
            public ObjectId Id { get; set; }
            public ObjectId TypeId { get; set; } // FK → MetricType

            public string Name { get; set; }

            [BsonExtraElements]
            public BsonDocument Extra { get; set; }
        }

        //Экземпляр
        public class Instance
        {
            [BsonId]
            public ObjectId Id { get; set; }
            public ObjectId MetricId { get; set; }      // FK → Metric
            public ObjectId ApplicationId { get; set; } // FK → Application
            public string Name { get; set; }          
            public int Value { get; set; }            

            [BsonExtraElements]
            public BsonDocument Extra { get; set; }
        }

        //Атрибут
        public class MAttribute
        {
            [BsonId]
            public ObjectId Id { get; set; }

            public string Name { get; set; }

            // Рекомендации
            public string Recommendation { get; set; }

            [BsonExtraElements]
            public BsonDocument Extra { get; set; }
        }

        // Значение атрибута
        public class Value
        {
            [BsonId]
            public ObjectId Id { get; set; }

            public ObjectId InstanceId { get; set; } // FK → Instance
            public ObjectId AttributeId { get; set; } // FK → Attribute

            public string Description { get; set; }

            public string ValueText { get; set; }

            [BsonExtraElements]
            public BsonDocument Extra { get; set; }
        }

        //Описание теста

        public class ABDescription
        {
            [BsonId]
            public ObjectId Id { get; set; }

            public string Target { get; set; } // цель теста
            public string Descript { get; set; } // описание

            [BsonExtraElements]
            public BsonDocument Extra { get; set; }
        }

        // A/B тест
        public class AbTest
        {
            [BsonId]
            public ObjectId ApplicationId { get; set; } // FK → Application
            public ObjectId DescriptionID { get; set; } // FK → ABDescription
            public ObjectId Id { get; set; }
            public string Name { get; set; }

            [BsonExtraElements]
            public BsonDocument Extra { get; set; }
        }

        //Вариант теста
        public class Variant
        {
            [BsonId]
            public ObjectId AbTestId { get; set; } // FK → AbTest
            public ObjectId Id { get; set; }
            public string Description { get; set; }
            public string Name { get; set; }

            [BsonExtraElements]
            public BsonDocument Extra { get; set; }
        }

        // Результат теста
        public class Result
        {
            [BsonId]
            public ObjectId InstanceId { get; set; } // FK → Instance
            public ObjectId VariantId { get; set; }  // FK → Variant
            public ObjectId Id { get; set; }

            [BsonExtraElements]
            public BsonDocument Extra { get; set; }
        }
    }
}

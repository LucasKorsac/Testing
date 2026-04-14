using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testing.Base;
using static Testing.Base.BaseMongo;

namespace Testing.Pattern
{   /// <summary>
    /// Паттерн адаптер
    /// </summary>
    public class Adapter
    {
        /// <summary>
        /// Выгружаемые данные для A/B теста
        /// </summary>
        public class AbTestDto
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string ApplicationId { get; set; }
            public string DescriptionId { get; set; }
        }
        /// <summary>
        /// Преобразование Mongo модели → DTO
        /// </summary>
        public static class AbTestMapper
        {
            /// <summary>
            /// Конвертация AbTest → AbTestDto
            /// </summary>
            public static AbTestDto ToDto(AbTest model)
            {
                if (model == null)
                    return null;

                return new AbTestDto
                {
                    Id = model.Id.ToString(), Name = model.Name, ApplicationId = model.ApplicationId.ToString(),
                    DescriptionId = model.DescriptionID.ToString()
                };
            }

            /// <summary>
            /// Обратное преобразование DTO → Mongo модель
            /// </summary>
            public static AbTest ToModel(AbTestDto dto)
            {
                if (dto == null)
                    return null;

                return new AbTest
                {
                    Id = ObjectId.Parse(dto.Id), Name = dto.Name, ApplicationId = ObjectId.Parse(dto.ApplicationId),
                    DescriptionID = ObjectId.Parse(dto.DescriptionId)
                };
            }
        }
    }
}

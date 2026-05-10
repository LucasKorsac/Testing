using MongoDB.Bson;
using static Testing.Base.BaseMongo;

namespace Testing.Pattern
{
    /// <summary> Адаптер для преобразования модели A/B теста в DTO </summary>
    public class Adapter
    {
        /// <summary> DTO для передачи данных </summary>
        public class AbTestDto
        {
            public string Id { get; set; } = "";

            public string Name { get; set; } = "";

            public string Description { get; set; } = "";

            public bool Enabled { get; set; }
        }

        /// <summary> Mapper между моделью и DTO </summary>
        public static class AbTestMapper
        {
            /// <summary> Преобразование модели в DTO </summary>
            public static AbTestDto? ToDto(ABTests model)
            {
                if (model == null)
                    return null;

                return new AbTestDto
                {Id = model.Id.ToString(), Name = model.Name, Description = model.Description, Enabled = model.Enabled};
            }

            /// <summary> Преобразование DTO в модель </summary>
            public static ABTests? ToModel(AbTestDto dto)
            {
                if (dto == null)
                    return null;

                return new ABTests
                {
                    Id = string.IsNullOrWhiteSpace(dto.Id)
                        ? ObjectId.GenerateNewId()
                        : ObjectId.Parse(dto.Id),

                    Name = dto.Name, Description = dto.Description, Enabled = dto.Enabled
                };
            }
        }
    }
}
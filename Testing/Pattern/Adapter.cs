using MongoDB.Bson;
using static Testing.Base.BaseMongo;

namespace Testing.Pattern
{
    // Адаптер для преобразования модели A/B теста в DTO и обратно
    public class Adapter
    {
        // DTO используется для передачи данных наружу (например, в API или UI)
        public class AbTestDto
        {
            public string Id { get; set; }
            public string Name { get; set; }

            public string ApplicationId { get; set; }

            public string DescriptionId { get; set; }
        }

        public static class AbTestMapper
        {
            // Преобразование из доменной модели в DTO
            public static AbTestDto ToDto(ABTests model)
            {
                if (model == null) return null;

                return new AbTestDto
                {
                    Id = model.Id.ToString(), Name = model.Name, DescriptionId = model.DescriptionId.ToString()
                };
            }

            // Преобразование из DTO в доменную модель
            public static ABTests ToModel(AbTestDto dto)
            {
                if (dto == null) return null;

                return new ABTests
                {
                    Id = ObjectId.Parse(dto.Id), Name = dto.Name, DescriptionId = ObjectId.Parse(dto.DescriptionId)
                };
            }
        }
    }
}
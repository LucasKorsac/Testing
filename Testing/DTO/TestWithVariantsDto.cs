
namespace Testing.DTO
{
    public class TestWithVariantsDto
    {
        public TestDto Test { get; set; } = new();
        public List<VariantDto> Variants { get; set; } = new();
    }
}

namespace Testing.DTO
{

    public class VariantDto
    {
        public string Id { get; set; } = "";
        public string AbTestId { get; set; } = "";

        public string Name { get; set; } = "";
        public string Description { get; set; } = "";

        public int Mean { get; set; }
        public int Audience { get; set; }
    }
}
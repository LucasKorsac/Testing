namespace Testing.DTO
{

    public class ApplicationWithInstancesDto
    {
        public ApplicationDto Application { get; set; } = new();
        public List<InstanceDto> Instances { get; set; } = new();
    }
}
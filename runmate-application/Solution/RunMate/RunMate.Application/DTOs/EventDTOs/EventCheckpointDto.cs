namespace RunMate.UserService.RunMate.Application.DTOs.EventDTOs
{
    public class EventCheckpointDto
    {
        public Guid Id { get; set; }
        public Guid EventId { get; set; }
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double DistanceFromStart { get; set; }
        public int Order { get; set; }
    }
}

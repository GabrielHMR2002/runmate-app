namespace RunMate.UserService.RunMate.Domain.Entities
{
    public class EventCheckpointEntity
    {
        public Guid Id { get; set; }
        public Guid EventId { get; set; }
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double DistanceFromStart { get; set; } // em km
        public int Order { get; set; }

        // Propriedade de navegação
        public virtual EventEntity Event { get; set; }
    }
}

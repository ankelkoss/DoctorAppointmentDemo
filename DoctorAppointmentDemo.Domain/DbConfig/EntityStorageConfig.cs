namespace DoctorAppointmentDemo.Domain.DbConfig
{
    public sealed class EntityStorageConfig
    {
        public FileStorageConfig Json { get; set; } = new();
        public FileStorageConfig Xml { get; set; } = new();
    }
}

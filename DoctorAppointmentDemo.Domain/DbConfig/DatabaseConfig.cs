namespace DoctorAppointmentDemo.Domain.DbConfig
{
    public sealed class DatabaseConfig
    {
        public EntityStorageConfig Doctors { get; set; } = new();
        public EntityStorageConfig Patients { get; set; } = new();
        public EntityStorageConfig Appointments { get; set; } = new();
    }
}

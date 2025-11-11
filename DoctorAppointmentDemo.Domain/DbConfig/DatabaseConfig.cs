namespace DoctorAppointmentDemo.Domain.DbConfig
{
    public sealed class DatabaseConfig
    {
        public JsonConfig Doctors { get; set; } = new();
        public JsonConfig Patients { get; set; } = new();
        public JsonConfig Appointments { get; set; } = new();
    }
}

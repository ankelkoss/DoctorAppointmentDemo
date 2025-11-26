using DoctorAppointmentDemo.Domain.Entities;

namespace DoctorAppointmentDemo.Domain.Dto
{
    public class AppointmentDTO : Auditable
    {
        public int DoctorId { get; set; }
        public int PatientId { get; set; }

        public DateTime DateTimeFrom { get; set; }
        public DateTime DateTimeTo { get; set; }
        public string? Description { get; set; }
    }
}

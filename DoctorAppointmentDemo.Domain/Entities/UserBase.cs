namespace MyDoctorAppointment.Domain.Entities
{
    public abstract class UserBase : Auditable
    {
        public string Name { get; set; } = string.Empty;

        public string Surname { get; set; } = string.Empty;

        public string? Phone { get; set; }

        public string? Email { get; set; }
    }
}

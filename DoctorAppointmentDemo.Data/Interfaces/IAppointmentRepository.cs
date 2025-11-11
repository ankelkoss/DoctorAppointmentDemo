using DoctorAppointmentDemo.Data.Interfaces.Base;
using MyDoctorAppointment.Domain.Entities;

namespace DoctorAppointmentDemo.Data.Interfaces
{
    public interface IAppointmentRepository : IGenericRepository<Appointment>
    {
    }
}

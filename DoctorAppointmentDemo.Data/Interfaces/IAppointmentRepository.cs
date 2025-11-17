using DoctorAppointmentDemo.Data.Interfaces.Base;
using DoctorAppointmentDemo.Domain.Entities;

namespace DoctorAppointmentDemo.Data.Interfaces
{
    public interface IAppointmentRepository : IGenericRepository<Appointment>
    {
        public void ShowInfo(Appointment source);
    }
}

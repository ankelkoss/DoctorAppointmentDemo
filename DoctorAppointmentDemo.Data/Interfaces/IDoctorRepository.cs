using DoctorAppointmentDemo.Data.Interfaces.Base;
using DoctorAppointmentDemo.Domain.Entities;


namespace DoctorAppointmentDemo.Data.Interfaces
{
    public interface IDoctorRepository : IGenericRepository<Doctor>
    {
        public void ShowInfo(Doctor doctor);
    }
}

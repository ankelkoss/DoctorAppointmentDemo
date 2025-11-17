using DoctorAppointmentDemo.Data.Interfaces.Base;
using DoctorAppointmentDemo.Domain.Entities;

namespace DoctorAppointmentDemo.Data.Interfaces
{
    public interface IPatientRepository : IGenericRepository<Patient>
    {
        public void ShowInfo(Patient source);
    }
}

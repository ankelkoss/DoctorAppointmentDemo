using DoctorAppointmentDemo.Data.Interfaces;
using DoctorAppointmentDemo.Data.Interfaces.Base;
using DoctorAppointmentDemo.Data.Repositories.Json;

namespace DoctorAppointmentDemo.Data.Repositories.Provider
{
    public class JsonRepositoryProvider : IRepositoryProvider
    {
        public IDoctorRepository Doctors { get; }
        public IPatientRepository Patients { get; }
        public IAppointmentRepository Appointments { get; }

        public JsonRepositoryProvider()
        {
            Doctors = new JsonDoctorRepository();
            Patients = new JsonPatientRepository();
            Appointments = new JsonAppointmentRepository();
        }
    }
}

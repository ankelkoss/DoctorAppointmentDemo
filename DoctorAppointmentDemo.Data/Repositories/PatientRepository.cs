using DoctorAppointmentDemo.Data.Interfaces;
using DoctorAppointmentDemo.Data.Repositories.Base;
using DoctorAppointmentDemo.Domain.DbConfig;
using DoctorAppointmentDemo.Domain.Entities;

namespace DoctorAppointmentDemo.Data.Repositories
{
    public class PatientRepository : GenericRepository<Patient>, IPatientRepository
    {
        public override JsonConfig JsonConfig { get; set; }

        public PatientRepository()
        {
            JsonConfig = base.ReadFromAppSettings().Database.Patients;
        }

        public override void ShowInfo(Patient source)
        {
            //TODO: implement this
            Console.WriteLine();
        }
    }
}

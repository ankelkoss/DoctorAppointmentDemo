using DoctorAppointmentDemo.Data.Interfaces;
using DoctorAppointmentDemo.Data.Repositories.Base;
using DoctorAppointmentDemo.Domain.DbConfig;
using DoctorAppointmentDemo.Domain.Entities;

namespace DoctorAppointmentDemo.Data.Repositories
{
    public class AppointmentRepository : GenericRepository<Appointment>, IAppointmentRepository
    {
        public override JsonConfig JsonConfig { get ; set ; }

        public AppointmentRepository()
        {
            JsonConfig = base.ReadFromAppSettings().Database.Appointments;
        }

        public override void ShowInfo(Appointment source)
        {
            string info = string.Format("Запись к доктору: {0} {1}, пациента: {2} {3}, на: {4}", source.Doctor!.Name, source.Doctor.Surname, source.Patient!.Name, source.Patient.Surname, source.DateTimeFrom);
            Console.WriteLine(info);
        }
    }
}

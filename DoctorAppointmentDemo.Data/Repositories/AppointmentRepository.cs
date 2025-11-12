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
            //TODO: implement this
            Console.WriteLine();
        }
    }
}

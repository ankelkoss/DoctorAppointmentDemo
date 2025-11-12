using DoctorAppointmentDemo.Data.Configuration;
using DoctorAppointmentDemo.Data.Interfaces;
using DoctorAppointmentDemo.Data.Repositories.Base;
using DoctorAppointmentDemo.Domain.DbConfig;
using DoctorAppointmentDemo.Domain.Entities;

namespace DoctorAppointmentDemo.Data.Repositories
{
    public class DoctorRepository : GenericRepository<Doctor>, IDoctorRepository
    {
        public override JsonConfig JsonConfig { get; set; }

        public DoctorRepository()
        {
            JsonConfig = base.ReadFromAppSettings().Database.Doctors;
        }

        public override void ShowInfo(Doctor doctor)
        {
            //TODO: implement this
            Console.WriteLine(); // implement view of all object fields
        }
    }
}

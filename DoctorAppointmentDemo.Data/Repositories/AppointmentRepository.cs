using DoctorAppointmentDemo.Data.Configuration;
using DoctorAppointmentDemo.Data.Interfaces;
using DoctorAppointmentDemo.Domain.DbConfig;
using DoctorAppointmentDemo.Domain.Entities;
using Newtonsoft.Json;

namespace DoctorAppointmentDemo.Data.Repositories
{
    public class AppointmentRepository : GenericRepository<Appointment>, IAppointmentRepository
    {
        public override string Path { get; set; }

        public override int LastId { get; set; }

        public AppointmentRepository()
        {
            JsonConfig result = base.ReadFromAppSettings().Database.Appointments;

            Path = result.Path;
            LastId = result.LastId;
        }

        public override void ShowInfo(Appointment source)
        {
            //TODO: implement this
            Console.WriteLine();
        }

        protected override void SaveLastId()
        {
            AppDbConfig result = base.ReadFromAppSettings();

            result = base.MoveGlobalPathAppSettings(result);

            result.Database.Doctors.LastId = LastId;

            File.WriteAllText(Constants.AppSettingsPath, JsonConvert.SerializeObject(result, Formatting.Indented));
        }
    }
}

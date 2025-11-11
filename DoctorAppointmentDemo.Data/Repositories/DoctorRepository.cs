using DoctorAppointmentDemo.Data.Interfaces;
using DoctorAppointmentDemo.Domain.DbConfig;
using MyDoctorAppointment.Data.Configuration;
using MyDoctorAppointment.Domain.Entities;
using Newtonsoft.Json;

namespace MyDoctorAppointment.Data.Repositories
{
    public class DoctorRepository : GenericRepository<Doctor>, IDoctorRepository
    {
        public override string Path { get; set; }

        public override int LastId { get; set; }

        public DoctorRepository()
        {
            JsonConfig result = base.ReadFromAppSettings().Database.Doctors;

            Path = result.Path;
            LastId = result.LastId;
        }

        public override void ShowInfo(Doctor doctor)
        {
            //TODO: implement this
            Console.WriteLine(); // implement view of all object fields
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

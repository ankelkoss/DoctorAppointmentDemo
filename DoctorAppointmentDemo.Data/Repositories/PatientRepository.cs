using DoctorAppointmentDemo.Data.Interfaces;
using DoctorAppointmentDemo.Domain.DbConfig;
using MyDoctorAppointment.Data.Configuration;
using MyDoctorAppointment.Data.Repositories;
using MyDoctorAppointment.Domain.Entities;
using Newtonsoft.Json;

namespace DoctorAppointmentDemo.Data.Repositories
{
    public class PatientRepository : GenericRepository<Patient>, IPatientRepository
    {
        public override string Path { get; set; }

        public override int LastId { get; set; }

        public PatientRepository()
        {
            JsonConfig result = base.ReadFromAppSettings().Database.Patients;

            Path = result.Path;
            LastId = result.LastId;
        }

        public override void ShowInfo(Patient source)
        {
            //TODO: implement this
            Console.WriteLine();
        }

        protected override void SaveLastId()
        {
            AppDbConfig result = base.ReadFromAppSettings();

            result = base.MoveGlobalPathAppSettings(result);

            result.Database.Patients.LastId = LastId;

            File.WriteAllText(Constants.AppSettingsPath, JsonConvert.SerializeObject(result, Formatting.Indented));
        }
    }
}

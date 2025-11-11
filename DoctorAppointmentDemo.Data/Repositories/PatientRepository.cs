
using DoctorAppointmentDemo.Data.Interfaces;
using DoctorAppointmentDemo.Domain.DbConfig;
using MyDoctorAppointment.Data.Configuration;
using MyDoctorAppointment.Data.Interfaces;
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
            AppDbConfig result = base.ReadFromAppSettings();

            Path = result.Database.Patients.Path;
            LastId = result.Database.Patients.LastId;
        }

        public override void ShowInfo(Patient source)
        {
            //TODO: implement this
            throw new NotImplementedException();
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

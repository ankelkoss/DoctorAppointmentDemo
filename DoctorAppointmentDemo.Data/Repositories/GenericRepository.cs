using DoctorAppointmentDemo.Data.Interfaces.Base;
using DoctorAppointmentDemo.Domain.DbConfig;
using MyDoctorAppointment.Data.Configuration;
using MyDoctorAppointment.Domain.Entities;
using Newtonsoft.Json;

namespace MyDoctorAppointment.Data.Repositories
{
    public abstract class GenericRepository<TSource> : IGenericRepository<TSource> where TSource : Auditable
    {
        public abstract string Path { get; set; }

        public abstract int LastId { get; set; }

        public TSource Create(TSource source)
        {
            source.Id = ++LastId;
            source.CreatedAt = DateTime.Now;

            File.WriteAllText(Path, JsonConvert.SerializeObject(GetAll().Append(source), Formatting.Indented));
            SaveLastId();

            return source;
        }

        public bool Delete(int id)
        {
            if (GetById(id) is null)
                return false;

            File.WriteAllText(Path, JsonConvert.SerializeObject(GetAll().Where(x => x.Id != id), Formatting.Indented));

            return true;
        }

        public IEnumerable<TSource> GetAll()
        {
            if (!File.Exists(Path))
            {
                File.WriteAllText(Path, "[]");
            }

            var json = File.ReadAllText(Path);

            if (string.IsNullOrWhiteSpace(json))
            {
                File.WriteAllText(Path, "[]");
                json = "[]";
            }

            return JsonConvert.DeserializeObject<List<TSource>>(json)!;
        }

        public TSource? GetById(int id)
        {
            return GetAll().FirstOrDefault(x => x.Id == id);
        }

        public TSource Update(int id, TSource source)
        {
            source.UpdatedAt = DateTime.Now;
            source.Id = id;

            File.WriteAllText(Path, JsonConvert.SerializeObject(GetAll().Select(x => x.Id == id ? source : x), Formatting.Indented));

            return source;
        }

        public abstract void ShowInfo(TSource source);

        protected abstract void SaveLastId();

        protected AppDbConfig ReadFromAppSettings()
        {
            AppDbConfig appDbConfig = JsonConvert.DeserializeObject<AppDbConfig>(File.ReadAllText(Constants.AppSettingsPath))!;

            appDbConfig = AddGlobalPathAppSettings(appDbConfig);

            return appDbConfig;
        }

        protected AppDbConfig AddGlobalPathAppSettings(AppDbConfig appDbConfig)
        {
            appDbConfig.Database.Doctors.Path = System.IO.Path.Combine(Constants.PrjData, appDbConfig.Database.Doctors.Path);
            appDbConfig.Database.Patients.Path = System.IO.Path.Combine(Constants.PrjData, appDbConfig.Database.Patients.Path);
            appDbConfig.Database.Appointments.Path = System.IO.Path.Combine(Constants.PrjData, appDbConfig.Database.Appointments.Path);

            return appDbConfig;
        }

        protected AppDbConfig MoveGlobalPathAppSettings(AppDbConfig appDbConfig)
        {
            appDbConfig.Database.Doctors.Path = System.IO.Path.GetRelativePath(Constants.PrjData, appDbConfig.Database.Doctors.Path);
            appDbConfig.Database.Patients.Path = System.IO.Path.GetRelativePath(Constants.PrjData, appDbConfig.Database.Patients.Path);
            appDbConfig.Database.Appointments.Path = System.IO.Path.GetRelativePath(Constants.PrjData, appDbConfig.Database.Appointments.Path);

            return appDbConfig;
        }
    }
}

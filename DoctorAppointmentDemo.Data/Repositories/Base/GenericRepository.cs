using DoctorAppointmentDemo.Data.Configuration;
using DoctorAppointmentDemo.Data.Interfaces.Base;
using DoctorAppointmentDemo.Domain.DbConfig;
using DoctorAppointmentDemo.Domain.Entities;
using Newtonsoft.Json;

namespace DoctorAppointmentDemo.Data.Repositories.Base
{
    public abstract class GenericRepository<TSource> : IGenericRepository<TSource> where TSource : Auditable
    {
        public abstract JsonConfig JsonConfig { get; set; }

        public TSource Create(TSource source)
        {
            source.Id = ++this.JsonConfig.LastId;
            source.CreatedAt = DateTime.Now;

            File.WriteAllText(JsonConfig.Path, JsonConvert.SerializeObject(GetAll().Append(source), Formatting.Indented));
            SaveLastId(this.JsonConfig);

            return source;
        }

        public bool Delete(int id)
        {
            if (GetById(id) is null)
                return false;

            File.WriteAllText(this.JsonConfig.Path, JsonConvert.SerializeObject(GetAll().Where(x => x.Id != id), Formatting.Indented));

            return true;
        }

        public IEnumerable<TSource> GetAll()
        {
            if (!File.Exists(this.JsonConfig.Path))
            {
                File.WriteAllText(this.JsonConfig.Path, "[]");
            }

            var json = File.ReadAllText(this.JsonConfig.Path);

            if (string.IsNullOrWhiteSpace(json))
            {
                File.WriteAllText(this.JsonConfig.Path, "[]");
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

            File.WriteAllText(this.JsonConfig.Path, JsonConvert.SerializeObject(GetAll().Select(x => x.Id == id ? source : x), Formatting.Indented));

            return source;
        }

        public abstract void ShowInfo(TSource source);

        public void SaveLastId(JsonConfig jsonConfig)
        {
            // реализовал метод тут так как не вижу смысла плодить копипасту

            var currentPath = Constants.AppSettingsPath;
            var lockPath = currentPath + ".lck";
            var tmpPath = currentPath + ".tmp";

            using var lockStream = new FileStream(
                lockPath,
                FileMode.OpenOrCreate,
                FileAccess.ReadWrite,
                FileShare.None);

            try
            {
                AppDbConfig appDbConfig = this.ReadFromAppSettings();

                if (appDbConfig.Database.Doctors.Equals(jsonConfig))
                {
                    appDbConfig.Database.Doctors.LastId = jsonConfig.LastId;
                    appDbConfig.Database.Doctors = MoveGlobalPathAppSettings(jsonConfig);
                }

                if (appDbConfig.Database.Patients.Equals(jsonConfig))
                {
                    appDbConfig.Database.Patients.LastId = jsonConfig.LastId;
                    appDbConfig.Database.Patients = MoveGlobalPathAppSettings(jsonConfig);
                }

                if (appDbConfig.Database.Appointments.Equals(jsonConfig))
                {
                    appDbConfig.Database.Appointments.LastId = jsonConfig.LastId;
                    appDbConfig.Database.Patients = MoveGlobalPathAppSettings(jsonConfig);
                }

                File.WriteAllText(tmpPath, JsonConvert.SerializeObject(appDbConfig, Formatting.Indented));
                File.Replace(tmpPath, currentPath, null);
            }
            finally
            {
                lockStream.Close();
                try
                {
                    File.Delete(lockPath);
                }
                catch
                {
                    //
                }
            }
        }

        protected AppDbConfig ReadFromAppSettings()
        {
            AppDbConfig appDbConfig = JsonConvert.DeserializeObject<AppDbConfig>(File.ReadAllText(Constants.AppSettingsPath))!;

            appDbConfig = AddGlobalPathAppSettings(appDbConfig);

            return appDbConfig;
        }

        protected AppDbConfig AddGlobalPathAppSettings(AppDbConfig appDbConfig)
        {
            // в файле храним относительные пути, при работе программы динамически подставляем путь при чтении
            appDbConfig.Database.Doctors.Path = Path.Combine(Constants.PrjData, appDbConfig.Database.Doctors.Path);
            appDbConfig.Database.Patients.Path = Path.Combine(Constants.PrjData, appDbConfig.Database.Patients.Path);
            appDbConfig.Database.Appointments.Path = Path.Combine(Constants.PrjData, appDbConfig.Database.Appointments.Path);

            return appDbConfig;
        }

        protected JsonConfig MoveGlobalPathAppSettings(JsonConfig jsonConfig)
        {
            // в файле храним относительные пути, при работе программы динамически удаляем путь при записи
            jsonConfig.Path = Path.GetRelativePath(Constants.PrjData, jsonConfig.Path);

            return jsonConfig;
        }

        
    }
}

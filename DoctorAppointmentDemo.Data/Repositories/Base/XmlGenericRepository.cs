using DoctorAppointmentDemo.Data.Configuration;
using DoctorAppointmentDemo.Data.Interfaces.Base;
using DoctorAppointmentDemo.Data.Repositories.Base.Helpers;
using DoctorAppointmentDemo.Domain.DbConfig;
using DoctorAppointmentDemo.Domain.Entities;
using Newtonsoft.Json;
using System;
using System.Xml.Serialization;

namespace DoctorAppointmentDemo.Data.Repositories.Base
{
    public abstract class XmlGenericRepository<TSource> : IGenericRepository<TSource> where TSource : Auditable
    {
        public abstract FileStorageConfig FileStorageConfig { get; set; }

        public TSource Create(TSource source)
        {
            source.Id = ++this.FileStorageConfig.LastId;
            source.CreatedAt = DateTime.Now;
             
            File.WriteAllText(FileStorageConfig.Path, this.SerializeAll(GetAll().Append(source)));
            SaveLastId(this.FileStorageConfig);

            return source;
        }

        public bool Delete(int id)
        {
            if (GetById(id) is null)
                return false;

            File.WriteAllText(this.FileStorageConfig.Path, this.SerializeAll(GetAll().Where(x => x.Id != id)));

            return true;
        }

        public IEnumerable<TSource> GetAll()
        {
            if (!File.Exists(this.FileStorageConfig.Path))
            {
                File.WriteAllText(this.FileStorageConfig.Path, "");
            }

            var xml = File.ReadAllText(this.FileStorageConfig.Path);

            return DeserializeAll(xml);
        }

        public TSource? GetById(int id)
        {
            return GetAll().FirstOrDefault(x => x.Id == id);
        }

        public TSource Update(int id, TSource source)
        {
            source.UpdatedAt = DateTime.Now;
            source.Id = id;

            File.WriteAllText(this.FileStorageConfig.Path, this.SerializeAll(GetAll().Select(x => x.Id == id ? source : x)));

            return source;
        }

        public TSource Upsert(TSource source)
        {
            var items = GetAll();

            // используя рефлексию (свой класс) ищем нужные (сейчас недоступные) абстрактные поля
            var existing = ReflectionSearch.FindTSource(items, source);

            if (existing is null)
                return Create(source);

            // достаём Id, он есть у всех
            var idProp = ReflectionSearch.FindProp(existing.GetType(), "Id");
            var idObj = idProp.GetValue(existing);

            var id = (int)Convert.ChangeType(idObj, typeof(int));

            return Update(id, source);
        }

        // ---------------------------------------------------------------------------------------

        public abstract void ShowInfo(TSource source);

        public void SaveLastId(FileStorageConfig jsonConfig)
        {
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
                    appDbConfig.Database.Doctors.Xml.LastId = jsonConfig.LastId;
                }
                else if (appDbConfig.Database.Patients.Equals(jsonConfig))
                {
                    appDbConfig.Database.Patients.Xml.LastId = jsonConfig.LastId;
                }
                else if (appDbConfig.Database.Appointments.Equals(jsonConfig))
                {
                    appDbConfig.Database.Appointments.Xml.LastId = jsonConfig.LastId;
                }

                appDbConfig = this.MoveGlobalPathAppSettings(appDbConfig);

                // appsettings.json
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
            appDbConfig.Database.Doctors.Xml.Path = Path.Combine(Constants.PrjData, appDbConfig.Database.Doctors.Xml.Path);
            appDbConfig.Database.Patients.Xml.Path = Path.Combine(Constants.PrjData, appDbConfig.Database.Patients.Xml.Path);
            appDbConfig.Database.Appointments.Xml.Path = Path.Combine(Constants.PrjData, appDbConfig.Database.Appointments.Xml.Path);

            return appDbConfig;
        }

        protected AppDbConfig MoveGlobalPathAppSettings(AppDbConfig appDbConfig)
        {
            // в файле храним относительные пути, при работе программы динамически удаляем путь при записи
            appDbConfig.Database.Doctors.Xml.Path = Path.GetRelativePath(Constants.PrjData, appDbConfig.Database.Doctors.Xml.Path);
            appDbConfig.Database.Patients.Xml.Path = Path.GetRelativePath(Constants.PrjData, appDbConfig.Database.Patients.Xml.Path);
            appDbConfig.Database.Appointments.Xml.Path = Path.GetRelativePath(Constants.PrjData, appDbConfig.Database.Appointments.Xml.Path);

            return appDbConfig;
        }

        protected virtual string SerializeAll(IEnumerable<TSource> items)
        {
            var list = items.ToList();
            var serializer = new XmlSerializer(typeof(List<TSource>));

            using var stringWriter = new StringWriter();
            serializer.Serialize(stringWriter, list);

            return stringWriter.ToString();
        }

        protected virtual IEnumerable<TSource> DeserializeAll(string xml)
        {
            if (string.IsNullOrWhiteSpace(xml))
                return Enumerable.Empty<TSource>();

            var serializer = new XmlSerializer(typeof(List<TSource>));

            using var reader = new StringReader(xml);

            var result = serializer.Deserialize(reader) as List<TSource>;

            return result ?? Enumerable.Empty<TSource>();
        }
    }
}

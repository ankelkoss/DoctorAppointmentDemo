using DoctorAppointmentDemo.Data.Interfaces;
using DoctorAppointmentDemo.Data.Repositories.Base;
using DoctorAppointmentDemo.Domain.DbConfig;
using DoctorAppointmentDemo.Domain.Entities;
using DoctorAppointmentDemo.Domain.Enums;

namespace DoctorAppointmentDemo.Data.Repositories.Xml
{
    public class XmlPatientRepository : XmlGenericRepository<Patient>, IPatientRepository
    {
        public override FileStorageConfig FileStorageConfig { get; set; }

        public XmlPatientRepository()
        {
            FileStorageConfig = ReadFromAppSettings().Database.Patients.Xml;
        }

        public override void ShowInfo(Patient source)
        {
            string info = string.Format("Пациент: {0} {1}, тел: {2}, описание: {3}, тип: {4}", source.Name, source.Surname, source.Phone, source.AdditionalInfo, source.IllnessType.GetDescription());
            Console.WriteLine(info);
        }
    }
}

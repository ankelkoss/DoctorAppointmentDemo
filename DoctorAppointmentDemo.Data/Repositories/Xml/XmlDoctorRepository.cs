using DoctorAppointmentDemo.Data.Interfaces;
using DoctorAppointmentDemo.Data.Repositories.Base;
using DoctorAppointmentDemo.Domain.DbConfig;
using DoctorAppointmentDemo.Domain.Entities;
using DoctorAppointmentDemo.Domain.Enums;

namespace DoctorAppointmentDemo.Data.Repositories.Xml
{
    public class XmlDoctorRepository : XmlGenericRepository<Doctor>, IDoctorRepository
    {
        public override FileStorageConfig FileStorageConfig { get; set; }

        public XmlDoctorRepository()
        {
            FileStorageConfig = ReadFromAppSettings().Database.Doctors.Xml;
        }

        public override void ShowInfo(Doctor doctor)
        {
            string info = string.Format("Доктор: {0} {1}, тел: {2}, стаж: {3}, зп: {4}, тип: {5}", doctor.Name, doctor.Surname, doctor.Phone, doctor.Experience, doctor.Salary, doctor.DoctorType.GetDescription());
            Console.WriteLine(info);
        }
    }
}

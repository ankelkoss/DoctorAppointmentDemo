using DoctorAppointmentDemo.Data.Interfaces;
using DoctorAppointmentDemo.Data.Repositories.Base;
using DoctorAppointmentDemo.Data.Repositories.Json;
using DoctorAppointmentDemo.Domain.DbConfig;
using DoctorAppointmentDemo.Domain.Dto;
using DoctorAppointmentDemo.Domain.Entities;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace DoctorAppointmentDemo.Data.Repositories.Xml
{
    public class XmlAppointmentRepository : XmlGenericRepository<Appointment>, IAppointmentRepository
    {
        public override FileStorageConfig FileStorageConfig { get; set; }
        private readonly IDoctorRepository _doctorRepository;
        private readonly IPatientRepository _patientRepository;

        public XmlAppointmentRepository()
        {
            FileStorageConfig = ReadFromAppSettings().Database.Appointments.Xml;
            _doctorRepository = new XmlDoctorRepository();
            _patientRepository = new XmlPatientRepository();
        }

        public override void ShowInfo(Appointment source)
        {
            string info = string.Format("Запись к доктору: {0} {1}, пациента: {2} {3}, на: {4}", source.Doctor!.Name, source.Doctor.Surname, source.Patient!.Name, source.Patient.Surname, source.DateTimeFrom);
            Console.WriteLine(info);
        }

        protected override IEnumerable<Appointment> DeserializeAll(string xml)
        {
            if (string.IsNullOrWhiteSpace(xml))
                return Enumerable.Empty<Appointment>();

            List<Appointment> result = new();

            var serializer = new XmlSerializer(typeof(List<AppointmentDTO>));
            using var reader = new StringReader(xml);

            var appointmentDTO = serializer.Deserialize(reader) as List<AppointmentDTO> ?? new List<AppointmentDTO>();

            IEnumerable<Doctor> doctors = _doctorRepository.GetAll();
            IEnumerable<Patient> patients = _patientRepository.GetAll();

            foreach (var dto in appointmentDTO)
            {
                result.Add(new Appointment
                {
                    Id = dto.Id,
                    CreatedAt = dto.CreatedAt,
                    UpdatedAt = dto.UpdatedAt,
                    DateTimeFrom = dto.DateTimeFrom,
                    DateTimeTo = dto.DateTimeTo,
                    Description = dto.Description,
                    Doctor = doctors.Where(x => x.Id == dto.DoctorId).FirstOrDefault(),
                    Patient = patients.Where(x => x.Id == dto.PatientId).FirstOrDefault()
                });
            }

            return result;
        }

        protected override string SerializeAll(IEnumerable<Appointment> items)
        {
            List<AppointmentDTO> appointmentDTO = new();

            foreach (var item in items)
            {
                appointmentDTO.Add(new AppointmentDTO()
                {
                    Id = item.Id,
                    CreatedAt = item.CreatedAt,
                    UpdatedAt = item.UpdatedAt,
                    DateTimeFrom = item.DateTimeFrom,
                    DateTimeTo = item.DateTimeTo,
                    Description = item.Description,
                    DoctorId = item.Doctor!.Id,
                    PatientId = item.Patient!.Id
                });
            }

            var serializer = new XmlSerializer(typeof(List<AppointmentDTO>));
            using var stringWriter = new StringWriter();
            serializer.Serialize(stringWriter, appointmentDTO);
            return stringWriter.ToString();
        }
    }
}
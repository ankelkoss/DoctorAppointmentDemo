using DoctorAppointmentDemo.Data.Interfaces;
using DoctorAppointmentDemo.Data.Repositories.Base;
using DoctorAppointmentDemo.Domain.DbConfig;
using DoctorAppointmentDemo.Domain.Dto;
using DoctorAppointmentDemo.Domain.Entities;
using Newtonsoft.Json;

namespace DoctorAppointmentDemo.Data.Repositories
{
    public class AppointmentRepository : GenericRepository<Appointment>, IAppointmentRepository
    {
        public override JsonConfig JsonConfig { get; set; }
        private readonly IDoctorRepository _doctorRepository;
        private readonly IPatientRepository _patientRepository;

        public AppointmentRepository()
        {
            JsonConfig = base.ReadFromAppSettings().Database.Appointments;
            _doctorRepository = new DoctorRepository();
            _patientRepository = new PatientRepository();
        }

        public override void ShowInfo(Appointment source)
        {
            string info = string.Format("Запись к доктору: {0} {1}, пациента: {2} {3}, на: {4}", source.Doctor!.Name, source.Doctor.Surname, source.Patient!.Name, source.Patient.Surname, source.DateTimeFrom);
            Console.WriteLine(info);
        }

        protected override IEnumerable<Appointment> DeserializeAll(string json)
        {
            List<Appointment> result = new ();
            List<AppointmentDTO> appointmentDTO = JsonConvert.DeserializeObject<List<AppointmentDTO>>(json) ?? new();

            IEnumerable<Doctor> doctors = _doctorRepository.GetAll();
            IEnumerable<Patient> patients = _patientRepository.GetAll();

            foreach (var dto in appointmentDTO)
            {
                result.Add(new Appointment()
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
            List<AppointmentDTO> appointmentDTOs = new ();

            foreach (var item in items)
            {
                appointmentDTOs.Add(new AppointmentDTO()
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

            return JsonConvert.SerializeObject(appointmentDTOs, Formatting.Indented);
        }
    }
}

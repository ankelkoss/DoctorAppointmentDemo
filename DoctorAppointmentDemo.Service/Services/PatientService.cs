using DoctorAppointmentDemo.Data.Interfaces;
using DoctorAppointmentDemo.Data.Repositories;
using DoctorAppointmentDemo.Service.Interfaces;
using DoctorAppointmentDemo.Domain.Entities;

namespace DoctorAppointmentDemo.Service.Services
{
    public class PatientService : IPatientService
    {
        private readonly IPatientRepository _patientRepository;

        public PatientService()
        {
            _patientRepository = new PatientRepository();
        }
        public Patient Create(Patient patient)
        {
            return _patientRepository.Create(patient);
        }

        public bool Delete(int id)
        {
            return _patientRepository.Delete(id);
        }

        public Patient? Get(int id)
        {
            return _patientRepository.GetById(id);
        }

        public IEnumerable<Patient> GetAll()
        {
            return _patientRepository.GetAll();
        }

        public Patient Update(int id, Patient patient)
        {
            return _patientRepository.Update(id, patient);
        }

        public Patient Upsert(Patient patient)
        {
            return _patientRepository.Upsert(patient);
        }
    }
}

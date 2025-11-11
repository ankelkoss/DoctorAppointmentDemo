using DoctorAppointmentDemo.Domain.Entities;
using DoctorAppointmentDemo.Service.Services;

namespace DoctorAppointmentDemo.UI
{
    public static class Program
    {
        public static void Main()
        {
            //var doctorAppointment = new DoctorAppointment();
            //doctorAppointment.Menu();

            var patientService = new PatientService();
            var doctorService = new DoctorService();
            var appointmentService = new AppointmentService();

            Doctor doctor = doctorService.GetAll().First();

            Patient patinet = new Patient()
            {
                Name = "Тарас",
                Surname = "Тарасович",
                Address = "Украина, Киев, ул. Прорезная 12 кв 3",
                Phone = "+380931234567",
                Email = "taras.tarasovich@gmail.com",
                AdditionalInfo = "somithing",
                IllnessType = Domain.Enums.IllnessTypes.EyeDisease
            };

            patinet = patientService.Create(patinet);

            var appointment = new Appointment()
            {
                Doctor = doctor,
                Patient = patinet,
                DateTimeFrom = DateTime.Parse("20.11.2025 14:00"),
                DateTimeTo = DateTime.Parse("20.11.2025 15:00"),
                Description = "Something"
            };

            var r = appointmentService.Create(appointment);


        }
    }
}
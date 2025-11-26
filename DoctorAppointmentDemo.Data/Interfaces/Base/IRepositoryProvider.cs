namespace DoctorAppointmentDemo.Data.Interfaces.Base
{
    public interface IRepositoryProvider
    {
        IDoctorRepository Doctors { get; }
        IPatientRepository Patients { get; }
        IAppointmentRepository Appointments { get; }
    }
}

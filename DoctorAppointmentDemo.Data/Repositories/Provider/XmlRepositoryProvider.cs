using DoctorAppointmentDemo.Data.Interfaces;
using DoctorAppointmentDemo.Data.Interfaces.Base;
using DoctorAppointmentDemo.Data.Repositories.Xml;

namespace DoctorAppointmentDemo.Data.Repositories.Provider
{
    public class XmlRepositoryProvider : IRepositoryProvider
    {
        public IDoctorRepository Doctors { get; }
        public IPatientRepository Patients { get; }
        public IAppointmentRepository Appointments { get; }

        public XmlRepositoryProvider()
        {
            Doctors = new XmlDoctorRepository();
            Patients = new XmlPatientRepository();
            Appointments = new XmlAppointmentRepository();
        }
    }
}

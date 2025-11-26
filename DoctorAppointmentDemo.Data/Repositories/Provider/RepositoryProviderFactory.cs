using DoctorAppointmentDemo.Data.Interfaces.Base;
using DoctorAppointmentDemo.Domain.Enums;

namespace DoctorAppointmentDemo.Data.Repositories.Provider
{
    public static class RepositoryProviderFactory
    {
        public static IRepositoryProvider Create(StorageTypeEnum storageType)
        {
            return storageType switch
            {
                StorageTypeEnum.Json => new JsonRepositoryProvider(),
                StorageTypeEnum.Xml => new XmlRepositoryProvider(),
      
                _ => throw new ArgumentOutOfRangeException(nameof(storageType), storageType, null)
            };
        }
    }
}

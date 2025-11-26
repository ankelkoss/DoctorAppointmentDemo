namespace DoctorAppointmentDemo.Domain.DbConfig
{
    public sealed class FileStorageConfig
    {
        public int LastId { get; set; }
        public string Path { get; set; } = string.Empty;

        public override bool Equals(object? obj)
        {
            if (obj is FileStorageConfig config)
            {
                return Path == config.Path;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Path.GetHashCode();
        }
    }
}

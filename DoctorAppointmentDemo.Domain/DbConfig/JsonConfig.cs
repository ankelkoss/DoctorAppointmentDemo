namespace DoctorAppointmentDemo.Domain.DbConfig
{
    public sealed class JsonConfig
    {
        public int LastId { get; set; }
        public string Path { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj is JsonConfig config)
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

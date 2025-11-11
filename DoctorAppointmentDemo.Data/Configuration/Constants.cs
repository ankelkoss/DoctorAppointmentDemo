namespace MyDoctorAppointment.Data.Configuration
{
    public static class Constants
    {
        public static bool IsDebug =>
#if DEBUG
                true;
#else
                false;
#endif
        // заменить на путь валидный для вашей директории на пк (в будущем будем использовать относительный путь)
        //public const string AppSettingsPath = @"C:\\Users\\admin\\source\\repos\\DoctorAppointmentDemo\\DoctorAppointmentDemo.Data\\Configuration\\appsettings.json";

        public static readonly string Base = AppContext.BaseDirectory;
        public static readonly string SolutionDir = IsDebug
            ? Directory.GetParent(Base)!.Parent!.Parent!.Parent!.Parent!.FullName
            : throw new Exception("Для продакшена нужно изменить путь в Constants.SolutionDir");

        public static readonly string PrjData = Path.Combine(SolutionDir, @"DoctorAppointmentDemo.Data");
        public static readonly string PrjDomain = Path.Combine(SolutionDir, @"DoctorAppointmentDemo.Domain");
        public static readonly string PrjService = Path.Combine(SolutionDir, @"DoctorAppointmentDemo.Service");
        public static readonly string PrjUi = Path.Combine(SolutionDir, @"DoctorAppointmentDemo.UI");

        public static readonly string AppSettingsPath = Path.Combine(PrjData, @"Configuration\appsettings.json");
    }
}

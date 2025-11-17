using System.ComponentModel;
using System.Reflection;

namespace DoctorAppointmentDemo.Domain.Enums
{
    public enum IllnessTypes
    {
        [Description("Заболевание глаз")]
        EyeDisease = 1,

        [Description("Инфекционное заболевание")]
        Infection,

        [Description("Зубное заболевание")]
        DentalDisease,

        [Description("Кожное заболевание")]
        SkinDisease,

        [Description("Скорая помощь / неотложное состояние")]
        Ambulance,
    }

    public static class IllnessTypesExtensions
    {
        public static string GetDescription(this IllnessTypes value)
        {
            var type = value.GetType();
            var member = type.GetMember(value.ToString());

            if (member.Length > 0)
            {
                var attr = member[0].GetCustomAttribute<DescriptionAttribute>();
                if (attr != null)
                    return attr.Description;
            }

            return value.ToString(); // fallback, если атрибута нет
        }
    }
}

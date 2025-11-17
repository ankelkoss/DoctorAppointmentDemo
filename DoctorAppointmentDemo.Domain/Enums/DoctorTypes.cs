using System.ComponentModel;
using System.Reflection;

namespace DoctorAppointmentDemo.Domain.Enums
{
    public enum DoctorTypes
    {
        [Description("Стоматолог")]
        Dentist = 1,

        [Description("Дерматолог")]
        Dermatologist,

        [Description("Семейный врач")]
        FamilyDoctor,

        [Description("Фельдшер")]
        Paramedic
    }

    public static class DoctorTypesExtensions
    {
        public static string GetDescription(this DoctorTypes value)
        {
            var type = value.GetType();
            var member = type.GetMember(value.ToString());

            if (member.Length > 0)
            {
                var attr = member[0].GetCustomAttribute<DescriptionAttribute>();
                if (attr != null)
                    return attr.Description;
            }

            return value.ToString(); // fallback
        }
    }
}

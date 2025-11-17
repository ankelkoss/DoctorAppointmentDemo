using System.Reflection;
using System.Text.RegularExpressions;

namespace DoctorAppointmentDemo.Data.Repositories.Base
{
    public static class ReflectionSearch
    {
        public static TSource? FindTSource<TSource>(IEnumerable<TSource> items, TSource source)
        {
            var type = source!.GetType();

            // любой UserBase наследник
            var phoneProp = FindProp(type, "Phone");
            if (phoneProp != null)
            {
                var srcPhone = phoneProp.GetValue(source)?.ToString();
                if (string.IsNullOrEmpty(srcPhone))
                    return default;

                return items.FirstOrDefault(x =>
                {
                    var p = FindProp(x!.GetType(), "Phone");
                    if (p is null) return false;
                    return PhonesEqual(p.GetValue(x)?.ToString(), srcPhone);
                });
            }

            // Appointment серч по Doctor.Phone и Patient.Phone
            // или доктор или пациен могут быть нал (в моделе)
            if (FindProp(type, "Doctor") != null && FindProp(type, "Patient") != null)
            {
                var srcDoctorPhone = GetNestedValue(source, "Doctor.Phone")?.ToString();
                var srcPatientPhone = GetNestedValue(source, "Patient.Phone")?.ToString();
                // TODO: по хорошему нужно отсеивать старые записи к врачу (старше сегодняшней даты) 
                
                // если оба пустые
                if (string.IsNullOrEmpty(srcDoctorPhone) && string.IsNullOrEmpty(srcPatientPhone))
                    return default;

                var now = DateTime.Now;

                return items.FirstOrDefault(x =>
                {
                    // проверяем не протухла ли запись к врачу
                    // если протухла то создаем новую
                    string dtObj = GetNestedValue(x, "DateTimeFrom")!.ToString();
                    DateTime dateTimeFrom = DateTime.Parse(dtObj);

                    if(dateTimeFrom <= now) 
                        return false;

                    var docPhone = GetNestedValue(x!, "Doctor.Phone")?.ToString();
                    var patPhone = GetNestedValue(x!, "Patient.Phone")?.ToString();

                    bool matchDoc = !string.IsNullOrEmpty(srcDoctorPhone) && PhonesEqual(docPhone, srcDoctorPhone);
                    bool matchPat = !string.IsNullOrEmpty(srcPatientPhone) && PhonesEqual(patPhone, srcPatientPhone);

                    return matchDoc || matchPat;
                });
            }

            return default;
        }


        // Получаем тип объекта
        public static PropertyInfo? FindProp(Type type, string name)
        {
            return type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
        }
        
        //
        private static object? GetNestedValue(object? obj, string prop)
        {
            if (obj is null || string.IsNullOrWhiteSpace(prop)) 
                return null;

            // идем по вложенному классу и ищем prop
            foreach (var item in prop.Split('.'))
            {
                var type = obj!.GetType();
                var propFind = FindProp(type, item);

                if (propFind is null) 
                    return null;

                obj = propFind.GetValue(obj);
            }
            return obj;
        }

        //
        private static bool PhonesEqual(string? a, string? b)
        {
            if (string.IsNullOrEmpty(a) || string.IsNullOrEmpty(b)) 
                return false;

            a = clearNumber(a);
            b = clearNumber(b);

            string clearNumber(string number)
            {
                number = Regex.Replace(number, @"\D", ""); // удаляем всё, что НЕ цифра

                // берем все что после 0 (включительно) в номере 
                // +380 ХХХ ХХ ХХ
                int index = number.IndexOf('0');

                return index >= 0 ? number[index..] : number;
            }

            return string.Equals(a, b, StringComparison.Ordinal);
        }
    }
}

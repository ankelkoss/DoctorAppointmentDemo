using DoctorAppointmentDemo.Domain.Enums;
using Spectre.Console;

namespace DoctorAppointmentDemo.UI.Helpers
{
    public static class ConsoleInputHelpers
    {
        //стринга с пустым значением, впомогательный метод
        public static string PromptStringAllowEmpty(string message)
        {
            return AnsiConsole.Prompt(
                new TextPrompt<string>(message)
                    .AllowEmpty()
            );
        }

        // обязательнный ввод строка
        public static string AskRequired(string question)
        {
            while (true)
            {
                var value = PromptStringAllowEmpty(question);

                if (!string.IsNullOrWhiteSpace(value))
                    return value;

                AnsiConsole.MarkupLine("[red]Поле обязательно для заполнения. Попробуйте ещё раз.[/]");
            }
        }

        // опциаонально, стринг или нал
        public static string? AskOptional(string question)
        {
            var value = PromptStringAllowEmpty(question);
            return string.IsNullOrWhiteSpace(value) ? null : value;
        }

        // установить новое или старое значение
        public static string? AskWithDefault(string fieldName, string? current)
        {
            var label = string.IsNullOrEmpty(current) ? "пусто" : current!;
            var input = PromptStringAllowEmpty(
                $"{fieldName} ([grey]текущее: {label}[/], Enter — оставить):");

            if (string.IsNullOrWhiteSpace(input))
                return current;

            return input;
        }

        // обязательный ввод число
        public static int AskInt(string question)
        {
            while (true)
            {
                var input = PromptStringAllowEmpty(question);
                if (int.TryParse(input, out var value))
                    return value;

                AnsiConsole.MarkupLine("[red]Некорректное число. Попробуйте ещё раз.[/]");
            }
        }

        // обятельный, ограниченый ввод число
        public static byte AskByte(string question)
        {
            while (true)
            {
                var input = PromptStringAllowEmpty(question);
                if (byte.TryParse(input, out var value))
                    return value;

                AnsiConsole.MarkupLine("[red]Некорректное значение (0-255). Попробуйте ещё раз.[/]");
            }
        }

        // установить новое число с ограничением или старое значение
        public static byte AskByteWithDefault(string fieldName, byte current)
        {
            var input = PromptStringAllowEmpty(
                $"{fieldName} ([grey]текущее: {current}[/], Enter — оставить):");

            if (string.IsNullOrWhiteSpace(input))
                return current;

            if (byte.TryParse(input, out var value))
                return value;

            AnsiConsole.MarkupLine("[red]Некорректное значение. Оставлено текущее.[/]");
            return current;
        }

        // обятельный, установить новое число
        public static decimal AskDecimal(string question)
        {
            while (true)
            {
                var input = PromptStringAllowEmpty(question);
                if (decimal.TryParse(input, out var value))
                    return value;

                AnsiConsole.MarkupLine("[red]Некорректное число. Попробуйте ещё раз.[/]");
            }
        }

        // установить новое число или старое значение
        public static decimal AskDecimalWithDefault(string fieldName, decimal current)
        {
            var input = PromptStringAllowEmpty(
                $"{fieldName} ([grey]текущее: {current}[/], Enter — оставить):");

            if (string.IsNullOrWhiteSpace(input))
                return current;

            if (decimal.TryParse(input, out var value))
                return value;

            AnsiConsole.MarkupLine("[red]Некорректное число. Оставлено текущее.[/]");
            return current;
        }

        // DateTime  обятельный, установить новое число
        public static DateTime AskDateTime(string question)
        {
            while (true)
            {
                var input = PromptStringAllowEmpty(question);

                if (DateTime.TryParse(input, out var value))
                    return value;

                AnsiConsole.MarkupLine("[red]Некорректный формат даты/времени. Используйте формат yyyy-MM-dd HH:mm.[/]");
            }
        }

        // DateTime или старое значение
        public static DateTime AskDateTimeWithDefault(string fieldName, DateTime current)
        {
            var input = PromptStringAllowEmpty(
                $"{fieldName} ([grey]текущее: {current:yyyy-MM-dd HH:mm}[/], Enter — оставить):");

            if (string.IsNullOrWhiteSpace(input))
                return current;

            if (DateTime.TryParse(input, out var value))
                return value;

            AnsiConsole.MarkupLine("[red]Некорректный формат. Оставлено текущее значение.[/]");
            return current;
        }

        // выбор типа болезни пациента по enum IllnessTypes
        public static IllnessTypes AskIllnessType()
        {
            return AnsiConsole.Prompt(
                new SelectionPrompt<IllnessTypes>()
                    .Title("Выберите [green]тип болезни[/]:")
                    .UseConverter(t => t switch
                    {
                        IllnessTypes.EyeDisease => "EyeDisease (Заболевание глаз)",
                        IllnessTypes.Infection => "Infection (Инфекция)",
                        IllnessTypes.DentalDisease => "DentalDisease (Зубное заболевание)",
                        IllnessTypes.SkinDisease => "SkinDisease (Кожное заболевание)",
                        IllnessTypes.Ambulance => "Ambulance (Скорая помощь)",
                        _ => t.ToString()
                    })
                    .AddChoices(Enum.GetValues<IllnessTypes>())
            );
        }
    }
}

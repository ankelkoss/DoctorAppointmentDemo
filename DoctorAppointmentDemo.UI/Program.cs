using System.Text;
using DoctorAppointmentDemo.Data.Configuration;
using DoctorAppointmentDemo.Domain.Enums;
using Spectre.Console;

namespace DoctorAppointmentDemo.UI
{
    public static class Program
    {
        static void DelLocFile()
        {
            // удаление лок и бек файлов если они остались после внезапного завершения программы
            // GenericRepository > SaveLastId

            var currentPath = Constants.AppSettingsPath;
            var lockPath = currentPath + ".lck";
            var tmpPath = currentPath + ".tmp";

            if (File.Exists(lockPath))
            {
                File.Delete(lockPath);
            }

            if (File.Exists(tmpPath))
            {
                File.Delete(tmpPath);
            }
        }
        public static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            StartEffect.Matrix();
            StartEffect.Banner();

            var doctorAppointment = new DoctorAppointment();
            var patientAppointment = new PatientAppointment();
            var appAppointment = new AppAppointment();

            DelLocFile();

            var exit = false;

            while (!exit)
            {
                AnsiConsole.Clear();
                AnsiConsole.MarkupLine("[bold cyan]DoctorAppointmentDemo[/]");
                AnsiConsole.WriteLine();

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<MainMenuEnum>()
                        .Title("Выберите [green]раздел[/]:")
                        .UseConverter(item => item switch
                        {
                            MainMenuEnum.DoctorMenu => "Управление докторами",
                            MainMenuEnum.PatientMenu => "Управление пациентами",
                            MainMenuEnum.AppointmentMenu => "Управление записями на приём",
                            MainMenuEnum.Exit => "Выход",

                            _ => item.ToString()
                        })
                        .AddChoices(Enum.GetValues<MainMenuEnum>()));

                switch (choice)
                {
                    case MainMenuEnum.DoctorMenu:
                        doctorAppointment.Menu();
                        break;

                    case MainMenuEnum.PatientMenu:
                        patientAppointment.Menu();
                        break;

                    case MainMenuEnum.AppointmentMenu:
                        appAppointment.Menu();
                        break;

                    case MainMenuEnum.Exit:
                        exit = true;
                        break;
                }
            }

            AnsiConsole.Clear();
            AnsiConsole.MarkupLine("[grey]Приложение завершено.[/]");
        }
    }
}
